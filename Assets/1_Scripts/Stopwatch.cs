using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch
{
    public class Stopwatch : MonoBehaviour
    {
        [SerializeField]
        public TMP_Text timerText;

        private float elapsedTime = 0f;
        private bool isRunning = false;

        void Update()
        {
            if (isRunning)
            {
                elapsedTime += Time.deltaTime;
                DisplayTime(elapsedTime);
            }
        }

        void DisplayTime(float timeToDisplay)
        {
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);

            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }

        public void StopTimer()
        {
            isRunning = false;
        }

        public void StartTimer()
        {
            isRunning = true;
        }

        public void ResetTimer()
        {
            elapsedTime = 0f;
            DisplayTime(elapsedTime);
        }
    }
}