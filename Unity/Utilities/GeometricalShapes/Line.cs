using ArTiX.Utils;
using UnityEngine;

public struct Line
{
    private const float DISTANCE_THRESHOLD = 0.01f;

    public Vector2 DirectionalVector { get; private set; }
    public readonly float directionalCoef;
    
    private readonly Vector2 point;

    public Line(Vector2 point, Vector2 directionalVector)
    {
        if (directionalVector == Vector2.zero)
        {
            Debug.LogError("DirectionalVector is equal to 0, assigning Vector (1, 1) as default vector.");
            directionalVector = Vector2.one;
        }

        DirectionalVector = directionalVector.normalized;
        this.point = point;
        directionalCoef = directionalVector.y / directionalVector.x;
    }

    public bool IsPointOnLine(in Vector2 p)
    {
        // True if 
        // p.x = point.x + DirectionalVector.x * k
        // AND
        // p.y = point.y + DirectionalVector.y * k

        float kx = (p.x - point.x) / DirectionalVector.x;
        float ky = (p.y - point.y) / DirectionalVector.y;
        return kx - ky <= DISTANCE_THRESHOLD;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="line"></param>
    /// <param name="intersectionPoint"></param>
    /// <returns>False if parrallel</returns>
    public readonly bool GetIntersectionPoint(in Line line, out Vector2 intersectionPoint)
    {
        if (line.DirectionalVector.Colinear(DirectionalVector))
        {
            intersectionPoint = Vector2.zero;
            return false;
        }

        if (directionalCoef == float.PositiveInfinity || directionalCoef == float.NegativeInfinity)
        {
            intersectionPoint = new Vector2
            {
                x = point.x,
                y = (line.directionalCoef * (point.x - line.point.x)) + line.point.y
            };
            return true;
        }
        else if (line.directionalCoef == float.PositiveInfinity || line.directionalCoef == float.NegativeInfinity)
        {
            intersectionPoint = new Vector2
            {
                x = line.point.x,
                y = (directionalCoef * (line.point.x - point.x)) + point.y
            };
            return true;
        }

        if (point.y - line.point.y == 0) 
        {
            intersectionPoint = Vector2.zero;
            return false; 
        }

        intersectionPoint.x = ((point.y - line.point.y) / (line.directionalCoef - directionalCoef)) + point.x;
        intersectionPoint.y = (line.directionalCoef * intersectionPoint.x) + line.point.y;
        return true;
    }
}
