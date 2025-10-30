using ArTiX;
using System;
using UnityEngine;

namespace ArTiX
{
    public class Utilities
    {
        public static Vector2 GetCenterOfTheScreen()
        {
            float x_Center = Screen.width / 2;
            float y_Center = Screen.height / 2;

            return new(x_Center, y_Center);
        }
    }

    public class MyDebug
    {
        public static void DrawSquareXY(Vector3 squareMiddleWorldPos, float size, Color color, float duration)
        {
            CalculateCornerPositionsXY(out Vector3[,] corners, size, squareMiddleWorldPos.x, squareMiddleWorldPos.y);

            DrawSquareLines(corners, color, duration);
        }

        private static void CalculateCornerPositionsXY(out Vector3[,] corners, float size, float width, float height)
        {
            corners = new Vector3[2, 2];
            corners[0, 0] = new Vector2(width, height);
            corners[1, 0] = new Vector2(width + size, height);
            corners[0, 1] = new Vector2(width, height + size);
            corners[1, 1] = new Vector2(width + size, height + size);
        }

        public static void DrawSquareXZ(Vector3 squareMiddleWorldPos, float size, Color color, float duration)
        {
            CalculateCornerPositionsXZ(out Vector3[,] corners, size, squareMiddleWorldPos.x, squareMiddleWorldPos.z);

            DrawSquareLines(corners, color, duration);
        }

        private static void CalculateCornerPositionsXZ(out Vector3[,] corners, float size, float width, float height)
        {
            corners = new Vector3[2, 2];
            corners[0, 0] = new Vector3(width, 0, height);
            corners[1, 0] = new Vector3(width + size, 0, height);
            corners[0, 1] = new Vector3(width, 0, height + size);
            corners[1, 1] = new Vector3(width + size, 0, height + size);
        }

        public static void DrawSquareYZ(Vector3 squareMiddleWorldPos, float size, Color color, float duration)
        {
            CalculateCornerPositionsYZ(out Vector3[,] corners, size, squareMiddleWorldPos.y, squareMiddleWorldPos.z);

            DrawSquareLines(corners, color, duration);
        }

        private static void CalculateCornerPositionsYZ(out Vector3[,] corners, float size, float width, float height)
        {
            corners = new Vector3[2, 2];
            corners[0, 0] = new Vector3(0, width, height);
            corners[1, 0] = new Vector3(0, width + size, height);
            corners[0, 1] = new Vector3(0, width, height + size);
            corners[1, 1] = new Vector3(0, width + size, height + size);
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

    public class MyRandom
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

    public class MyCursor
    {
        public static Vector2 GetMousePosition2D(Camera camera)
        {
            Vector2 mousePos = Input.mousePosition;
            return camera.ScreenToWorldPoint(mousePos);
        }

        public static Vector2 GetMousePosition2D() => GetMousePosition2D(Camera.main);

        public static Vector3 GetMousePosition3D(Camera camera)
        {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 100000);
            return hit.point;
        }

        public static Vector3 GetMousePosition3D() => GetMousePosition3D(Camera.main);
    }
}

public static class ArrayExtension
{
    public static bool Contains<T>(this Array array, T element) => Array.IndexOf(array, element) >= 0;

    public static bool CheckIfContainsIndex(this Array array, uint index) => index < array.Length;

    public static bool TryGetFirstEmptyElement<T>(this Array array, out int firstEmptyIndex) where T : class
    {
        firstEmptyIndex = Array.IndexOf(array, null);

        return firstEmptyIndex >= 0;
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
        Vector3 vector = (other.position - transform.position);
        return normalized ? vector.normalized : vector;
    }

    public static Vector3 TransformToSelfVector(this Transform transform, Transform other, bool normalized)
    {
        Vector3 vector = (transform.position - other.position);
        return normalized ? vector.normalized : vector;
    }
}

public static class Vector3Extension
{
    public static Vector3 PutVectorOnXZPlane(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);

    public static Vector3 PutVectorOnZYPlane(this Vector3 vector) => new Vector3(0, vector.x, vector.z);
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