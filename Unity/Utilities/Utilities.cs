using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ArTiX.Utils
{
    #region MyClasses

    public class Utilities
    {
        public const float FLOAT_THRESHOLD = 0.0001f;

        // COMMON MATERIAL PROPERTIES
        public const string RADIUS = "_Radius";
        public const string INTENSITY = "_Intensity";
        public const string FILL_AMOUNT = "_FillAmount";
        public const string HIT_AMOUNT = "_HitAmount";
        public const string CENTER = "_Center";
        public const string COLOR = "_Color";

        public static Vector2 GetCenterOfTheScreen()
        {
            float x_Center = Screen.width / 2;
            float y_Center = Screen.height / 2;

            return new(x_Center, y_Center);
        }

        public static Vector2 VectorFromAngle(in float angle, float magnitude = 1)
            => new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;

        public static Vector3 ConvertFromWorldPointToNormalizedScreenPoint(Vector3 worldPoint)
        {
            worldPoint = Camera.main.WorldToScreenPoint(worldPoint);
            worldPoint.x /= Screen.width;
            worldPoint.y /= Screen.height;

            return worldPoint;
        }

        public static TextMeshPro SpawnText(in string text, in Vector3 position, in Quaternion rotation, in float fontSize)
        {
            TextMeshPro textObj = new GameObject("Text", typeof(TextMeshPro)).GetComponent<TextMeshPro>();
            textObj.transform.SetPositionAndRotation(position, rotation);
            textObj.text = text;
            textObj.fontSize = fontSize;
            textObj.alignment = TextAlignmentOptions.Center;

            return textObj;
        }
    }

    public static class MyDebug
    {
        public static void DrawSquareXY(Vector3 squareMiddleWorldPos, float size, Color color, float duration)
        {
            CalculateCornerPositionsXY(out Vector3[,] corners, size, squareMiddleWorldPos.x, squareMiddleWorldPos.y);

            DrawSquareLines(corners, color, duration);
        }

        private static void CalculateCornerPositionsXY(out Vector3[,] corners, float size, float width, float height)
        {
            float halfSize = size * 0.5f;
            corners = new Vector3[2, 2];
            corners[0, 0] = new Vector2(width - halfSize, height - halfSize);
            corners[1, 0] = new Vector2(width + halfSize, height - halfSize);
            corners[0, 1] = new Vector2(width - halfSize, height + halfSize);
            corners[1, 1] = new Vector2(width + halfSize, height + halfSize);
        }

        public static void DrawSquareXZ(Vector3 squareMiddleWorldPos, float size, Color color, float duration)
        {
            CalculateCornerPositionsXZ(out Vector3[,] corners, size, squareMiddleWorldPos.x, squareMiddleWorldPos.z);

            DrawSquareLines(corners, color, duration);
        }

        private static void CalculateCornerPositionsXZ(out Vector3[,] corners, float size, float width, float height)
        {
            float halfSize = size * 0.5f;
            corners = new Vector3[2, 2];
            corners[0, 0] = new Vector3(width - halfSize, 0, height - halfSize);
            corners[1, 0] = new Vector3(width + halfSize, 0, height - halfSize);
            corners[0, 1] = new Vector3(width - halfSize, 0, height + halfSize);
            corners[1, 1] = new Vector3(width + halfSize, 0, height + halfSize);
        }

        private static void DrawSquareLines(Vector3[,] corners, Color color, float duration)
        {
            Debug.DrawLine(corners[0, 0], corners[1, 0], color, duration);
            Debug.DrawLine(corners[0, 0], corners[0, 1], color, duration);
            Debug.DrawLine(corners[1, 0], corners[1, 1], color, duration);
            Debug.DrawLine(corners[0, 1], corners[1, 1], color, duration);
        }

        public static void DrawCrossXY(Vector3 worldPosition, Color color, float duration, float size = 1)
        {
            float halfedSize = size * .5f;
            Debug.DrawLine(new Vector3(worldPosition.x - halfedSize, worldPosition.y), new Vector3(worldPosition.x + halfedSize, worldPosition.y), color, duration);
            Debug.DrawLine(new Vector3(worldPosition.x, worldPosition.y - halfedSize), new Vector3(worldPosition.x, worldPosition.y + halfedSize), color, duration);
        }

        public static void DrawCrossXZ(Vector3 worldPosition, Color color, float duration, float size = 1)
        {
            float halfedSize = size * .5f;
            Debug.DrawLine(new Vector3(worldPosition.x - halfedSize, 0, worldPosition.z), new Vector3(worldPosition.x + halfedSize, 0, worldPosition.z), color, duration);
            Debug.DrawLine(new Vector3(worldPosition.x, 0, worldPosition.z - halfedSize), new Vector3(worldPosition.x, 0, worldPosition.z + halfedSize), color, duration);
        }

        public static void DrawCrossYZ(Vector3 worldPosition, Color color, float duration, float size = 1)
        {
            float halfedSize = size * .5f;
            Debug.DrawLine(new Vector3(0, worldPosition.y - halfedSize, worldPosition.z), new Vector3(0, worldPosition.y + halfedSize, worldPosition.z), color, duration);
            Debug.DrawLine(new Vector3(0, worldPosition.y, worldPosition.z - halfedSize), new Vector3(0, worldPosition.y, worldPosition.z + halfedSize), color, duration);
        }
    }

    public static class MyRandom
    {
        public static Vector2 GetRandomPointWithinSquare(float width, float height, Vector2 origin)
        {
            width *= .5f;
            height *= .5f;
            float xValue = UnityEngine.Random.Range(-width, width);
            float yValue = UnityEngine.Random.Range(-height, height);
            Vector2 randomPoint = new(xValue, yValue);
            return randomPoint + origin;
        }

        public static Vector3 GetRandomPointWithinCube(float length, float width, float height, Vector3 origin)
        {
            length *= .5f;
            width *= .5f;
            height *= .5f;
            float xValue = UnityEngine.Random.Range(-length, length);
            float yValue = UnityEngine.Random.Range(-height, height);
            float zValue = UnityEngine.Random.Range(-width, width);

            Vector3 randomPoint = new(xValue, yValue, zValue);
            return randomPoint + origin;
        }

        public static Vector3 GetRandomPointWithinCircle(in float radius, in Vector3 normal, in Vector3 center)
        {
            Vector2 randomPointInUnitCircle = UnityEngine.Random.insideUnitCircle * radius;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
            return center + (rotation * randomPointInUnitCircle);
        }

        /// <summary>
        /// Randomize a vector by modifying it by adding a small offset to it.
        /// </summary>
        /// <param name="maxNoiseStrength">Value which indicates how minimal the modification can be.</param>
        /// <param name="minNoiseStrength">Value which indicates how maximum the modification can be.</param>
        /// <param name="vector">The vector to randomize.</param>
        /// <returns></returns>
        public static void AddNoiseToVector(in float minNoiseStrength, in float maxNoiseStrength, ref Vector3 vector)
        {
            Vector3 centerOfTheCircle = Vector3.zero;
            float randomRadius = UnityEngine.Random.Range(minNoiseStrength, maxNoiseStrength);
            Vector3 randomPointWithinCircle = GetRandomPointWithinCircle(randomRadius, -vector, centerOfTheCircle);
            vector += randomPointWithinCircle;
        }

        /// <summary>
        /// Randomize a vector by modifying it by adding a small offset to it.
        /// </summary>
        /// <param name="modificationStrength">Value which indicates the angle to add to the vector.</param>
        /// <param name="vector">The vector to randomize.</param>
        public static void AddNoiseToVector(in float modificationStrength, ref Vector3 vector)
        {
            Vector3 centerOfTheCircle = Vector3.zero;
            float radius = modificationStrength;
            Vector3 randomPointWithinCircle = GetRandomPointWithinCircle(radius, -vector, centerOfTheCircle);
            vector += randomPointWithinCircle;
        }
    }

    public static class MyCursor
    {
        public static Vector2 GetMousePosition2D(Camera camera)
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            return camera.ScreenToWorldPoint(mousePos);
        }

        public static Vector2 GetMousePosition2D() => GetMousePosition2D(Camera.main);

        public static Vector3 GetMousePosition3D(Camera camera)
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 100000);
            return hit.point;
        }

        public static Vector3 GetMousePosition3D() => GetMousePosition3D(Camera.main);
    }

    public static class Math
    {
        public static float NormalizedSin(float x) => (Mathf.Sin(x) + 1) * 0.5f;
    }

    #endregion

    #region Class Extension

    public static class CollectionsExtension
    {
        public static bool Contains<T>(this T[] array, T element) => Array.IndexOf(array, element) >= 0;

        public static bool TryGetFirstEmptyElement<T>(this T[] array, out int firstEmptyIndex)
        {
            firstEmptyIndex = Array.IndexOf(array, null);

            return firstEmptyIndex >= 0;
        }

        public static List<T> ConvertToList<T>(this T[] array)
        {
            List<T> list = new List<T>();

            foreach (T element in array) list.Add(element);

            return list;
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list.Count == 0) return default;

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }

    public static class TransformExtension
    {
        public static Vector3 GetTrueScale(this Transform transform)
        {
            Vector3 trueScale = Vector3.one;

            void UpdateTrueScale(Transform _transform)
            {
                trueScale.x *= _transform.localScale.x;
                trueScale.y *= _transform.localScale.y;
                trueScale.z *= _transform.localScale.z;
            }

            UpdateTrueScale(transform);

            Transform parentTransform = transform.parent;

            while (parentTransform != null)
            {
                UpdateTrueScale(parentTransform);
                parentTransform = parentTransform.parent;
            }

            return trueScale;
        }

        public static Vector3 SelfToTransformVector(this Transform transform, Transform other, bool normalized)
        {
            Vector3 vector = other.position - transform.position;
            return normalized ? vector.normalized : vector;
        }

        public static Vector3 TransformToSelfVector(this Transform transform, Transform other, bool normalized)
        {
            Vector3 vector = transform.position - other.position;
            return normalized ? vector.normalized : vector;
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                MonoBehaviour.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public static class SpriteExtension
    {
        public static Vector2 GetRealWorldSize(this Sprite sprite)
        {
            return new Vector2(sprite.texture.width, sprite.texture.height) / sprite.pixelsPerUnit;
        }

        public static Rect GetSpriteRect(this SpriteRenderer renderer)
        {
            Vector2 enemySize = renderer.sprite.GetRealWorldSize();
            return new Rect(renderer.transform.position.AddVector2(-enemySize * .5f),
                enemySize);
        }
    }

    public static class VectorExtension
    {
        public static Vector3 PutVectorOnXZPlane(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

        public static Vector3 PutVectorOnZYPlane(this Vector3 vector) => new Vector3(0, vector.x, vector.z);

        public static Vector3 AddVector2(this Vector3 vector, Vector2 v)
        {
            vector.x += v.x;
            vector.y += v.y;
            return vector;
        }

        /// <summary>
        /// The functions directly applies on the vector it's called on.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle">Must be in radians.</param>
        /// <returns></returns>
        public static Vector2 RotateVector(this Vector2 vector, float angle)
        {
            vector.x = (vector.x * Mathf.Cos(angle)) - (vector.y * Mathf.Sin(angle));
            vector.y = (vector.x * Mathf.Sin(angle)) + (vector.y * Mathf.Cos(angle));
            return vector;
        }

        public static Vector3 Abs(this Vector3 vector)
        {
            Vector3 newVector = vector;
            newVector.x = Mathf.Abs(vector.x);
            newVector.y = Mathf.Abs(vector.y);
            newVector.z = Mathf.Abs(vector.z);
            return newVector;
        }
        public static Vector2 Abs(this Vector2 vector)
        {
            Vector3 newVector = vector;
            newVector.x = Mathf.Abs(vector.x);
            newVector.y = Mathf.Abs(vector.y);
            return newVector;
        }
        public static Vector2Int Abs(this Vector2Int vec)
        {
            return new Vector2Int(
                x: Mathf.Abs(vec.x),
                y: Mathf.Abs(vec.y));
        }

        public static bool Colinear(this Vector3 self, in Vector3 vector)
        {
            return Mathf.Abs(Vector3.Dot(self.normalized, vector.normalized)) == 1;
        }
        public static bool Colinear(this Vector2 self, in Vector2 vector)
        {
            return Mathf.Abs(Vector2.Dot(self.normalized, vector.normalized)) == 1;
        }

        public static Vector3 GetClosestPoint(this Vector3 origin, params Vector3[] points)
        {
            Vector3 closestPoint = Vector3.one * float.MaxValue;
            float closestDistance = float.MaxValue;
            foreach(Vector3 point in points)
            {
                if (Vector3.Distance(origin, point) < closestDistance)
                {
                    closestDistance = Vector3.Distance(point, origin);
                    closestPoint = point;
                }
            }

            return closestPoint;
        }
        public static Vector2 GetClosestPoint(this Vector3 origin, params Vector2[] points)
        {
            Vector2 closestPoint = Vector2.one * float.MaxValue;
            float closestDistance = float.MaxValue;
            foreach (Vector2 point in points)
            {
                if (Vector2.Distance(origin, point) < closestDistance)
                {
                    closestDistance = Vector3.Distance(point, origin);
                    closestPoint = point;
                }
            }

            return closestPoint;
        }
        public static Vector2 GetClosestPoint(this Vector2 origin, params Vector2[] points)
        {
            Vector2 closestPoint = Vector2.one * float.MaxValue;
            float closestDistance = float.MaxValue;
            foreach (Vector2 point in points)
            {
                if (Vector2.Distance(origin, point) < closestDistance)
                {
                    closestDistance = Vector3.Distance(point, origin);
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        public static bool IsBetweenPoints(this Vector3 self, in Vector3 a, in Vector3 b)
        {
            float aDot = Vector3.Dot(a - self, a - b);
            float bDot = Vector3.Dot(b - self, b - a);
            return aDot >= 0 && bDot >= 0;
        }
        public static bool IsBetweenPoints(this Vector2 self, in Vector2 a, in Vector2 b)
        {
            float aDot = Vector2.Dot(a - self, a - b);
            float bDot = Vector2.Dot(b - self, b - a);
            return aDot >= 0 && bDot >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A vector normal to this vector.</returns>
        public static Vector2 Normal(this Vector2 vec) => new Vector2(-vec.y, vec.x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="angle">In degrees.</param>
        /// <returns></returns>
        public static Vector2Int Rotate(this Vector2Int vec, float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector2Int(
                x: Mathf.RoundToInt((vec.x * Mathf.Cos(angle)) - (vec.y * Mathf.Sin(angle))),
                y: Mathf.RoundToInt((vec.x * Mathf.Sin(angle)) + (vec.y * Mathf.Cos(angle)))
            );
        }
    }

    public static class CameraExtension
    {
        public static Vector3 GetPointInFrontOfCamera(this Camera camera, float distance = 1)
        {
            Vector3 cameraPosition = camera.transform.position;
            Vector3 cameraForward = camera.transform.forward * distance;
            return cameraPosition + cameraForward;
        }
    }

    public static class BoxCollider2DExtension
    {
        public static Vector2 GetRandomPointWithinBoxCollider2D(this BoxCollider2D collider2D)
        {
            Vector3 objectScale = collider2D.transform.GetTrueScale();

            return MyRandom.GetRandomPointWithinSquare(
                collider2D.size.x * objectScale.x,
                collider2D.size.y * objectScale.y,
                collider2D.transform.position);
        }
    }

    public static class BoxColliderExtension
    {
        public static Vector3 GetRandomPointWithinBoxCollider(this BoxCollider collider)
        {
            Vector3 objectScale = collider.transform.GetTrueScale();

            return MyRandom.GetRandomPointWithinCube(
                collider.size.x * objectScale.x,
                collider.size.y * objectScale.y,
                collider.size.z * objectScale.z,
                collider.transform.position);
        }
    }

    public static class FloatExtension
    {
        /// <summary>
        /// Change the value of this float by a certain percentage. For exemple, if value equals 5 and percentage equals 0.2,
        /// value will now be equal to a value in the [4, 6] range.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentage">Percentage of modification. Must be in the [0, 1] range.</param>
        /// <returns></returns>
        public static float Randomize(this float value, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            value *= 1 + UnityEngine.Random.Range(-percentage, percentage);
            return value;
        }
    }

    public static class RectExtension
    {
        public static Vector2 GetHalfSize(this Rect rect) => rect.size * 0.5f;

        public static bool IsPointWithin(this Rect rect, Vector2 point)
        {
            Vector2 rectHalfSize = rect.GetHalfSize();

            return Mathf.Abs(point.x - rect.center.x) - Utilities.FLOAT_THRESHOLD <= rectHalfSize.x && 
                   Mathf.Abs(point.y - rect.center.y) - Utilities.FLOAT_THRESHOLD <= rectHalfSize.y;
        }
    }

    #endregion
}