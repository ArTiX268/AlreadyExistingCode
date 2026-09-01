using ArTiX.GridBuildingSystem.Buildings;
using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.GridBuildingSystem
{
    public class GridBuildingSystem : MonoBehaviour
    {
        private static GridBuildingSystem instance;
        public static GridBuildingSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(GridBuildingSystem), typeof(GridBuildingSystem))
                        .GetComponent<GridBuildingSystem>();
                }
                return instance;
            }
        }

        #region Variables

        [SerializeField] private GridSO datas;
        [SerializeField] private Transform buildingsParent;

        private Grid<PlacedObject> grid;
        private Pathfinding pathfinding;
        private GameObject gridVisual;
        /// <summary>
        /// One pixel per cell: white when the cell holds a building, black when it is free.
        /// Fed to the grid shader so it can tell occupied cells apart.
        /// </summary>
        private Texture2D occupancyTexture;

        private static readonly Color OCCUPIED_CELL_COLOR = Color.white;
        private static readonly Color FREE_CELL_COLOR = Color.black;

        private BuildingState buildingState;
        private DestroyState destroyState;
        private GameState currentState;

        #endregion

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            GenerateGrid();

            buildingState = new BuildingState();
            destroyState = new DestroyState();
        }

        private void Update()
        {
            currentState?.Update();
        }

        public void GenerateGrid()
        {
            if (grid != null)
            {
                for (int i = 0; i < datas.Width; i++)
                {
                    for (int j = 0; j < datas.Height; j++)
                    {
                        if (grid.GetCellValue(i, j) != null)
                            Destroy(grid.GetCellValue(i, j));
                    }
                }

                grid.Clear();
                ClearOccupancy();
                return;
            }

            grid = new Grid<PlacedObject>(datas.Width, datas.Height);
            pathfinding = new Pathfinding(datas.Width, datas.Height, allowDiagonal: false);

            gridVisual = Instantiate(datas.PrfbGridVisual, datas.Origin, Quaternion.identity);
            // Dividing by 10 because using a plane and a plane base size is 10 units in width and height
            gridVisual.transform.localScale = new Vector3(datas.Width / 10, 1, datas.Height / 10);
            // Rotating 180° because of the occupancy map.
            gridVisual.transform.rotation = Quaternion.Euler(0, 180, 0);

            occupancyTexture = new Texture2D(datas.Width, datas.Height, TextureFormat.RGBA32, mipChain: false)
            {
                name = "GridOccupancy",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            Material gridMat = gridVisual.GetComponent<Renderer>().material;
            gridMat.SetFloat(GridSO.WIDTH, datas.Width);
            gridMat.SetFloat(GridSO.HEIGHT, datas.Height);
            gridMat.SetFloat(GridSO.CELL_SIZE, datas.CellSize);
            gridMat.SetFloat(GridSO.LINE_WIDTH, datas.LineWidth);
            gridMat.SetTexture(GridSO.OCCUPANCY_MAP, occupancyTexture);

            ClearOccupancy();
            ToggleGridVisual(false);
        }

        #region Occupancy Texture

        private void ClearOccupancy()
        {
            Color[] pixels = new Color[occupancyTexture.width * occupancyTexture.height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = FREE_CELL_COLOR;

            occupancyTexture.SetPixels(pixels);
            occupancyTexture.Apply(updateMipmaps: false);
        }

        private void SetCellOccupancy(in Vector2Int cell, in bool isOccupied)
        {
            occupancyTexture.SetPixel(cell.x, cell.y, isOccupied ? OCCUPIED_CELL_COLOR : FREE_CELL_COLOR);
        }

        #endregion

        public void ToggleGridVisual(in bool isVisible)
        {
            gridVisual.SetActive(isVisible);
        }

        public PlacedObject GetObjectOnCell(Vector2Int cell) => grid.GetCellValue(cell);

        private void SwitchState(in GameState newState)
        {
            if (newState != currentState)
            {
                if (currentState != null)
                {
                    currentState.OnExitState -= ExitCurrentState;
                    currentState?.ExitState();
                }

                currentState = newState;

                if (currentState != null)
                {
                    currentState.EnterState();
                    currentState.OnExitState += ExitCurrentState;
                }
            }
        }

        private void ExitCurrentState() => currentState = null;

        #region World pos, grid pos Conversion

        private Vector2Int ConvertWorldPosToCellPos(Vector3 worldPos)
        {
            worldPos.y = 0;
            worldPos += .5f * new Vector3(datas.Width, 0, datas.Height);

            return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));
        }

        public Vector3 ConvertCellPosToWorldPos(in Vector2Int cell)
        {
            Vector3 worldPos = new Vector3(
                x: datas.CellSize * (cell.x + .5f - (datas.Width * .5f)),
                y: 0,
                z: datas.CellSize * (cell.y + .5f - (datas.Width * .5f))
            );

            return worldPos;
        }

        private Vector3 RealWorldMousePosOnGrid()
        {
            Transform camTrans = Camera.main.transform;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            // Intersection between a line between camera and the ground.
            float t = -camTrans.position.y / ray.direction.y;

            return new Vector3(
                x: (t * ray.direction.x) + camTrans.position.x,
                y: 0,
                z: (t * ray.direction.z) + camTrans.position.z);
        }

        public Vector2Int GetMouseCellPos() => ConvertWorldPosToCellPos(RealWorldMousePosOnGrid());

        #endregion

        #region Building

        public void SelectBuilding(BuildingSO buildingDatas)
        {
            SwitchState(buildingState);
            buildingState.SetBuildingDatas(buildingDatas);
        }

        public void EnterDestructionState()
        {
            SwitchState(destroyState);
        }

        public List<Vector2Int> FindPath(in Vector2Int start, in Vector2Int end, in PlacedObjectSO placedObjectDatas)
        {
            if (!grid.IsCellWithinGrid(start) || !grid.IsCellWithinGrid(end)) return null;

            List<Pathfinding.PathNode> nodePath = pathfinding.FindPath(start.x, start.y, end.x, end.y);
            if (nodePath == null) return null;

            List<Vector2Int> path = new List<Vector2Int>();
            foreach (Pathfinding.PathNode node in nodePath)
                path.Add(new Vector2Int(node.X, node.Y));

            return path;
        }

        public void CreateBuilding(PlacedObjectSO placedObjectDatas, Vector2Int origin, float rotation = 0)
        {
            if (CanBuild(placedObjectDatas, origin))
            {
                PlacedObject building = Instantiate(
                    original: placedObjectDatas.Prefab,
                    position: ConvertCellPosToWorldPos(origin),
                    rotation: Quaternion.Euler(x: 0, -rotation, z: 0),
                    buildingsParent
                );

                Instantiate(datas.PrefabSpawnSmoke, ConvertCellPosToWorldPos(origin), Quaternion.identity);

                foreach (Vector2Int cell in placedObjectDatas.GetOccupiedCells(origin, rotation))
                {
                    grid.SetCellValue(cell, building);
                    pathfinding.SetIsWalkable(cell.x, cell.y, false);
                    SetCellOccupancy(cell, isOccupied: true);
                }
                occupancyTexture.Apply(updateMipmaps: false);

                building.Initialize(origin);
            }
            else
            {
                // Fail effects
            }
        }

        public bool CanBuild(in PlacedObjectSO placedObjectDatas, in Vector2Int origin, in float rotation = 0)
        {
            foreach (Vector2Int cell in placedObjectDatas.GetOccupiedCells(origin, rotation))
            {
                if (grid.GetCellValue(cell) != null || !grid.IsCellWithinGrid(cell))
                    return false;
            }

            return true;
        }

        public void DestroyBuilding(PlacedObject building)
        {
            foreach (Vector2Int cell in building.GetOccupiedCells())
            {
                grid.SetCellValue(cell, null);
                pathfinding.SetIsWalkable(cell.x, cell.y, true);
                SetCellOccupancy(cell, isOccupied: false);
            }
            occupancyTexture.Apply(updateMipmaps: false);

            building.Destroy();
        }

        #endregion
    }
}