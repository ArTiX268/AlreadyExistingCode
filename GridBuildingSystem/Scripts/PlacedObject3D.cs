using System.Collections.Generic;
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
    [HideInInspector] public List<Vector2Int> gridPositionList;
}