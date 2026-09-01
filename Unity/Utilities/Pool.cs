using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : Component
{
    private readonly List<T> disabledObjects = new List<T>();

    public void Take(out T objectTaken)
    {
        if (disabledObjects.Count == 0)
        {
            objectTaken = null;
            return;
        }

        objectTaken = disabledObjects[0];
        disabledObjects.RemoveAt(0);
        objectTaken.gameObject.SetActive(true);
    }

    public void MoveIn(in T obj)
    {
        if (disabledObjects.Contains(obj) || obj == null) return;

        obj.gameObject.SetActive(false);
        disabledObjects.Add(obj);
    }

    public void Remove(T obj)
    {
        if (disabledObjects.Contains(obj)) disabledObjects.Remove(obj);
    }
}
