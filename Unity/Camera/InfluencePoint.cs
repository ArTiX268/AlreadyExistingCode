using ArTiX.Effects;
using System.Collections.Generic;
using UnityEngine;

public class InfluencePoint : MonoBehaviour
{
    public static List<InfluencePoint> instances = new List<InfluencePoint>();

    [SerializeField] private float radius;
    [SerializeField] private float weight;
    [SerializeField] private float zoom;
    [SerializeField] private Tween.ETransition weightTransition;

    private void Awake()
    {
        instances.Add(this);
    }

    public float GetWeight(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        if (distance > radius) return 0;

        return weight * Tween.InterpolateValue(1, 0, radius, distance, weightTransition);
    }

    public float GetZoom() => zoom;

    private void OnDestroy()
    {
        instances.Remove(this);
    }
}
