using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CardMatch
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        GameObject originalCardPrefab;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        SaveState saveState;

        private int numCardsW = 4;

        private int numCardsH = 3;

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

        private GameCard CreateNewGameCard(string cardName)
        {
            GameObject gameObject = Instantiate(originalCardPrefab, canvas.transform);
            gameObject.name = cardName;

            GameCard gameCard = gameObject.GetComponent<GameCard>();
            gameCard.ChangeSprite(cardName);
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
                // Access the key (string) and value (CardPair) from the entry
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
                GameCard cardA = CreateNewGameCard(cardNameToUse);
                GameCard cardB = CreateNewGameCard(cardNameToUse);
                CardPair cardPair = new CardPair(cardA, cardB);
                cardPairMap.Add(cardNameToUse, cardPair);
            }

            UpdateLayout(true); // only shuffle on creation
        }

        void Start()
        {
            CreateCards();

            UpdateLayout();
        }

        void Update()
        {
        
        }
    }
}
