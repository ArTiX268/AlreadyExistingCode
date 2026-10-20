using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Buildings
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class PlacedObject : MonoBehaviour
    {
        [SerializeField] protected PlacedObjectSO datas;
        public PlacedObjectSO Datas => datas;

        public Vector2Int Origin { get; private set; }

        protected Vector2Int[] AdjacentCellArray
        {
            get
            {
                return new Vector2Int[]
                {
                    Origin + Vector2Int.right,
                    Origin + Vector2Int.up,
                    Origin + Vector2Int.left,
                    Origin + Vector2Int.down
                };
            }
        }

        public virtual void Initialize(Vector2Int origin) => Origin = origin;

        protected virtual void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            name = datas.Name;
        }

        public List<Vector2Int> GetOccupiedCells()
        {
            List<Vector2Int> occupiedCells = new List<Vector2Int>();

            foreach (Vector2Int cell in datas.CellArray)
            {
                occupiedCells.Add(Origin + cell.Rotate(transform.eulerAngles.y));
            }

            return occupiedCells;
        }

        protected bool IsAdjacent(Vector2Int cell)
        {
            return Array.IndexOf(AdjacentCellArray, cell) != -1;
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }
    }
}