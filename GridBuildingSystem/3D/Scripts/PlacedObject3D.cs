using UnityEngine;

public enum Direction
{
    Down,
    Right,
    Up,
    Left
}

public class PlacedObject3D : MonoBehaviour
{
    [HideInInspector] public Vector2Int[] gridPositionList;
    [HideInInspector] public BuildingType buildingType;
}