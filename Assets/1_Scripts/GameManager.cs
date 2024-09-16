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
        GameCard originalCardPrefab;

        [SerializeField]
        SaveState saveState;

        private int numCardsW = 3;

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

        private GameCard CreateNewGameCard(string cardName)
        {
            GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(originalCardPrefab);
            gameObject.name = cardName;

            GameCard gameCard = gameObject.GetComponent<GameCard>();
            gameCard.ChangeSprite(cardName);
            return gameCard;
        }

        public static void Shuffle(Array array)
        {
            System.Random rng = new System.Random();
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                // Randomly pick an index between 0 and i
                int j = rng.Next(0, i + 1);

                // Swap elements array[i] and array[j]
                object temp = array.GetValue(i);
                array.SetValue(array.GetValue(j), i);
                array.SetValue(temp, j);
            }
        }

        private void MountLayout()
        {
            Array cardNamesArray = Enum.GetValues(typeof(CardName));
            Shuffle(cardNamesArray);

            int cardNameIdx = 0;
            for (int i = 0; i < numCardsW; i++)
            {
                for (int j = 0; j < numCardsH; j++)
                {
                    string cardNameToUse = cardNamesArray.GetValue(cardNameIdx++).ToString();
                    GameCard cardA = CreateNewGameCard(cardNameToUse);
                    GameCard cardB = CreateNewGameCard(cardNameToUse);
                    CardPair cardPair = new CardPair(cardA, cardB);
                    cardPairMap.Add(cardNameToUse, cardPair);
                }
            }
        }

        void Start()
        {
            MountLayout();
        }


        void Update()
        {
        
        }
    }
}
