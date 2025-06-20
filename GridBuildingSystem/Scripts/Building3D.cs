using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Building3D : ScriptableObject
{
    public int width;
    public int height;

    public Transform prefab;
    public GameObject visual;

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction)
    {
        List<Vector2Int> gridPositionList = new();
        switch (direction)
        {
            case Direction.Up:
            case Direction.Down:
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        gridPositionList.Add(new Vector2Int(x, z) + offset);
                    }
                }
                break;

            case Direction.Left:
            case Direction.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        gridPositionList.Add(new Vector2Int(x, z) + offset);
                    }
                }
                break;
        }

        return gridPositionList;
    }

    public Vector2Int GetDiagonal(Direction direction)
    {
        Vector2Int diagonal = Vector2Int.zero;

        switch (direction)
        {
            case Direction.Up:
            case Direction.Down:
                diagonal = new Vector2Int(width, height);
                break;

            case Direction.Right:
            case Direction.Left:
                diagonal = new Vector2Int(height, width);
                break;
        }
        return diagonal;
    }

    public int GetRotationAngle(Direction dir)
    {
        switch (dir)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }
}