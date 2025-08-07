using ArTiX;
using UnityEngine;

public class GridBuildingSystem3D : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;

    [SerializeField] private Vector3 originPosition;

    [SerializeField] private Building3D[] buildings;

    [SerializeField] private Material canBuildMat;
    [SerializeField] private Material cannotBuildMat;

    [HideInInspector] public Building3D currentBuilding;

    private Direction currentDirection;

    private GridSystem3D<PlacedObject3D> gridSystem;

    private Transform currentGhost;

    private void Start()
    {
        currentBuilding = buildings[0];

        gridSystem = new GridSystem3D<PlacedObject3D>(width, height, cellSize, originPosition);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellPos = new Vector3(x, 0, z) * cellSize + originPosition;
                Utilities.DrawSquareXZ(cellPos, cellSize, Color.black, 100);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Utilities.GetMousePosition3D();
            CreateBuilding(mousePos);
        }

        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mousePos = Utilities.GetMousePosition3D();
            ClearBuilding(mousePos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Utilities.GetMousePosition3D();
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
        gridSystem.GetXZ(worldPos, out int x, out int z);
        Vector2Int offset = new(x, z);

        Vector2Int[] gridPositionList = currentBuilding.GetGridPositionList(offset, currentDirection);

        if (CanBuild(gridPositionList))
        {
            Vector3 placedObjectWorldPosition = gridSystem.GetWorldPosition(x, z) + (new Vector3(cellSize, 0, cellSize) / 2);

            try
            {
                PlacedObject3D placedObject =
                Instantiate(
                currentBuilding.prefab,
                placedObjectWorldPosition,
                Quaternion.Euler(currentBuilding.visual.transform.rotation.x, currentBuilding.GetRotationAngle(currentDirection), 0))
                .GetComponent<PlacedObject3D>();

                placedObject.gridPositionList = gridPositionList;

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    gridSystem.SetGridObject(gridPosition.x, gridPosition.y, placedObject);
                }
            }
            catch
            {
                Debug.LogError("There is no PlacedObject3D script on the prefab of the current building.");
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

    private bool CanBuild(int x, int z)
    {
        PlacedObject3D placedObject = gridSystem.GetGridObject(x, z);
        if (placedObject == null && x < gridSystem.GetWidth() && z < gridSystem.GetHeight() && x >= 0 && z >= 0)
            return true;
        else
            return false;
    }

    private bool CanBuild(Vector2Int[] gridPositionList)
    {
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!CanBuild(gridPosition.x, gridPosition.y))
            {
                return false;
            }
        }
        return true;
    }

    private void ClearBuilding(Vector3 worldPos)
    {
        gridSystem.GetXZ(worldPos, out int x, out int z);

        try
        {
            PlacedObject3D placedObject = gridSystem.GetGridObject(x, z);
            Vector2Int[] gridPositionList = placedObject.gridPositionList;

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
                    currentBuilding.GetRotationAngle(currentDirection),
                    0))
                .transform;
    }

    private void MoveGhost()
    {
        if (currentGhost != null)
        {
            currentGhost.position = GetMouseSnappedPosition();

            gridSystem.GetXZ(Utilities.GetMousePosition3D(), out int x, out int z);
            Vector2Int offset = new(x, z);

            Vector2Int[] gridPositionList = currentBuilding.GetGridPositionList(offset, currentDirection);

            MeshRenderer[] meshRenderers = currentGhost.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < meshRenderers.Length; i++)
                if (CanBuild(gridPositionList))
                    meshRenderers[i].material = canBuildMat;
                else
                    meshRenderers[i].material = cannotBuildMat;
        }
    }

    private Vector3 GetMouseSnappedPosition()
    {
        gridSystem.GetXZ(Utilities.GetMousePosition3D(), out int x, out int z);
        return new Vector3((x) * cellSize, 0, (z) * cellSize) + new Vector3(cellSize, 0, cellSize) / 2;
    }

    private void RefreshGhost()
    {
        Destroy(currentGhost.gameObject);
    }

    #endregion BuildingGhost
}