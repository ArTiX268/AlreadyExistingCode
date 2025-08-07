using ArTiX;
using UnityEngine;

public class GridSystem2D<TGridObject>
{
    private int width;
    private int height;
    private int cellSize;
    private Vector3 originPosition;

    private TGridObject[,] grid;

    public GridSystem2D(int width, int height, int cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        grid = new TGridObject[width, height];
    }

    public int GetWidth() { return width; }

    public int GetHeight() { return height; }

    public float GetCellSize() { return cellSize; }

    public TGridObject GetGridObject(Vector3 position)
    {
        try
        {
            GetXY(position, out int x, out int y);
            return GetGridObject(x, y);
        }
        catch
        {
            return default;
        }
    }

    public TGridObject GetGridObject(int x, int y)
    {
        try
        {
            return grid[x, y];
        }
        catch
        {
            return default;
        }
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        try
        {
            grid[x, y] = value;
        }
        catch
        {
            return;
        }
    }

    public void SetGridObject(Vector3 position, TGridObject value)
    {
        try
        {
            GetXY(position, out int x, out int y);
            grid[x, y] = value;
        }
        catch
        {
            return;
        }
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        try
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }
        catch
        {
            x = -1000;
            y = -1000;
        }
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector3 GetMouseSnappedPosition()
    {
        Vector2 mousePos = Utilities.GetMousePosition2D();
        GetXY(mousePos, out int x, out int y);
        return GetWorldPosition(x, y);
    }
}