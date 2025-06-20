using ArTiX;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem2D : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;

    [SerializeField] private Vector3 originPosition;

    [SerializeField] private Building2D[] buildings;

    [HideInInspector] public Building2D currentBuilding;

    private Direction currentDirection;

    private GridSystem2D<PlacedObject2D> gridSystem;

    private Transform currentGhost;

    private void Start()
    {
        gridSystem = new GridSystem2D<PlacedObject2D>(width, height, cellSize, originPosition);

        currentBuilding = buildings[0];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 cellPos = new Vector2(x, y) * cellSize + (Vector2)originPosition;
                Utilities.DrawSquareXY(cellPos, cellSize, Color.black, 100);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Utilities.GetMousePosition2D();
            CreateBuilding(mousePos);
        }

        if (Input.GetMouseButtonDown(2))
        {
            Vector2 mousePos = Utilities.GetMousePosition2D();
            ClearBuilding(mousePos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Utilities.GetMousePosition2D();
            try
            {
                Debug.Log(gridSystem.GetGridObject(mousePos));
            }
            catch
            {
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeDirection();
            RefreshGhost();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeBuilding();
            RefreshGhost();
        }

        CreateGhost();
        MoveGhost();
    }

    #region Building

    private void ChangeDirection()
    {
        switch (currentDirection)
        {
            case Direction.Up:
                currentDirection = Direction.Left;
                break;

            case Direction.Left:
                currentDirection = Direction.Down;
                break;

            case Direction.Down:
                currentDirection = Direction.Right;
                break;

            case Direction.Right:
                currentDirection = Direction.Up;
                break;
        }
    }

    private void CreateBuilding(Vector3 worldPos)
    {
        gridSystem.GetXY(worldPos, out int x, out int y);
        Vector2Int offset = new(x, y);
        Vector2 diagonal = currentBuilding.GetDiagonal(currentDirection);
        Vector2Int halfedDiagonal = new(Mathf.RoundToInt((diagonal.x - .01f) / 2), Mathf.RoundToInt((diagonal.y - .01f) / 2));

        List<Vector2Int> gridPositionList = currentBuilding.GetGridPositionList(offset - halfedDiagonal, currentDirection);

        if (CanBuild(gridPositionList))
        {
            Vector3 placedObjectWorldPosition = gridSystem.GetWorldPosition(x, y) + (new Vector3(cellSize, cellSize) / 2);
            PlacedObject2D placedObject =
                Instantiate(
                currentBuilding.prefab,
                placedObjectWorldPosition,
                Quaternion.Euler(0, 0, currentBuilding.GetRotationAngle(currentDirection)))
                .GetComponent<PlacedObject2D>();

            placedObject.gridPositionList = gridPositionList;

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridSystem.SetGridObject(gridPosition.x, gridPosition.y, placedObject);
            }
        }
    }

    public void ChangeBuilding()
    {
        if (currentBuilding == buildings[0])
            currentBuilding = buildings[1];
        else if (currentBuilding == buildings[1])
            currentBuilding = buildings[0];
    }

    private bool CanBuild(int x, int y)
    {
        PlacedObject2D placedObject = gridSystem.GetGridObject(x, y);
        if (placedObject == null && x < gridSystem.GetWidth() && y < gridSystem.GetHeight() && x >= 0 && y >= 0)
            return true;
        else
        {
            return false;
        }
    }

    private bool CanBuild(List<Vector2Int> gridPositionList)
    {
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!CanBuild(gridPosition.x, gridPosition.y))
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    private void ClearBuilding(Vector3 worldPos)
    {
        gridSystem.GetXY(worldPos, out int _x, out int _y);

        try
        {
            PlacedObject2D placedObject = gridSystem.GetGridObject(_x, _y);
            List<Vector2Int> gridPositionList = placedObject.gridPositionList;

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridSystem.SetGridObject(gridPosition.x, gridPosition.y, default);
            }

            Destroy(placedObject.gameObject);
        }
        catch
        {
            return;
        }
    }

    #endregion Building

    #region BuildingGhost

    private void CreateGhost()
    {
        if (currentGhost == null)
            currentGhost = Instantiate(
                currentBuilding.visual,
                GetMouseSnappedPosition(),
                Quaternion.Euler(
                    0,
                    0,
                    currentBuilding.GetRotationAngle(currentDirection)))
                .transform;
    }

    private void MoveGhost()
    {
        if (currentGhost != null)
            currentGhost.position = GetMouseSnappedPosition();
    }

    private Vector3 GetMouseSnappedPosition()
    {
        gridSystem.GetXY(Utilities.GetMousePosition2D(), out int x, out int y);
        return new Vector3(x, y) + new Vector3(cellSize, cellSize) / 2;
    }

    private void RefreshGhost()
    {
        Destroy(currentGhost.gameObject);
    }

    #endregion BuildingGhost
}