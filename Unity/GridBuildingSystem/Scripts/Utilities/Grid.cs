using UnityEngine;

namespace ArTiX.Utils
{
    public class Grid<T>
    {
        private int width;
        private int height;

        private T[,] grid;

        public Grid(in int width, in int height)
        {
            this.width = width;
            this.height = height;

            grid = new T[width, height];
        }

        public T GetCellValue(in int x, in int y)
        {
            if (!IsCellWithinGrid(x, y)) return default;
            return grid[x, y];
        }
        public T GetCellValue(in Vector2Int cell) => GetCellValue(cell.x, cell.y);

        public void SetCellValue(in int x, in int y, in T value)
        {
            if (!IsCellWithinGrid(x, y)) return;
            grid[x, y] = value;
        }
        public void SetCellValue(in Vector2Int cell, in T value) => SetCellValue(cell.x, cell.y, value);

        public bool IsCellWithinGrid(in int x, in int y) => x >= 0 && x < width && y >= 0 && y < height;
        public bool IsCellWithinGrid(in Vector2Int cell) => IsCellWithinGrid(cell.x, cell.y);

        public void Clear()
        {
            for (int x =  0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = default;
                }
            }
        }
    }
}