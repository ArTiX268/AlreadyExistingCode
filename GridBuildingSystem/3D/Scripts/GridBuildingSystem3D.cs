using ArTiX;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridBuildingSystem3D : MonoBehaviour
{
    #region Variables

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;

    [SerializeField] private Vector3 originPosition;

    [SerializeField] private Building3D[] buildings;

    [SerializeField] private Transform gridVisual;

    [SerializeField] private Material canBuildMat;
    [SerializeField] private Material cannotBuildMat;

    [HideInInspector] public Building3D currentBuilding;

    private Direction currentDirection;

    private GridSystem3D<PlacedObject3D> gridSystem;

    private Transform currentGhost;

    #endregion

    private void Start()
    {
        gridSystem = new GridSystem3D<PlacedObject3D>(width, height, cellSize, originPosition);

        AssignInput();

        SetGridVisual();
    }

    private void Update()
    {
        if (currentBuilding != null)
            MoveGhost();
    }

    private void SetGridVisual()
    {
        Material gridMat = gridVisual.GetComponent<MeshRenderer>().material;

        float invertCellSize = 1 / (float)cellSize;
        gridMat.SetVector("_Size", new Vector2(invertCellSize, invertCellSize));
        Vector2 defaultScale = gridMat.GetVector("_DefaultScale");
        Vector3 offset = new(width * .5f * cellSize, 0.015f, height * .5f * cellSize);

        gridVisual.localScale = new Vector3((width * cellSize) / defaultScale.x, 1, (height * cellSize) / defaultScale.y);
        gridVisual.position = originPosition + offset;
    }

    private void AssignInput()
    {
        InputManager.AssignEvent(ref InputManager.placeBuildingAction, CreateBuilding, EventType.Started);
        InputManager.DisableInput(ref InputManager.placeBuildingAction);

        InputManager.AssignEvent(ref InputManager.rotateBuildingAction, ChangeDirection, EventType.Started);
        InputManager.DisableInput(ref InputManager.rotateBuildingAction);

        InputManager.AssignEvent(ref InputManager.cancelAction, CancelBuilding, EventType.Started);
        InputManager.DisableInput(ref InputManager.cancelAction);
    }

    #region InputMethod

    public void CancelBuilding(InputAction.CallbackContext context)
    {
        GameManager.instance.ExitBuildingMode();
    }

    public void EnterBuildingMode()
    {
        gridVisual.gameObject.SetActive(true);
        InputManager.EnableInput(ref InputManager.placeBuildingAction);
        InputManager.EnableInput(ref InputManager.rotateBuildingAction);
        InputManager.EnableInput(ref InputManager.cancelAction);
    }

    public void ExitBuildingMode()
    {
        gridVisual.gameObject.SetActive(false);
        currentBuilding = null;
        RefreshGhost();
        InputManager.DisableInput(ref InputManager.placeBuildingAction);
        InputManager.DisableInput(ref InputManager.rotateBuildingAction);
        InputManager.DisableInput(ref InputManager.cancelAction);
    }

    public void CreateBuilding(InputAction.CallbackContext context)
    {
        if (currentBuilding != null)
        {
            Vector3 mousePos = Utilities.GetMousePosition3D();
            CreateBuilding(mousePos);
        }
    }

    #endregion

    #region Building

    public void ChangeDirection(InputAction.CallbackContext context)
    {
        if (currentBuilding != null)
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

            RefreshGhost();
        }
    }

    private void CreateBuilding(Vector3 worldPos)
    {
        gridSystem.GetXZ(worldPos, out int x, out int z);
        Vector2Int offset = new(x, z);

        Vector2Int[] gridPositionList = currentBuilding.GetGridPositionList(offset, currentDirection);

        if (CanBuild(gridPositionList))
        {
            try
            {
                Vector3 placedObjectWorldPosition = GetPlacedObjectWorldPosition(x, z);

                PlacedObject3D placedObject = SpawnBuilding(placedObjectWorldPosition);

                SetPlacedBuildingVariables(placedObject, gridPositionList);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    gridSystem.SetGridObject(gridPosition.x, gridPosition.y, placedObject);
                }

                List<PlacedObject3D> adjacentBuildings = GetAdjacentBuilding(gridPositionList, placedObject);
                ApplyEffect(adjacentBuildings);

                RemoveRessources();
            }
            catch
            {
                return;
            }
        }
    }

    private void SetPlacedBuildingVariables(PlacedObject3D placedObject, Vector2Int[] gridPositionList)
    {
        placedObject.gridPositionList = gridPositionList;
        placedObject.buildingType = currentBuilding.buildingType;
    }

    private void RemoveRessources()
    {
        foreach (NeededRessource neededRessource in currentBuilding.neededRessources)
        {
            RessourceManager.instance.SubtractRessource(
                neededRessource.ressourceType, neededRessource.ressourceNeeded);
        }
    }

    private PlacedObject3D SpawnBuilding(Vector3 placedObjectWorldPosition)
    {
        PlacedObject3D placedObject =
        Instantiate(
        currentBuilding.prefab,
        placedObjectWorldPosition,
        Quaternion.Euler(
            currentBuilding.visual.transform.rotation.x,
            currentBuilding.GetRotationAngle(currentDirection), 0))
        .GetComponent<PlacedObject3D>();

        return placedObject;
    }

    private Vector3 GetPlacedObjectWorldPosition(int x, int z)
    {
        return gridSystem.GetWorldPosition(x, z) + (new Vector3(cellSize, 0, cellSize) / 2);
    }

    private List<PlacedObject3D> GetAdjacentBuilding(Vector2Int[] gridPositionList, PlacedObject3D placedObject)
    {
        List<PlacedObject3D> adjacentBuildings = new List<PlacedObject3D>();

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            // Not in diagonal
            CheckAdjacentCells(new(1, 0),  gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(0, 1), gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(-1, 0), gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(0, -1), gridPosition, placedObject, ref adjacentBuildings);

            // In diagonal
            CheckAdjacentCells(new(1, 1), gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(-1, 1), gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(-1, -1), gridPosition, placedObject, ref adjacentBuildings);
            CheckAdjacentCells(new(1, -1), gridPosition, placedObject, ref adjacentBuildings);
        }

        return adjacentBuildings;
    }

    private void CheckAdjacentCells(Vector2Int direction, Vector2Int gridPosition, PlacedObject3D placedObject, ref List<PlacedObject3D> adjacentBuildings)
    {
        Vector2Int currentCheckedCell = Vector2Int.zero;

        for (int i = 1; i <= currentBuilding.rangeOfEffect; i++)
        {
            if (direction.magnitude > 1 && i > 1)
            {
                currentCheckedCell = gridPosition + (direction * (i - 1));
            }
            else if (direction.magnitude == 1)
            {
                currentCheckedCell = gridPosition + (direction * i);
            }

            PlacedObject3D currentlyCheckedObject = gridSystem.GetGridObject(currentCheckedCell.x, currentCheckedCell.y);

            if
                (gridSystem.IsInGrid(currentCheckedCell.x, currentCheckedCell.y) &&
                currentlyCheckedObject != null && currentlyCheckedObject != placedObject &&
                !adjacentBuildings.Contains(currentlyCheckedObject))
            {
                adjacentBuildings.Add(currentlyCheckedObject);
            }
        }
    }

    private void ApplyEffect(List<PlacedObject3D> adjacentBuildings)
    {
        foreach (Effect effect in currentBuilding.effects)
        {
            if (effect.buildingNecessary)
            {
                RessourceManager.instance.AddRessource(
                    effect.ressourceType,
                    effect.reward * GetNumberOfSameTypeBuilding(effect.buildingType, adjacentBuildings));
            }
            else
                RessourceManager.instance.AddRessource(effect.ressourceType, effect.reward);
        }
    }

    private int GetNumberOfSameTypeBuilding(BuildingType buildingType, List<PlacedObject3D> buildings)
    {
        int number = 0;

        foreach (PlacedObject3D building in buildings)
        {
            if (building.buildingType == buildingType)
                number++;
        }

        return number;
    }

    private bool CanBuild(int x, int z)
    {
        if (Utilities.GetMousePosition3D() != Vector3.zero)
        {
            PlacedObject3D placedObject = gridSystem.GetGridObject(x, z);
            if (placedObject == null && IsWithinGrid(x, z))
                if (RessourceManager.instance.HasEnoughRessources(currentBuilding.neededRessources))
                    return true;
        }

        return false;
    }

    private bool IsWithinGrid(int x, int z)
    {
        return x < gridSystem.GetWidth() && z < gridSystem.GetHeight() && x >= 0 && z >= 0;
    }

    private bool IsWithinGrid(Vector2Int[] cellsInTheVoid)
    {
        foreach (Vector2Int cell in cellsInTheVoid)
        {
            if (IsWithinGrid(cell.x, cell.y))
                return true;
        }
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

        if (currentBuilding.mustBeNearAnEdge)
        {
            gridSystem.GetXZ(Utilities.GetMousePosition3D(), out int x, out int z);
            Vector2Int offset = new(x, z);

            if (IsWithinGrid(currentBuilding.GetGridCellsInTheVoidList(offset, currentDirection)))
                return false;
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

    public void ChangeBuilding(Building3D newBuilding)
    {
        currentBuilding = newBuilding;
        RefreshGhost();
    }

    #endregion Building

    #region BuildingGhost

    private void CreateGhost()
    {
        if (currentGhost == null)
        {
            currentGhost = Instantiate(
                currentBuilding.visual,
                GetMouseSnappedPosition(),
                Quaternion.Euler(
                    0,
                    currentBuilding.GetRotationAngle(currentDirection),
                    0))
                .transform;
        }
    }

    private void MoveGhost()
    {
        if (currentGhost != null)
        {
            currentGhost.position = GetMouseSnappedPosition();

            UpdateGhostVisual();
        }
        else
            CreateGhost();
    }

    private void UpdateGhostVisual()
    {
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

    private Vector3 GetMouseSnappedPosition()
    {
        gridSystem.GetXZ(Utilities.GetMousePosition3D(), out int x, out int z);
        return new Vector3((x) * cellSize, 0,(z) * cellSize)+ new Vector3(cellSize, 0, cellSize) / 2;
    }

    private void RefreshGhost()
    {
        if (currentGhost != null)
            Destroy(currentGhost.gameObject);
    }

    #endregion BuildingGhost
}