using UnityEngine;

public class GridSystem<TGridObject>
{
    private readonly int width;
    private readonly int height;
    private readonly int cellSize;
    private readonly Vector3 origin;

    private readonly TGridObject[,] grid;

    public GridSystem(int pWidth, int pHeight, int pCellSize, Vector3 pOrigin)
    {
        width    = pWidth;
        height   = pHeight;
        cellSize = pCellSize;
        origin   = pOrigin;

        grid = new TGridObject[width, height];
    }

    public TGridObject GetGridObject(in int pX, in int pY) => grid[pX, pY];
    public TGridObject GetGridObject(Vector3 pPosition)
    {
        GetCellCoordinates(pPosition, out int pX, out int pY);
        return GetGridObject(pX, pY);
    }

    public void SetGridObject(int pX, int pY, in TGridObject pGridObject) => grid[pX, pY] = pGridObject;
    public void SetGridObject(Vector2Int pCell, in TGridObject pGridObject) => SetGridObject(pCell.x, pCell.y, pGridObject);
    public void SetGridObject(Vector2Int[] pCells, in TGridObject pGridObject)
    {
        foreach (Vector2Int lCell in pCells) SetGridObject(lCell, pGridObject);
    }

    public Vector3 GetCellPosition(int pX, int pY) => origin + new Vector3(pX + (float)cellSize / 2, 0, pY + (float)cellSize / 2) * cellSize;

    /// <summary>
    /// Given a world position, returns the cell X and Y.
    /// </summary>
    /// <param name="pPosition">The world position.</param>
    /// <param name="pX">The X of the cell at the given position.
    /// Returns -1 if the position doesn't correspond to a cell.</param>
    /// <param name="pY">The Y of the cell at the given position.
    /// Returns -1 if the position doesn't correspond to a cell.</param>
    public void GetCellCoordinates(Vector3 pPosition, out int pX, out int pY)
    {
        pPosition -= origin;
        if (pPosition.x < 0 || pPosition.y < 0)
        {
            pX = -1;
            pY = -1;
            return;
        }
        pPosition /= cellSize;
        pX = (int)pPosition.x;
        pY = (int)pPosition.z;
    }

    public bool IsCellWithinGrid(int pX, int pY) => pX >= 0 && pX < width && pY >= 0 && pY < height;
    public bool IsCellWithinGrid(in Vector2Int pCell) => IsCellWithinGrid(pCell.x, pCell.y);

    public bool IsPositionWithinGrid(in Vector3 pPosition)
    {
        GetCellCoordinates(pPosition, out int pX, out int pY);
        if (IsCellWithinGrid(pX, pY))
            return true;

        return false;
    }
}