using System;
using UnityEngine;

public class Timer
{
    private readonly float duration;
    private readonly bool fixedTimer;
    private readonly bool scaled;
    private readonly uint repetitionNumber = 0;

    private bool isActive;
    private float timer;
    private uint currentRepetitionNumber = 0;

    public event Action FinishedEvent;

    public Timer(float duration, bool fixedTimer = false, bool scaled = true, uint repetitionNumber = 0, Action finishedEvent = null)
    {
        this.duration = duration;
        this.fixedTimer = fixedTimer;
        this.scaled = scaled;
        this.repetitionNumber = repetitionNumber;
        this.currentRepetitionNumber = repetitionNumber;

        FinishedEvent += finishedEvent;
    }

    private float GetIncrementation() => fixedTimer ?
        (scaled ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime) :
        (scaled ? Time.deltaTime : Time.unscaledDeltaTime);

    public void IncrementTimer()
    {
        void FinishTimer()
        {
            if (currentRepetitionNumber > 0)
                currentRepetitionNumber--;

            StopTimer();

            timer = 0;
            FinishedEvent?.Invoke();
        }

        if (isActive && currentRepetitionNumber >= 0)
        {
            timer += GetIncrementation();

            if (timer > duration)
            {
                FinishTimer();
            }
        }
    }

    public void StartTimer() => isActive = true;

    public void StartTimerAtTheBeginning()
    {
        StopTimer();
        StartTimer();
    }

    public void PauseTimer() => isActive = false;

    public void StopTimer()
    {
        currentRepetitionNumber = repetitionNumber;
        isActive = false;
        timer = 0;
    }

    public bool IsActive() => isActive;

    public float GetTime() => timer;
}