using UnityEngine;

namespace GridSystem
{
    public class Grid<T>
    {
        public Grid(in int width, in int height)
        {
            grid = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                    grid[x, y] = default;
            }
        }

        private T[,] grid;

        public T GetGridObject(in Vector2Int cell)
        {
            return IsCellWithinGrid(cell) ? grid[cell.x, cell.y] : default;
        }

        public void SetGridObject(in Vector2Int cell, in T value)
        {
            if (IsCellWithinGrid(cell))
                grid[cell.x, cell.y] = value;
            else
                Debug.Log("Didn't set the value.");
        }

        public bool AreCellsWithinGrid(params Vector2Int[] cells)
        {
            int nbCell = cells.Length;
            for (int i = 0; i < nbCell; i++)
            {
                if (!IsCellWithinGrid(cells[i])) return false;
            }

            return true;
        }

        private bool IsCellWithinGrid(in Vector2Int cell)
        {
            try
            {
                T trashValue = grid[cell.x, cell.y];
                return true;
            }
            catch
            {
                if (cell.x < 0)
                    Debug.LogError("X is negative. It must be positive");

                int width = grid.GetLength(0);
                if (cell.x >= width)
                    Debug.LogError($"X equals {cell.x} but the grid width equals {width}." +
                        $" X must be less than {width}");

                if (cell.y < 0)
                    Debug.LogError("Y is negative. It must be positive");

                int height = grid.GetLength(1);
                if (cell.y >= height)
                    Debug.LogError($"Y equals {cell.y} but the grid height equals {height}." +
                        $" Y must be less than {height}");

                Debug.LogError("Cell not inside grid.");

                return false;
            }
        }
    }
}