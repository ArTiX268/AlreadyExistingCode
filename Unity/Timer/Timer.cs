using System;
using UnityEngine;

namespace Utils
{
    public class Timer : MonoBehaviour
    {
        public bool IsActive { get; private set; }
        public float TimerValue { get; private set; }

        private float duration;
        private bool scaled;
        private uint repetitionNumber = 0;

        private uint currentRepetitionNumber = 0;

        public event Action FinishedEvent;

        public static Timer Create(float duration, bool scaled = true, uint repetitionNumber = 0, Action finishedEvent = null, string name = "Timer")
        {
            Timer timer = new GameObject().AddComponent(typeof(Timer)) as Timer;
            timer.gameObject.name = name;

            timer.duration = duration;
            timer.scaled = scaled;
            timer.repetitionNumber = repetitionNumber;
            timer.FinishedEvent = finishedEvent;

            return timer;
        }

        private void Update()
        {
            if (IsActive && currentRepetitionNumber >= 0)
            {
                TimerValue += scaled ? Time.deltaTime : Time.unscaledDeltaTime;

                if (TimerValue > duration)
                {
                    if (currentRepetitionNumber > 0)
                        currentRepetitionNumber--;

                    TimerValue = 0;
                    FinishedEvent?.Invoke();
                }
            }
        }

        public void StartTimer() => IsActive = true;

        public void StartTimerAtTheBeginning()
        {
            StopTimer();
            StartTimer();
        }

        public void PauseTimer() => IsActive = false;

        public void StopTimer()
        {
            currentRepetitionNumber = repetitionNumber;
            IsActive = false;
            TimerValue = 0;
        }
    }
}