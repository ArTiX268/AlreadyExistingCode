using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private static readonly List<Timer> timers = new List<Timer>();

    public static Timer CreateTimer(float duration, bool fixedTimer = false, bool scaled = true, int repetitionNumber = 0, Action finishedEvent = null)
    {
        Timer timer = new Timer(duration, fixedTimer, scaled, repetitionNumber, finishedEvent);
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