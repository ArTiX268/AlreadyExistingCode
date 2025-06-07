using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    private PlacedObjectTypeSO placedObjectTypeSO;
    [HideInInspector] public GridXZ<GridObject> grid;
    [HideInInspector] public PlacedObjectTypeSO.Dir dir = PlacedObjectTypeSO.Dir.Down;

    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;

    private void Awake()
    {
        Instance = this;

        grid = new GridXZ<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        placedObjectTypeSO = placedObjectTypeSOList[0];
    }

    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private PlacedObject placedObject;

        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, z);
        }

        public PlacedObject GetPlacedObject() { return placedObject; }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, z);
        }

        public bool CanBuild() { return placedObject == null; }

        public override string ToString() { return x + ", " + z + "\n" + placedObject; }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            {
                grid.GetXZ(hit.point, out int x, out int z);

                List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), dir);

                if (CanBuild(gridPositionList))
                {
                    Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                    Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

                    PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), dir, placedObjectTypeSO);

                    foreach (Vector2Int gridPosition in gridPositionList)
                        grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);

                    OnObjectPlaced?.Invoke(this, EventArgs.Empty);
                }
                else
                    UtilsClass.CreateWorldTextPopup("Cannot build here !", hit.point);
            }
            else
                Debug.Log("no were found");
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            {
                GridObject gridObject = grid.GetGridObject(hit.point);
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject != null)
                {
                    placedObject.DestroySelf();

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();

                    foreach (Vector2Int gridPosition in gridPositionList)
                        grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
                UtilsClass.CreateWorldTextPopup("" + dir, hit.point);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
    }

    public bool CanBuild(List<Vector2Int> gridPositionList)
    {
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (grid.GetGridObject(gridPosition.x, gridPosition.y) == null || !grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    public void DeselectObjectType() { placedObjectTypeSO = null; RefreshSelectedObjectType(); }

    private void RefreshSelectedObjectType() { OnSelectedChanged?.Invoke(this, EventArgs.Empty); }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
        {
            Vector3 mousePosition = hit.point;
            grid.GetXZ(mousePosition, out int x, out int z);

            if (placedObjectTypeSO != null)
            {
                Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
                Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                return placedObjectWorldPosition;
            }
            else
                return mousePosition;
        }
        else
            return Vector3.zero;
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir) + 180, 0);
        }
        else
            return Quaternion.identity;
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() { return placedObjectTypeSO; }
}