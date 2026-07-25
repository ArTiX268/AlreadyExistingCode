using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Utils
{
    public static class Geometry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbPoint">The X component represents the number of points in widht and the Y on the height.</param>
        /// <param name="size"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Vector2[] GetBox(in Vector2Int nbPoint, in Vector2 size, in Vector2 origin)
        {
            Vector2 topLeftCorner = origin + (new Vector2(-size.x, size.y) * 0.5f);
            Vector2 topRightCorner = origin + (size * 0.5f);
            Vector2 bottomLeftCorner = origin - (size * 0.5f);
            Vector2 bottomRightCorner = origin + (new Vector2(size.x, -size.y) * 0.5f);

            Vector2 segment = topRightCorner - topLeftCorner;
            List<Vector2> points = new List<Vector2>();
            int i;
            for (i = 0; i < nbPoint.x; i++)
                points.Add(topLeftCorner + (segment * (i / size.x)));

            segment = bottomRightCorner - topRightCorner;
            for (i = 0; i < nbPoint.x; i++)
                points.Add(topRightCorner + (segment * (i / size.y)));

            segment = bottomLeftCorner - bottomRightCorner;
            for (i = 0; i < nbPoint.x; i++)
                points.Add(bottomRightCorner + (segment * (i / size.x)));

            segment = topLeftCorner - bottomLeftCorner;
            for (i = 0; i < nbPoint.x; i++)
                points.Add(bottomLeftCorner + (segment * (i / size.y)));

            return points.ToArray();
        }

        public static Vector2[] GetCircle(in int nbPoint, in float radius, in Vector2 origin)
        {
            Vector2[] points = new Vector2[nbPoint];
            float currentAngle = 0;
            float deltaAngle = Mathf.PI * 2 / nbPoint;

            for (int i = 0; i < nbPoint; i++)
            {
                points[i] = origin + Utilities.VectorFromAngle(currentAngle, radius);
                currentAngle += deltaAngle;
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nbPoint"></param>
        /// <param name="curvature">The bigger it is, the less it is curvy.</param>
        /// <param name="startAngle">Must be provided in radians.</param>
        /// <param name="arcAngularLength">How much degrees this arc does in radians.</param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static Vector2[] GetArcCircle(in int nbPoint, in float curvature, in float distanceToOrigin, in float startAngle, in float arcAngularLength, in Vector2 origin)
        {
            Vector2[] points = new Vector2[nbPoint];
            float currentAngle = startAngle;
            float deltaAngle = arcAngularLength / nbPoint;
            Vector2 originalVector;
            
            for (int i = 0; i < nbPoint; i++)
            {
                originalVector = Utilities.VectorFromAngle(currentAngle, curvature);
                points[i] = origin + (originalVector.normalized * distanceToOrigin);
                currentAngle += deltaAngle;
            }

            return points;
        }
    
        public static Vector2[] GetIntersectionPointsBetweenLineAndRect(in Rect rect, in Line line)
        {
            Vector2[] points = new Vector2[2];
            int i = 0;

            // Is on bottom segment
            Vector2 corner = rect.center - rect.GetHalfSize();
            Line segment = new Line(corner, Vector2.right);

            if (line.GetIntersectionPoint(segment, out Vector2 intersectionPoint) && 
                rect.IsPointWithin(intersectionPoint))
            {
                points[i] = intersectionPoint;
                i++;
            }

            // Is on left segment
            segment = new Line(corner, Vector2.up);
            if (line.GetIntersectionPoint(segment, out intersectionPoint) &&
                rect.IsPointWithin(intersectionPoint))
            {
                points[i] = intersectionPoint;
                i++;
                if (i == 2) return points;
            }

            // Is on top segment
            corner = rect.center + rect.GetHalfSize();
            segment = new Line(corner, Vector2.left);
            if (line.GetIntersectionPoint(segment, out intersectionPoint) &&
                rect.IsPointWithin(intersectionPoint))
            {
                points[i] = intersectionPoint;
                i++;
                if (i == 2) return points;
            }

            // Is on right segment
            segment = new Line(corner, Vector2.down);
            if (line.GetIntersectionPoint(segment, out intersectionPoint) &&
                rect.IsPointWithin(intersectionPoint))
            {
                points[i] = intersectionPoint;
                i++;
                if (i == 2) return points;
            }

            return points;
        }
    }
}