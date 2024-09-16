using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

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
        [SerializeField]
        SpriteAtlas atlas;

        [SerializeField]
        string spriteName;

        private bool open;

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

        void Start()
        {
            ChangeSprite(spriteName);
        }


        void Update()
        {
        
        }
    }
}
