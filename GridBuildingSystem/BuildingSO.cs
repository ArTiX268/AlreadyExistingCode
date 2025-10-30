using Sirenix.OdinInspector;
using System;
using UnityEngine;


namespace Com.ArTiX.FactoryGame
{
    [Serializable]
    public enum EBuildingRotation
    {
        Forward,
        Right,
        Backward,
        Left
    }

    [CreateAssetMenu(fileName = "BuildingSO", menuName = "Scriptable Objects/BuildingSO")]
    public class BuildingSO : ScriptableObject
    {
        public PlaceableBuilding prfb_Building;
        public Transform prfb_BuildingVisual;
        [AssetsOnly] public Sprite resourceIcon;

        [SerializeField] private Vector2Int[] cells;

        public Vector2Int[] GetCells(in EBuildingRotation pRotation)
        {
            if (pRotation == EBuildingRotation.Forward) return cells;

            int lNbCells = cells.Length;
            Vector2Int[] lCells = new Vector2Int[lNbCells];

            switch (pRotation)
            {
                case EBuildingRotation.Right:
                    for (int i = 0; i < lNbCells; i++)
                        lCells[i] = new Vector2Int(cells[i].y, -cells[i].x);
                    break;

                case EBuildingRotation.Backward:
                    for (int i = 0; i < lNbCells; i++)
                        lCells[i] = new Vector2Int(-cells[i].x, -cells[i].y);
                    break;

                case EBuildingRotation.Left:
                    for (int i = 0; i < lNbCells; i++)
                        lCells[i] = new Vector2Int(-cells[i].y, cells[i].x);
                    break;
            }

            return lCells;
        }
    }
}