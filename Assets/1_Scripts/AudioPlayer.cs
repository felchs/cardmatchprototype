using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardMatch
{
    public class AudioPlayer : MonoBehaviour
    {
        public List<AudioClip> audioClipList;

        [SerializeField]
        private AudioSource audioSourceMusic;

        [SerializeField]
        private AudioSource audioSourceEffect;

        void Start()
        {
        }

        public void PlayMusic()
        {
            audioSourceMusic.Play();
        }

        public void StopMusic()
        {
            audioSourceMusic.Stop();
        }

        private AudioClip GetAudioClip(string name)
        {
            foreach (AudioClip a in audioClipList)
            {
                if (a.name == name)
                {
                    return a;
                }
            }

            return null;
        }

        public void PlayEffect(string name)
        {
            audioSourceEffect.clip = GetAudioClip(name);
            audioSourceEffect.Play();
        }

        public void StopEffect()
        {
            audioSourceEffect.Stop();
        }
    }
}