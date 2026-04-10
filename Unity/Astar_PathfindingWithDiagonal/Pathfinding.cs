using System.Collections.Generic;
using UnityEngine;

namespace Com.ArTiX.FactoryGame
{
    public class Pathfinding
    {
        public static Pathfinding Instance { get; private set; }

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private readonly Vector2Int[] possibleNeighbours =
            {
                Vector2Int.right,
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.up,
                Vector2Int.one,
                -Vector2Int.one,
                Vector2Int.down + Vector2Int.right,
                Vector2Int.up + Vector2Int.left };

        private readonly GridSystem<PathNode> grid;
        private List<PathNode> openList;
        private List<PathNode> closedList;

        static Pathfinding()
        {
            Instance = new Pathfinding();
        }

        public Pathfinding()
        {
            grid = GridSpawner.Instance.CreateGrid<PathNode>();

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    grid.SetGridObject(x, y, new PathNode(grid, x, y));
                }
            }
        }

        public List<PathNode> FindPath(in int pStartX, in int pStartY, in int pEndX, in int pEndY)
        {
            PathNode lCurrentNode;
            for (int lX = 0; lX < grid.width;  lX++)
            {
                for (int lY = 0; lY < grid.height; lY++)
                {
                    lCurrentNode = grid.GetGridObject(lX, lY);
                    lCurrentNode.gCost = int.MaxValue;
                    lCurrentNode.CalculateFCost();
                    lCurrentNode.cameFromNode = null;
                }
            }

            PathNode lStartNode = grid.GetGridObject(pStartX, pStartY);
            PathNode lEndNode = grid.GetGridObject(pEndX, pEndY);
            lStartNode.gCost = 0;
            lStartNode.hCost = CalculateDistance(lStartNode, lEndNode);
            lStartNode.CalculateFCost();

            openList = new() { lStartNode };
            closedList = new();

            while (openList.Count > 0)
            {
                lCurrentNode = GetLowestFCostNode(openList);
                if (lCurrentNode == lEndNode)
                    return CalculatePath(lEndNode);

                openList.Remove(lCurrentNode);
                closedList.Add(lCurrentNode);

                int lTentativeGCost;
                foreach (PathNode lNeighbourNode in GetNeighbours(lCurrentNode))
                {
                    if (!closedList.Contains(lNeighbourNode))
                    {
                        lTentativeGCost = lCurrentNode.gCost + CalculateDistance(lCurrentNode, lNeighbourNode);
                        if (lTentativeGCost < lNeighbourNode.gCost)
                        {
                            lNeighbourNode.cameFromNode = lCurrentNode;
                            lNeighbourNode.gCost = lTentativeGCost;
                            lNeighbourNode.hCost = CalculateDistance(lNeighbourNode, lEndNode);
                            lNeighbourNode.CalculateFCost();

                            if (!openList.Contains(lNeighbourNode))
                                openList.Add(lNeighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        private List<PathNode> GetNeighbours(in PathNode pNode)
        {
            List<PathNode> lNeighbours = new List<PathNode>();

            foreach (Vector2Int lPossibleNeighbour in possibleNeighbours)
            {
                if (grid.IsCellWithinGrid(lPossibleNeighbour.x + pNode.x, lPossibleNeighbour.y + pNode.y))
                    lNeighbours.Add(grid.GetGridObject(lPossibleNeighbour.x + pNode.x, lPossibleNeighbour.y + pNode.y));
            }

            return lNeighbours;
        }

        private List<PathNode> CalculatePath(in PathNode pEndNode)
        {
            List<PathNode> lPath = new List<PathNode> { pEndNode };
            PathNode lCurrentNode = pEndNode;
            while (lCurrentNode.cameFromNode != null)
            {
                lPath.Add(lCurrentNode.cameFromNode);
                lCurrentNode = lCurrentNode.cameFromNode;
            }

            lPath.Reverse();
            return lPath;
        }

        private int CalculateDistance(in PathNode pNodeA, in PathNode pNodeB)
        {
            int lXDistance = Mathf.Abs(pNodeA.x - pNodeB.x);
            int lYDistance = Mathf.Abs(pNodeA.y - pNodeB.y);
            int lRemaining = Mathf.Abs(lXDistance - lYDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(lXDistance, lYDistance) + MOVE_STRAIGHT_COST * lRemaining;
        }

        private PathNode GetLowestFCostNode(in List<PathNode> pPathNodes)
        {
            PathNode lLowestFCostNode = pPathNodes[0];
            foreach (PathNode lNode in pPathNodes)
            {
                if (lNode.fCost < lLowestFCostNode.fCost)
                    lLowestFCostNode = lNode;
            }
            return lLowestFCostNode;
        }
    }
}