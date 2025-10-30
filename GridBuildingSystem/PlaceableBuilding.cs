using UnityEngine;

namespace Com.ArTiX.FactoryGame
{
    public abstract class PlaceableBuilding : MonoBehaviour
    {
        [SerializeField] protected BuildingSO buildingData;

        protected Vector2Int[] occupiedCells;

        /// <summary>
        /// </summary>
        /// <param name="pX">The x of the cell on which the player clicked and that represents the middle of the building.</param>
        /// <param name="pY">The y of the cell on which the player clicked and that represents the middle of the building.</param>
        public void SetOccupiedCells(in int pX, in int pY, in EBuildingRotation pRotation)
            => SetOccupiedCells(new Vector2Int(pX, pY), pRotation);

        public void SetOccupiedCells(in Vector2Int pSpawningCell, in EBuildingRotation pRotation)
        {
            Vector2Int[] lCells = buildingData.GetCells(pRotation);
            int lNbCell = lCells.Length;
            occupiedCells = new Vector2Int[lNbCell];

            for (byte i = 0; i < lNbCell; i++)
                occupiedCells[i] = pSpawningCell + lCells[i];
        }

        public Vector2Int[] GetOccupiedCells() => occupiedCells;
    }
}