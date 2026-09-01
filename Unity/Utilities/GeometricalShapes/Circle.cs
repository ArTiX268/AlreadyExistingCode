using UnityEngine;

public struct Circle
{
    private const float DISTANCE_THRESHOLD = 0.01f;

    public Vector2 Center {  get; private set; }
    public float Radius { get; private set; }

    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool IsPointOnCircle(Vector2 point)
    {
        float d = (point - Center).magnitude;
        return d - Radius <= DISTANCE_THRESHOLD;
    }

    public bool IsPointInsideCircle(Vector2 point)
    {
        float d = (point - Center).magnitude;
        return d - DISTANCE_THRESHOLD <= Radius;
    }
}
