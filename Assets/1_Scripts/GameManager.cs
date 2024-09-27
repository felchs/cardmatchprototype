using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CardMatch
{
    enum GameStateEnum
    {
        NOT_INITIALIZED,
        PLAYING_SELECTING,
        FLIPPING_BACK,
        GAME_FINISHED,
        RESTART
    }

    /*
    public class WordPart
    {
        public string wordPart;

        public WordPart(string wordPart)
        {
            this.wordPart = wordPart;
        }
    }

    public class Word
    {
        public List<WordPart> word = new List<WordPart>();

        public Word(WordPart p1, WordPart p2)
        {
            word.Add(p1);
            word.Add(p2);
        }
    }
    */

    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        TMP_Text playerName;

        [SerializeField]
        TMP_Text textNumMatches;

        [SerializeField]
        GameObject startPanel;

        [SerializeField]
        GameObject winPanel;

        [SerializeField]
        GameObject scorenPanel;

        [SerializeField]
        GameObject originalCardPrefab;

        [SerializeField]
        GameObject canvas;

        [SerializeField]
        TMP_Text gameType;

        [SerializeField]
        private GameScore gameScore;

        [SerializeField]
        private AudioPlayer audioPlayer;

        private GameStateEnum gameState;

        private UnityEvent<GameStateEnum> OnGameStateChange;

        private ArrayList cardSelection = new ArrayList();

        private int numCardsW = 0;

        private int numCardsH = 0;

        private int pairMatched;

        private Stopwatch stopwatch;

        private CardPair currentWord;

        //private List<WordPart> wordParts = new List<WordPart>();

        //private List<Word> wordList = new List<Word>();

        private bool mario = false;

        class CardPair
        {
            public CardPair(GameCard cardA, GameCard cardB)
            {
                this.cardA = cardA;
                this.cardB = cardB;
            }

            // just a container so use as public
            public GameCard cardA;
            public GameCard cardB;
        }

        private Dictionary<string, CardPair> cardPairMap = new Dictionary<string, CardPair>();

        private Dictionary<string, GameCard> headCardMap = new Dictionary<string, GameCard>();

        private List<Vector2> cardPairPositions = new List<Vector2>();

        private List<Vector2> cardHeadPositions = new List<Vector2>();

        private UnityEvent onMatchEvent;

        private int GetTotalPairs()
        {
            return (numCardsW * numCardsH) / 2;
        }

        public string GetGameType()
        {
            return numCardsW + "x" + numCardsH;
        }

        public GameCard GetGameCard(string spriteName, string id)
        {
            CardPair cardPair = cardPairMap[spriteName];
            return id == "A" ? cardPair.cardA : cardPair.cardB;
        }

        private GameCard CreateNewGameCard(string cardSpriteName, string cardId)
        {
            GameObject gameObject = Instantiate(originalCardPrefab, canvas.transform);
            gameObject.name = cardSpriteName + "_" + cardId;

            GameCard gameCard = gameObject.GetComponent<GameCard>();
            gameCard.CardId = cardId;
            StartCoroutine(gameCard.ChangeSprite(cardSpriteName, 0));
            return gameCard;
        }

        private void UpdateLayout(bool shuffle = false)
        {
            if (!mario)
            {
                //
                // update memory cards
                //
                Vector2 sz = currentWord.cardA.GetSize();

                int numHeadCards = 2;
                float xOffsetHead = (float)(Screen.width / (numHeadCards + 1.0));
                float yOffsetHead = (float)(Screen.height - (sz.y / 2.0 + 10));

                currentWord.cardA.gameObject.transform.position = new Vector2(xOffsetHead * 1, yOffsetHead);
                currentWord.cardB.gameObject.transform.position = new Vector2(xOffsetHead * 2, yOffsetHead);
            }

            //
            // update memory cards
            //
            float xOffset = (float)(Screen.width / (numCardsW + 1.0));
            float yOffset = (float)(Screen.height / (numCardsH + (mario ? 1 : 3.0))); // 3.0 a little space of 3 cards

            for (int i = 0; i < numCardsW; i++)
            {
                for (int j = 0; j < numCardsH; j++)
                {
                    //int idx = i * numCardsW + j;
                    float xPos = i * xOffset + xOffset;
                    float yPos = j * yOffset + yOffset;
                    Vector2 vec = new Vector2(xPos, yPos);
                    cardPairPositions.Add(vec);
                }
            }

            if (shuffle)
            {
                Utils.Shuffle(cardPairPositions);
            }

            {
                int idx = 0;
                foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
                {
                    string key = entry.Key;
                    CardPair cardPair = entry.Value;

                    Vector2 posA = (Vector2)cardPairPositions[idx++];
                    cardPair.cardA.gameObject.transform.position = posA;

                    Vector2 posB = (Vector2)cardPairPositions[idx++];
                    cardPair.cardB.gameObject.transform.position = posB;
                }
            }
        }

        private string GetRandomString(string[] strings)
        {
            int index = UnityEngine.Random.Range(0, strings.Length);
            return strings[index];
        }

        private void CreateWordBase()
        {
            // by now hardcode it
            string[] vowels = { "_A", "_E", "_I", "_O", "_U" };
            string[] consonant = { "_B", "_C", "_D", "_F", "_G", "_H", "_J", "_K", "_L", "_M", "_N", "_P", "_QU", "_R", "_S", "_T", "_V", "_X", "_Z", "_cc", "_CH" };

            int totalPairs = GetTotalPairs(); //  by 2 because I make a pair
            for (int i = 0; i < totalPairs - 1; i++)
            {
                string co = GetRandomString(consonant);
                GameCard cardA = CreateNewGameCard(co, co);
                cardA.OnFlipCardEvent.AddListener(OnFlipCardEvent);

                string vo = GetRandomString(vowels);
                GameCard cardB = CreateNewGameCard(vo, vo);
                cardB.OnFlipCardEvent.AddListener(OnFlipCardEvent);

                CardPair cardPair = new CardPair(cardA, cardB);
                string cardNameToUsePair = co + "_" + vo;
                cardPairMap.Add(cardNameToUsePair, cardPair);
            }

            string randomConsonant = GetRandomString(consonant);
            string randomVowel = GetRandomString(vowels);
            this.currentWord = new CardPair(CreateNewGameCard(randomConsonant, randomConsonant),
                                            CreateNewGameCard(randomVowel, randomVowel));

            string nme = randomConsonant + "_" + randomVowel;
            cardPairMap.Add(nme, new CardPair(CreateNewGameCard(randomConsonant, randomConsonant),
                                                            CreateNewGameCard(randomVowel, randomVowel)));
        }

        private void CreateCardsMario()
        {
            if ((numCardsW * numCardsH) % 2 != 0)
            {
                Debug.LogError("You should number of cards in pairs, but you have: " + (numCardsW * numCardsH) + ", number of cards");
            }

            Array cardNamesArray = Enum.GetValues(typeof(CardNameMario));
            Utils.Shuffle(cardNamesArray);

            int cardNameIdx = 0;
            int totalPairs = GetTotalPairs(); //  by 2 because I make a pair
            for (int i = 0; i < totalPairs; i++)
            {
                string cardNameToUse = cardNamesArray.GetValue(cardNameIdx++).ToString();
                GameCard cardA = CreateNewGameCard(cardNameToUse, "A");
                cardA.OnFlipCardEvent.AddListener(OnFlipCardEvent);
                GameCard cardB = CreateNewGameCard(cardNameToUse, "B");
                cardB.OnFlipCardEvent.AddListener(OnFlipCardEvent);
                CardPair cardPair = new CardPair(cardA, cardB);
                cardPairMap.Add(cardNameToUse, cardPair);
            }

            UpdateLayout(true); // only shuffle on creation

            textNumMatches.text = string.Format("Num Matches: {0}/{1} ", 0, GetTotalPairs());
        }

        void EnableAllClicks(bool enable = true)
        {
            foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
            {
                CardPair cardPair = entry.Value;
                cardPair.cardA.CanFlipCard = enable;
                cardPair.cardB.CanFlipCard = enable;
            }
        }

        void ResetAll()
        {
            foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
            {
                CardPair cardPair = entry.Value;
                Destroy(cardPair.cardA.gameObject);
                Destroy(cardPair.cardB.gameObject);
            }

            cardPairMap.Clear();

            cardPairPositions.Clear();

            pairMatched = 0;
            
            textNumMatches.text = string.Format("Num Matches: {0}/{1} ", 0, GetTotalPairs());

            cardSelection.Clear();

            foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
            {
                string key = entry.Key;
                CardPair cardPair = entry.Value;
                StartCoroutine(cardPair.cardA.ChangeBackSprite(0));
                StartCoroutine(cardPair.cardB.ChangeBackSprite(0));
            }
        }

        void OnFlipCardEvent(GameCard gameCard)
        {
            if (gameState == GameStateEnum.FLIPPING_BACK)
            {
                audioPlayer.PlayEffect("wrong");
                cardSelection.Remove(gameCard);
                gameCard.CanFlipCard = true;

                if (cardSelection.Count == 0)
                {
                    EnableAllClicks(true);
                    OnGameStateChange?.Invoke(GameStateEnum.PLAYING_SELECTING);
                }

                return;
            }

            audioPlayer.PlayEffect("flip");

            cardSelection.Add(gameCard);

            gameCard.CanFlipCard = false;

            // flip both cards if they are equal
            if (cardSelection.Count == 2)
            {
                EnableAllClicks(false);

                GameCard card0 = (GameCard)cardSelection[0];
                GameCard card1 = (GameCard)cardSelection[1];

                if (card0.spriteName == card1.spriteName)
                {
                    // do graphic effect
                    audioPlayer.PlayEffect("flipright");
                    card0.DoEffectSelection();
                    card1.DoEffectSelection();

                    // do lock forever
                    card0.Locked = true;
                    card1.Locked = true;

                    // remove from selection
                    cardSelection.Remove(card0);
                    cardSelection.Remove(card1);

                    EnableAllClicks(true);

                    textNumMatches.text = string.Format("Num Matches: {0}/{1} ", (pairMatched + 1), GetTotalPairs());

                    if (++pairMatched == GetTotalPairs())
                    {
                        audioPlayer.StopMusic();
                        OnGameStateChange?.Invoke(GameStateEnum.GAME_FINISHED);
                    }
                }
                else
                {
                    OnGameStateChange?.Invoke(GameStateEnum.FLIPPING_BACK);

                    StartCoroutine(card0.ForceReverseFlipCard(2));
                    StartCoroutine(card1.ForceReverseFlipCard(2));
                }
            }
        }

        void OnGameStateChangeFnc(GameStateEnum gameState)
        {
            this.gameState = gameState;

            if (gameState == GameStateEnum.GAME_FINISHED)
            {
                audioPlayer.PlayEffect("letsgo");

                gameScore.UpdateScore(GetGameType(), playerName.text, stopwatch.timerText.text.Replace("Time:", ""));
                stopwatch.StopTimer();

                DisableAllPanels();
                this.winPanel.SetActive(true);
            }
            else if (gameState == GameStateEnum.RESTART)
            {
                gameScore.UpdateScore(GetGameType(), playerName.text, stopwatch.timerText.text.Replace("Time:", ""));

                stopwatch.ResetTimer();
            }
        }

        private void Awake()
        {
            if (OnGameStateChange == null)
            {
                OnGameStateChange = new UnityEvent<GameStateEnum>();
            }

            OnGameStateChange.AddListener(OnGameStateChangeFnc);
        }

        void Start()
        {
            OnGameStateChange.Invoke(GameStateEnum.NOT_INITIALIZED);

            DisableAllPanels();

            startPanel.SetActive(true);

            this.stopwatch = GetComponent<Stopwatch>();

        }

        void Update()
        {
        }

        //
        // Handling panel stuff
        // Idealy wouldbe nice to do a separate scripts to handle this but by now it's just a simple test
        // I'm doing it on the same GameManager.cs class
        //

        public void DisableAllPanels()
        {
            startPanel.SetActive(false);
            winPanel.SetActive(false);
            scorenPanel.SetActive(false);
        }

        public void OnPanelStartClick()
        {
            audioPlayer.PlayMusic();

            DisableAllPanels();

            string[] val = gameType.text.Split("x");
            this.numCardsW = Int32.Parse(val[0]);
            this.numCardsH = Int32.Parse(val[1]);

            if ((numCardsW * numCardsH) % 2 != 0)
            {
                // should never came here
                Debug.LogError("You should number of cards in pairs, but you have: " + (numCardsW * numCardsH) + ", number of cards");
                return;
            }

            stopwatch.ResetTimer();
            stopwatch.StartTimer();

            ResetAll();

            if (mario)
            {
                CreateCardsMario();
            }
            else
            {
                CreateWordBase();
            }

            UpdateLayout();
        }

        public void OnPanelChangeClick()
        {
            DisableAllPanels();

            startPanel.SetActive(true);
        }

        public void OnScorePanelClick()
        {
            scorenPanel.SetActive(true);
            scorenPanel.GetComponent<GameScore>().initialize();
        }
    }
}
