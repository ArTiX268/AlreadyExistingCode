using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "PlacedObjectDatas", menuName = "Datas/PlacedObject")]
    public class PlacedObjectDatas : ScriptableObject
    {
        [SerializeField] private string objName;
        [SerializeField] private Vector2Int[] cells;
        [SerializeField] private Sprite icon;
        [SerializeField] private PlacedObject prefab;
        [SerializeField] private Transform ghost; 

        public string     Name => objName;
        public Sprite     Icon => icon;
        public PlacedObject Prefab => prefab;
        public Transform Ghost => ghost;

        public List<Vector2Int> GetOccupiedCells(in Vector2Int offset, in float rotation)
        {
            List<Vector2Int> occupiedCells = new List<Vector2Int>();

            foreach (Vector2Int cell in cells) 
            {
                occupiedCells.Add(cell.Rotate(rotation) + offset);
            }

            return occupiedCells;
        }
    }
}