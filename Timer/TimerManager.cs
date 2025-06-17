using System.Collections.Generic;
using UnityEngine;

public class TimerManager : ScriptableObject
{
    public static List<GameObject> timers = new List<GameObject>();

    public static Timer CreateTimer(string timerName)
    {
        GameObject timer = new GameObject(timerName);
        timers.Add(timer);
        return timer.AddComponent(typeof(Timer)) as Timer;
    }

    public static void DestroyTimer(GameObject timerToDestroy)
    {
        timers.Remove(timerToDestroy);
        Destroy(timerToDestroy);
    }
}
