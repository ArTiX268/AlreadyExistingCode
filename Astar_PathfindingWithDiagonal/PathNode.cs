
using UnityEngine;

namespace Com.ArTiX.FactoryGame
{
    public class PathNode
    {
        private GridSystem<PathNode> grid;
        public readonly int x;
        public readonly int y;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;

        public PathNode(GridSystem<PathNode> pGrid, int pX, int pY)
        {
            grid = pGrid;
            x = pX;
            y = pY;
        }

        public void CalculateFCost() => fCost = gCost + hCost;

        public Vector3 GetWorldPosition() => grid.GetCellPosition(x, y);

        public override string ToString()
        {
            return x + ", " + y;
        }
    }
}