using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private static readonly List<Timer> timers = new List<Timer>();

    public static Timer CreateTimer(float duration, Action finishedEvent = null, bool fixedTimer = false, bool scaled = true, uint repetitionNumber = 0)
    {
        Timer timer = new(duration, fixedTimer, scaled, repetitionNumber, finishedEvent);
        timers.Add(timer);
        return timer;
    }

    public static void RemoveTimer(Timer timer)
    {
        timers.Remove(timer);
    }

    private void Update()
    {
        foreach (Timer timer in timers)
        {
            timer.IncrementTimer();
        }
    }
}