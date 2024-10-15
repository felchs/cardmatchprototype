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
        _A,
        _B,
        _C,
        _D,
        _E,
        _F,
        _G,
        _H,
        _I,
        _J,
        _K,
        _L,
        _M,
        _N,
        _O,
        _P,
        _Q,
        _R,
        _S,
        _T,
        _U,
        _V,
        _X,
        _W,
        _Y,
        _Z
    }

    public enum CardNumber
    {
        _0,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
    }

    public enum CardNameMario
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

        private SpriteAtlas atlas;

        [SerializeField]
        SpriteAtlas atlasMario;

        [SerializeField]
        SpriteAtlas atlasPortuguese;

        [SerializeField]
        public string spriteName;

        private float rotateSpeed = 180;

        private bool onFlippingCard;
        public void ForceFlipCard()
        {
            onFlippingCard = true;            
        }

        private float yRotation = 0;

        private bool locked = false;
        public bool Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        private bool canFlipCard = true;
        public bool CanFlipCard
        {
            get { return canFlipCard; }
            set { 
                if (locked)
                {
                    return;
                }

                canFlipCard = value; 
            }
        }

        private string cardId;
        public string CardId
        {
            get { return cardId; }
            set { cardId = value; }
        }

        private bool internalAnimFlip = false;

        public UnityEvent<GameCard> OnFlipCardEvent;

        private bool open = false;

        private GameType gameType;
        public GameType GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }

        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

        public IEnumerator ChangeSprite(string spriteName, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (this == null)
            {
                yield break;
            }

            this.spriteName = spriteName;

           GetComponent<Image>().sprite = atlas.GetSprite(spriteName);
        }

        public IEnumerator ChangeBackSprite(int delay)
        {
            yield return new WaitForSeconds(delay);

            if (this == null)
            {
                yield break;
            }

            GetComponent<Image>().sprite = atlas.GetSprite(BACKSPRITE);
        }

        public void DoEffectSelection()
        {
            Debug.Log("Effect!");
        }

        void Start()
        {
            if (GameType == GameType.MARIO)
            {
                atlas = atlasMario;
            }
            else
            {
                atlas = atlasPortuguese;
            }

            StartCoroutine(ChangeBackSprite(0));
        }

        public void DoFlipCardClickEvent()
        {
            if (!CanFlipCard || onFlippingCard)
            {
                return;
            }

            OnFlipCardEvent?.Invoke(this);
            onFlippingCard = true;
        }

        public IEnumerator ForceReverseFlipCard(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (onFlippingCard)
            {
                internalAnimFlip = !internalAnimFlip;
            }

            OnFlipCardEvent?.Invoke(this);
            onFlippingCard = true;
        }

        private void FlipCardGraphic()
        {
            RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
            Vector3 euler = rectTransform.eulerAngles;
            euler.y = yRotation;

            rectTransform.eulerAngles = euler;

            int sign = internalAnimFlip ? -1 : 1;
            yRotation += rotateSpeed * Time.deltaTime * sign;

            if (yRotation > 90 && !internalAnimFlip)
            {
                // change
                internalAnimFlip = !internalAnimFlip;

                if (open)
                {
                    StartCoroutine(ChangeBackSprite(0));
                }
                else
                {
                    StartCoroutine(ChangeSprite(spriteName, 0));
                }

                open = !open;
            }

            if (yRotation < 0 && internalAnimFlip)
            {
                internalAnimFlip = false;
                yRotation = 0;
                onFlippingCard = false;
            }
        }

        public Vector2 GetSize()
        {
            return ((RectTransform)this.gameObject.transform).sizeDelta;
        }

        void Update()
        {
            if (onFlippingCard)
            {
                FlipCardGraphic();
            }
        }

    }
}
