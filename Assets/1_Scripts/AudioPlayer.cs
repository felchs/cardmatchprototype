using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardMatch
{
    public enum AudioEventEnum
    {
        FLIP_CARD
    }

    public abstract class AudioPlayer : MonoBehaviour
    {
        public abstract void PlayAudio(AudioEventEnum audioEvent, float delayInSeconds);

        public abstract void PlayAudio(AudioEventEnum audioEvent, GameObject source, float delayInSeconds);

    }

}
