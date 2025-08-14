using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public enum BuildingType
{
    House,
    Monument,
    WheatFarm,
    PotatoFarm,
    LegumeFarm,
    Extractor,
    Foundry
}

[Serializable]
public struct Effect
{
    public bool buildingNecessary;

    [ShowIf("buildingNecessary")]
    public BuildingType buildingType;

    [Tooltip("Also works when you lose ressources.")]
    public int reward;

    public RessourceType ressourceType;
}

[CreateAssetMenu()]
public class Building3D : ScriptableObject
{
    public int rangeOfEffect;

    public bool mustBeNearAnEdge;

    public Effect[] effects;

    [ShowIf("mustBeNearAnEdge")]
    public Vector2Int[] cellsInTheVoid;
    public Vector2Int[] occupiedCells;

    public NeededRessource[] neededRessources;

    public Transform prefab;

    public GameObject visual;

    public BuildingType buildingType;

    public Vector2Int[] GetGridPositionList(Vector2Int offset, Direction direction)
    {
        Vector2Int[] gridPositionList = new Vector2Int[occupiedCells.Length];

        for (int i = 0; i < gridPositionList.Length; i++)
        {
            switch (direction)
            {
                case Direction.Down:
                    gridPositionList[i] = new Vector2Int(occupiedCells[i].x, occupiedCells[i].y)   + offset;
                    break;
                case Direction.Right:
                    gridPositionList[i] = new Vector2Int(-occupiedCells[i].y, occupiedCells[i].x)  + offset;
                    break;
                case Direction.Up:
                    gridPositionList[i] = new Vector2Int(-occupiedCells[i].x, -occupiedCells[i].y) + offset;
                    break;
                case Direction.Left:
                    gridPositionList[i] = new Vector2Int(occupiedCells[i].y, -occupiedCells[i].x)  + offset;
                    break;
            }
        }

        return gridPositionList;
    }

    public Vector2Int[] GetGridCellsInTheVoidList(Vector2Int offset, Direction direction)
    {
        Vector2Int[] gridPositionList = new Vector2Int[cellsInTheVoid.Length];

        for (int i = 0; i < gridPositionList.Length; i++)
        {
            switch (direction)
            {
                case Direction.Down:
                    gridPositionList[i] = new Vector2Int(cellsInTheVoid[i].x, cellsInTheVoid[i].y)   + offset;
                    break;
                case Direction.Right:
                    gridPositionList[i] = new Vector2Int(-cellsInTheVoid[i].y, cellsInTheVoid[i].x)  + offset;
                    break;
                case Direction.Up:
                    gridPositionList[i] = new Vector2Int(-cellsInTheVoid[i].x, -cellsInTheVoid[i].y) + offset;
                    break;
                case Direction.Left:
                    gridPositionList[i] = new Vector2Int(cellsInTheVoid[i].y, -cellsInTheVoid[i].x)  + offset;
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