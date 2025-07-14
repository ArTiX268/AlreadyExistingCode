using UnityEngine;

namespace ArTiX
{
    public class Utilities : MonoBehaviour
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

        public static Vector2 GetMousePosition2D(Camera camera)
        {
            Vector2 mousePos = Input.mousePosition;
            return camera.ScreenToWorldPoint(mousePos);
        }

        public static Vector2 GetMousePosition2D()
        {
            Vector2 mousePos = Input.mousePosition;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        public static Vector3 GetMousePosition3D(Camera camera)
        {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 1000);
            return hit.point;
        }

        public static Vector3 GetMousePosition3D()
        {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 1000);
            return hit.point;
        }

        public static Vector2 GetRandomPointWithinSquare(float width, float height, Vector2 origin)
        {
            width *= .5f;
            height *= .5f;
            float xValue = Random.Range(-width, width);
            float yValue = Random.Range(-height, height);
            Vector2 randomPoint = new(xValue, yValue);
            return randomPoint + origin;
        }

        public static Vector2 GetRandomPointWithinBoxCollider2D(BoxCollider2D collider2D)
        {
            Vector3 objectScale = GetTrueScaleOfObject(collider2D.transform);

            return GetRandomPointWithinSquare(
                collider2D.size.x * objectScale.x,
                collider2D.size.y * objectScale.y,
                collider2D.transform.position);
        }

        public static Vector3 GetTrueScaleOfObject(Transform transform)
        {
            Vector3 trueScale = Vector3.one;

            while (transform != null)
            {
                trueScale.x = trueScale.x * transform.localScale.x;
                trueScale.y = trueScale.y * transform.localScale.y;
                trueScale.z = trueScale.z * transform.localScale.z;

                transform = transform.parent;
            }

            return trueScale;
        }
    }
}