using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Utils
{
    public class Pathfinding
    {
        public class PathNode
        {
            public int X {  get; private set; }
            public int Y { get; private set; }

            /// <summary>
            /// Cost from the start node
            /// </summary>
            public int gCost;
            /// <summary>
            /// Cost to the end node
            /// </summary>
            public int HCost { get; private set; }
            public int FCost => gCost + HCost;

            public bool isWalkable = true;

            public PathNode previousNode;
            private Pathfinding pathfinding;

            public PathNode[] Neighbours { get; private set; }

            public PathNode(int x, int y, Pathfinding pathfinding)
            {
                X = x;
                Y = y;
                this.pathfinding = pathfinding;
            }

            public void SetNeighbours(in Vector2Int[] directionArray, in Grid<PathNode> grid)
            {
                List<PathNode> neighbours = new List<PathNode>();
                Vector2Int nodeCell = new Vector2Int(X, Y);
                Vector2Int searchedCell;
                foreach (Vector2Int direction in directionArray)
                {
                    searchedCell = nodeCell + direction;
                    if (grid.IsCellWithinGrid(searchedCell))
                    {
                        neighbours.Add(grid.GetCellValue(searchedCell));
                    }
                }

                Neighbours = neighbours.ToArray();
            }

            public void CalculateHCost(in PathNode endNode)
            {
                HCost = CalculateDistanceCost(this, endNode, pathfinding.AllowDiagonal);
            }

            public void Reset()
            {
                gCost = int.MaxValue;
                previousNode = null;
            }

            public override string ToString()
            {
                return X + "," + Y;
            }
        }

        #region Static

        private static int CalculateDistanceCost(in PathNode a, in PathNode b, in bool allowDiagonal)
        {
            if (a == b) return 0;

            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            if (allowDiagonal)
            {
                int remaining = Mathf.Abs(xDistance - yDistance);
                return (MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance)) + (MOVE_STRAIGHT_COST * remaining);
            }
            else
                return MOVE_STRAIGHT_COST * (xDistance + yDistance);
        }

        private static List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            while (endNode != null)
            {
                path.Add(endNode);
                endNode = endNode.previousNode;
            }

            path.Reverse();
            return path;
        }

        private static PathNode GetLowestFCostNode(in List<PathNode> nodeList)
        {
            PathNode lowestFCostNode = nodeList[0];
            int nbNode = nodeList.Count;
            for (int i = 1; i < nbNode; i++)
            {
                if (nodeList[i].FCost < lowestFCostNode.FCost)
                    lowestFCostNode = nodeList[i];
            }

            return lowestFCostNode;
        }

        #endregion

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private Grid<PathNode> grid;
        private List<PathNode> openList;
        private List<PathNode> closedList;

        private bool allowDiagonal;
        public bool AllowDiagonal
        {
            get => allowDiagonal;
            set
            {
                if (value)
                {
                    directionArray = new Vector2Int[]
                    {
                        Vector2Int.right,
                        Vector2Int.one,
                        Vector2Int.up,
                        Vector2Int.left + Vector2Int.up,
                        Vector2Int.left,
                        -Vector2Int.one,
                        Vector2Int.down,
                        Vector2Int.down + Vector2Int.right,
                    };
                }
                else
                {
                    directionArray = new Vector2Int[]
                    {
                        Vector2Int.right,
                        Vector2Int.up,
                        Vector2Int.left,
                        Vector2Int.down,
                    };
                }
            }
        }

        private Vector2Int[] directionArray;

        public Pathfinding(int width, int height, bool allowDiagonal)
        {
            grid = new Grid<PathNode>(width, height);
            AllowDiagonal = allowDiagonal;

            List<PathNode> nodeList = new List<PathNode>();
            PathNode newNode;
            int y;
            for (int x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    newNode = new PathNode(x, y, this);
                    grid.SetCellValue(x, y, newNode);
                    nodeList.Add(newNode);
                }
            }

            foreach (PathNode node in nodeList)
                node.SetNeighbours(directionArray, grid);
        }

        public void SetIsWalkable(in int x, in int y, in bool isWalkable)
        {
            if (grid.IsCellWithinGrid(x, y))
                grid.GetCellValue(x, y).isWalkable = isWalkable;
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            PathNode startNode = grid.GetCellValue(startX, startY);
            PathNode endNode = grid.GetCellValue(endX, endY);

            openList = new List<PathNode>()
            {
                startNode,
            };

            closedList = new List<PathNode>();

            int y;
            for (int x = 0; x < grid.Width; x++)
            {
                for (y = 0; y < grid.Height; y++)
                {
                    grid.GetCellValue(x, y).Reset();
                }
            }

            startNode.gCost = 0;
            startNode.CalculateHCost(endNode);

            PathNode currentNode;
            int i;
            int nbNeighbours;
            PathNode[] neighbourArray;
            while (openList.Count > 0)
            {
                currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode) 
                    return CalculatePath(endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                neighbourArray = currentNode.Neighbours;
                nbNeighbours = neighbourArray.Length;
                int tentativeGCost;
                PathNode neighbour;
                for (i = 0; i < nbNeighbours; i++)
                {
                    neighbour = neighbourArray[i];
                    if (closedList.Contains(neighbour)) continue;
                    else if (!neighbour.isWalkable)
                    {
                        closedList.Add(neighbour);
                        continue;
                    }

                    tentativeGCost = currentNode.gCost +
                        CalculateDistanceCost(currentNode, neighbour, AllowDiagonal);

                    if (tentativeGCost < neighbour.gCost)
                    {
                        neighbour.previousNode = currentNode;
                        neighbour.gCost = tentativeGCost;
                        neighbour.CalculateHCost(endNode);

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null; // No path found
        }
    }
}