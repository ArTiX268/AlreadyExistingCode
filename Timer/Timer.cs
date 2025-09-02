using System;
using UnityEngine;

public class Timer
{
    private readonly float duration;
    private readonly bool fixedTimer;
    private readonly bool scaled;
    private readonly int repetitionNumber;

    private bool isActive;
    private float timer;
    private int currentRepetitionNumber;

    public event Action FinishedEvent;

    public Timer(float duration, bool fixedTimer = false, bool scaled = true, int repetitionNumber = 0, Action finishedEvent = null)
    {
        this.duration = duration;
        this.fixedTimer = fixedTimer;
        this.scaled = scaled;
        this.repetitionNumber = currentRepetitionNumber = repetitionNumber;

        FinishedEvent += finishedEvent;
    }

    private float GetIncrementation() => fixedTimer ?
        (scaled ? Time.fixedDeltaTime : Time.fixedUnscaledDeltaTime) :
        (scaled ? Time.deltaTime : Time.unscaledDeltaTime);

    public void IncrementTimer()
    {
        if (isActive && currentRepetitionNumber >= 0)
        {
            timer += GetIncrementation();

            if (timer > duration)
            {
                FinishTimer();
            }
        }
    }

    private void FinishTimer()
    {
        currentRepetitionNumber--;

        if (currentRepetitionNumber < 0)
            isActive = false;

        timer = 0;
        FinishedEvent?.Invoke();
    }

    public void StartTimer() => isActive = true;

    public void PauseTimer() => isActive = false;

    public void StopTimer()
    {
        currentRepetitionNumber = repetitionNumber;
        isActive = false;
        timer = 0;
    }

    public bool IsActive() => isActive;
}