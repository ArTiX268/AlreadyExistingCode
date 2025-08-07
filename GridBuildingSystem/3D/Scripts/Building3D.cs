using UnityEngine;

[CreateAssetMenu()]
public class Building3D : ScriptableObject
{
    public Vector2Int[] occupiedCells;

    public Transform prefab;
    public GameObject visual;

    public Vector2Int[] GetGridPositionList(Vector2Int offset, Direction direction)
    {
        Vector2Int[] gridPositionList = new Vector2Int[occupiedCells.Length];

        for (int i = 0; i < gridPositionList.Length; i++)
        {
            switch (direction)
            {
                case Direction.Down:
                    gridPositionList[i] = new Vector2Int(occupiedCells[i].x, occupiedCells[i].y) + offset;
                    break;
                case Direction.Right:
                    gridPositionList[i] = new Vector2Int(-occupiedCells[i].y, occupiedCells[i].x) + offset;
                    break;
                case Direction.Up:
                    gridPositionList[i] = new Vector2Int(-occupiedCells[i].x, -occupiedCells[i].y) + offset;
                    break;
                case Direction.Left:
                    gridPositionList[i] = new Vector2Int(occupiedCells[i].y, -occupiedCells[i].x) + offset;
                    break;
            }
        }

        return gridPositionList;
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