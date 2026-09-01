using UnityEngine;

namespace ArTiX.Utils
{
    public class Grid<T>
    {
        public int Width { get; private set; }
        public int Height {  get; private set; }

        private T[,] grid;

        public Grid(in int width, in int height)
        {
            Width = width;
            Height = height;

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

        public bool IsCellWithinGrid(in int x, in int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool IsCellWithinGrid(in Vector2Int cell) => IsCellWithinGrid(cell.x, cell.y);

        public void Clear()
        {
            for (int x =  0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    grid[x, y] = default;
                }
            }
        }
    }
}