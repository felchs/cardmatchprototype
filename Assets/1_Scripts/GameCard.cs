using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CardMatch
{
    public enum CardName
    {
        _0_Mario,
        _1_Luigi,
        _2_Toad,
        _3_Donkey,
        _4_Peach,
        _5_Daisy,
        _6_Didi,
        _7_MarioFox,
        _8_Toadette,
        _9_MarioKart,
        _10_Yoshi,
        _11_Turtle,
        _12_Ghost2,
        _13_Walouigi,
        _14_BabyMario,
        _15_Carnivo,
        _16_Bowser,
        _17_Wario,
        _18_Turtle2,
        _19_BigBee,
        _20_MarioOne,
        _21_FlyTurtle,
        _22_Skelleton,
        _23_BabyPrincess,
        _24_Cento,
        _25_SkelletonTurtle,
        _26_BabyDaisy,
        _27_BabyPeach,
        _28_BabyLuigi,
        _29_PinkDino,
        _30_Yoshi3,
        _31_MiniBowser,
        _32_FlyingTurtle,
        _33_Princess,
        _34_FunkyKong,
        _35_Ghost,
        _36_LuigiKart
    }

    public class GameCard : MonoBehaviour
    {
        private const string BACKSPRITE = "_Back";

        [SerializeField]
        SpriteAtlas atlas;

        [SerializeField]
        string spriteName;

        private float rotateSpeed = 180;

        private bool onFlippingCard;
        private float yRotation = 0;

        private bool canFlipCard = true;

        private string cardId;

        public string CardId
        {
            get { return cardId; }
            set { cardId = value; }
        }

        public bool CanFlipCard
        {
            get { return canFlipCard; }
            set { canFlipCard = value; }
        }

        private bool internalAnimFlip = false;

        public UnityEvent<GameCard> OnFlipCardEvent;

        private bool open = false;

        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

        public void ChangeSprite(string spriteName)
        {
            this.spriteName = spriteName;
            GetComponent<Image>().sprite = atlas.GetSprite(spriteName);
        }

        public void ChangeOriginalSprite()
        {
            GetComponent<Image>().sprite = atlas.GetSprite(spriteName);
        }

        public void ChangeBackSprite()
        {
            GetComponent<Image>().sprite = atlas.GetSprite(BACKSPRITE);
        }

        public void DoEffectSelection()
        {
            Debug.Log("Effect!");
        }

        void Start()
        {
            ChangeBackSprite();
            //ChangeSprite(spriteName);
        }

        public void DoFlipCard()
        {
            if (!canFlipCard || onFlippingCard)
            {
                return;
            }

            onFlippingCard = true;
        }

        private void FlipCard()
        {
            RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
            Vector3 euler = rectTransform.eulerAngles;
            euler.y = yRotation;

            rectTransform.eulerAngles = euler;

            int sign = internalAnimFlip ? -1 : 1;
            yRotation += rotateSpeed * Time.deltaTime * sign;

            if (yRotation > 90)
            {
                // change
                internalAnimFlip = !internalAnimFlip;

                if (open)
                {
                    ChangeBackSprite();
                }
                else
                {
                    ChangeSprite(spriteName);
                }

                OnFlipCardEvent?.Invoke(this);

                open = !open;
            }

            if (yRotation < 0)
            {
                internalAnimFlip = false;
                yRotation = 0;
                onFlippingCard = false;
            }
        }

        void Update()
        {
            if (onFlippingCard)
            {
                FlipCard();
            }
        }

    }
}
