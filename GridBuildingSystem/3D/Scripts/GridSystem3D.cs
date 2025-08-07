using ArTiX;
using UnityEngine;

public class GridSystem3D<TGridObject>
{
    private int width;
    private int height;
    private int cellSize;
    private Vector3 originPosition;

    private TGridObject[,] grid;

    public GridSystem3D(int width, int height, int cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        grid = new TGridObject[width, height];
    }

    public int GetWidth()
    { return width; }

    public int GetHeight()
    { return height; }

    public float GetCellSize()
    { return cellSize; }

    public TGridObject GetGridObject(Vector3 position)
    {
        try
        {
            GetXZ(position, out int x, out int z);
            return GetGridObject(x, z);
        }
        catch
        {
            return default;
        }
    }

    public TGridObject GetGridObject(int x, int z)
    {
        try
        {
            return grid[x, z];
        }
        catch
        {
            return default;
        }
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        try
        {
            grid[x, z] = value;
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
            GetXZ(position, out int x, out int z);
            grid[x, z] = value;
        }
        catch
        {
            return;
        }
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        try
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        }
        catch
        {
            x = -1000;
            z = -1000;
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public Vector3 GetMouseSnappedPosition()
    {
        Vector3 mousePos = Utilities.GetMousePosition3D();
        GetXZ(mousePos, out int x, out int z);
        return GetWorldPosition(x, z);
    }
}