using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace CardMatch
{
    enum GameStateEnum
    {
        NOT_INITIALIZED,
        SELECTING_FIRST,
        SELECTING_SECOND,
        GAME_FINISHED,
        RESTART
    }

    public class GameManager : MonoBehaviour
    {
        private string playerName;

        [SerializeField]
        GameObject originalCardPrefab;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        SaveState saveState;

        [SerializeField]
        private GameScore gameScore = new GameScore();

        private GameStateEnum gameState;

        private UnityEvent<GameStateEnum> OnGameStateChange;

        private ArrayList cardSelection = new ArrayList();

        private int numCardsW = 2;

        private int numCardsH = 2;

        private int pairMatched;

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

        private ArrayList cardPairPositions = new ArrayList();

        private UnityEvent onMatchEvent;

        private int GetTotalPairs()
        {
            return (numCardsW * numCardsH) / 2;
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
            gameCard.ChangeSprite(cardSpriteName);
            return gameCard;
        }

        private void UpdateLayout(bool shuffle = false)
        {
            float xOffset = (float)(Screen.width / (numCardsW + 1.0));
            float yOffset = (float)(Screen.height / (numCardsH + 1.0));

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

        private void CreateCards()
        {
            if ((numCardsW * numCardsH) % 2 != 0)
            {
                Debug.LogError("You should number of cards in pairs, but you have: " + (numCardsW * numCardsH) + ", number of cards");
            }

            Array cardNamesArray = Enum.GetValues(typeof(CardName));
            Utils.Shuffle(cardNamesArray);

            int cardNameIdx = 0;
            int totalCardsBy2 = (numCardsW * numCardsH) / 2; //  by 2 because I make a pair
            for (int i = 0; i < totalCardsBy2; i++)
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
        }

        void EnableAllClicks(bool enable = true)
        {
            foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
            {
                string key = entry.Key;
                CardPair cardPair = entry.Value;
                cardPair.cardA.CanFlipCard = enable;
                cardPair.cardB.CanFlipCard = enable;
            }
        }

        IEnumerator ResetAll()
        {
            yield return new WaitForSeconds(2);

            pairMatched = 0;
            cardSelection.Clear();

            yield return new WaitForSeconds(2);

            foreach (KeyValuePair<string, CardPair> entry in cardPairMap)
            {
                string key = entry.Key;
                CardPair cardPair = entry.Value;
                cardPair.cardA.ChangeBackSprite();
                cardPair.cardB.ChangeBackSprite();
            }
        }

        void OnFlipCardEvent(GameCard GameCard)
        {
            cardSelection.Add(GameCard);

            // flip both cards if they are equal
            if (cardSelection.Count == 2)
            {
                GameCard card0 = (GameCard)cardSelection[0];
                GameCard card1 = (GameCard)cardSelection[1];

                if (card0.name == card1.name)
                {
                    card0.DoEffectSelection();
                    card1.DoEffectSelection();

                    if (++pairMatched == GetTotalPairs())
                    {
                        OnGameStateChange.Invoke(GameStateEnum.GAME_FINISHED);
                    }
                }
                else
                {
                    // flip back again both cards
                    card0.ChangeBackSprite();
                    card1.ChangeBackSprite();
                }

                cardSelection.Clear();
            }
        }

        void OnGameStateChangeFnc(GameStateEnum gameState)
        {
            this.gameState = gameState;

            if (gameState == GameStateEnum.GAME_FINISHED)
            {
                Debug.Log("GAME FINISHED!");
                StartCoroutine(ResetAll());
            }
            else if (gameState == GameStateEnum.RESTART)
            {
                Debug.Log("GAME RESTART!");

                gameScore.UpdateScore(playerName);
            }
        }

        private void Awake()
        {
            OnGameStateChange?.Invoke(GameStateEnum.NOT_INITIALIZED);
        }

        void Start()
        {
            OnGameStateChange?.AddListener(OnGameStateChangeFnc);

            CreateCards();

            UpdateLayout();
        }

        void Update()
        {
        
        }
    }
}
