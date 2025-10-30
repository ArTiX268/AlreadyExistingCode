using ArTiX;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Com.ArTiX.FactoryGame
{
    public class GridBuildingSystem : MonoBehaviour
    {
        [SerializeField, AssetsOnly] private Transform prfb_GridVisual;
        [SerializeField, AssetsOnly] private Material mat_CanBuild;
        [SerializeField, AssetsOnly] private Material mat_CannotBuild;
        [SerializeField] private LayerMask buildingLayer;

        private const float MAX_CHECKING_DISTANCE = 1000f;

        private GridSystem<PlaceableBuilding> gridSystem;
        private BuildingSO currentBuildingData;
        private EBuildingRotation currentRotation = EBuildingRotation.Forward;
        private Transform buildingGhost;

        private PlaceableBuilding currentlyCheckedBuilding;
        private Material[] currentlyCheckedBuldingMats;

        private bool isInDestructionMode;

        private void Update()
        {
            // Move building ghost
            if (buildingGhost != null)
            {
                gridSystem.GetCellCoordinates(Utilities.GetMousePosition3D(), out int pX, out int pY);
                Vector3 lCellWorldPosition = gridSystem.GetCellPosition(pX, pY);
                Vector3 lPreviousGhostPosition = buildingGhost.position;
                buildingGhost.position = lCellWorldPosition;

                if (lPreviousGhostPosition != buildingGhost.position)
                    UpdateGhostLook(pX, pY);
            }

            // Update the building when in destruction mode;
            if (isInDestructionMode)
            {
                Ray lCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(lCameraRay, out RaycastHit pHit, MAX_CHECKING_DISTANCE, buildingLayer))
                {
                    PlaceableBuilding lCurrentBuilding = pHit.collider.GetComponentInParent<PlaceableBuilding>();
                    if (lCurrentBuilding != currentlyCheckedBuilding)
                    {
                        currentlyCheckedBuilding = lCurrentBuilding;
                        MeshRenderer[] lMeshRenderers = currentlyCheckedBuilding.GetComponentsInChildren<MeshRenderer>();
                        int lNbRenderers = lMeshRenderers.Length;
                        currentlyCheckedBuldingMats = new Material[lNbRenderers];

                        for (int i = 0; i < lNbRenderers; i++)
                        {
                            currentlyCheckedBuldingMats[i] = lMeshRenderers[i].material;
                            lMeshRenderers[i].material = mat_CannotBuild;
                        }
                    }
                }
                else if (currentlyCheckedBuilding != null)
                {
                    MeshRenderer[] lMeshRenderers = currentlyCheckedBuilding.GetComponentsInChildren<MeshRenderer>();
                    int lNbRenderer = lMeshRenderers.Length;

                    for (int i = 0; i < lNbRenderer; i++)
                    {
                        lMeshRenderers[i].material = currentlyCheckedBuldingMats[i];
                    }
                    currentlyCheckedBuilding = null;
                }
            }
        }

        #region Create Building

        public void CreateBuilding(in int pX, in int pY, in BuildingSO pBuildingData)
        {
            if (CanBuild(pX, pY, pBuildingData))
            {
                PlaceableBuilding lBuilding = Instantiate(
                    original: pBuildingData.prfb_Building,
                    position: gridSystem.GetCellPosition(pX, pY),
                    rotation: Quaternion.Euler(0, GetRealWorldRotation(), 0));

                lBuilding.SetOccupiedCells(pX, pY, currentRotation);

                foreach (Vector2Int lOccupiedCell in lBuilding.GetOccupiedCells())
                    gridSystem.SetGridObject(lOccupiedCell.x, lOccupiedCell.y, lBuilding);
            }
        }

        private void CreateBuilding(in Vector3 pPosition)
        {
            gridSystem.GetCellCoordinates(pPosition, out int pX, out int pY);
            CreateBuilding(pX, pY, currentBuildingData);
        }

        private void CreateBuilding(InputAction.CallbackContext pContext)
        {
            CreateBuilding(Utilities.GetMousePosition3D());
            ExitBuildingMode();
        }

        #endregion

        private void DestroyBuilding(InputAction.CallbackContext pContext)
        {
            Vector3 lMousePosition = Utilities.GetMousePosition3D();
            if (gridSystem.IsPositionWithinGrid(lMousePosition))
            {
                PlaceableBuilding lBuilding = gridSystem.GetGridObject(Utilities.GetMousePosition3D());
                if (lBuilding != null)
                {
                    gridSystem.SetGridObject(lBuilding.GetOccupiedCells(), null);
                    Destroy(lBuilding.gameObject);
                    ExitDestructionMode();
                }
            }
        }

        #region Modes

        public void EnterBuildingMode(in BuildingSO pBuilding)
        {
            currentBuildingData = pBuilding;
            buildingGhost = Instantiate(
                original: pBuilding.prfb_BuildingVisual,
                position: Vector3.zero,
                rotation: Quaternion.Euler(0, GetRealWorldRotation(), 0));

            InputManager.Instance.EnableInput(InputManager.EAction.SpawnBuilding);
            InputManager.Instance.AssignInput(InputManager.EAction.SpawnBuilding, CreateBuilding, InputManager.EventType.Started);
            InputManager.Instance.EnableInput(InputManager.EAction.Rotate);
            InputManager.Instance.AssignInput(InputManager.EAction.Rotate, Rotate, InputManager.EventType.Started);
            InputManager.Instance.EnableInput(InputManager.EAction.ExitBuildingMode);
            InputManager.Instance.AssignInput(InputManager.EAction.ExitBuildingMode, ExitBuildingMode, InputManager.EventType.Started);
        }

        public void ExitBuildingMode()
        {
            Destroy(buildingGhost.gameObject);
            buildingGhost = null;
            currentRotation = EBuildingRotation.Forward;

            InputManager.Instance.DisableInput(InputManager.EAction.SpawnBuilding);
            InputManager.Instance.UnassignInput(InputManager.EAction.SpawnBuilding, CreateBuilding, InputManager.EventType.Started);
            InputManager.Instance.DisableInput(InputManager.EAction.Rotate);
            InputManager.Instance.UnassignInput(InputManager.EAction.Rotate, Rotate, InputManager.EventType.Started);
            InputManager.Instance.DisableInput(InputManager.EAction.ExitBuildingMode);
            InputManager.Instance.UnassignInput(InputManager.EAction.ExitBuildingMode, ExitBuildingMode, InputManager.EventType.Started);
        }

        private void ExitBuildingMode(InputAction.CallbackContext pContext) => ExitBuildingMode();

        public void EnterDestructionMode()
        {
            isInDestructionMode = true;

            InputManager.Instance.EnableInput(InputManager.EAction.DestroyBuilding);
            InputManager.Instance.AssignInput(InputManager.EAction.DestroyBuilding, DestroyBuilding, InputManager.EventType.Started);
            InputManager.Instance.EnableInput(InputManager.EAction.ExitDestructionMode);
            InputManager.Instance.AssignInput(InputManager.EAction.ExitDestructionMode, ExitDestructionMode, InputManager.EventType.Started);
        }

        private void ExitDestructionMode()
        {
            isInDestructionMode = false;

            InputManager.Instance.DisableInput(InputManager.EAction.DestroyBuilding);
            InputManager.Instance.UnassignInput(InputManager.EAction.DestroyBuilding, DestroyBuilding, InputManager.EventType.Started);
            InputManager.Instance.DisableInput(InputManager.EAction.ExitDestructionMode);
            InputManager.Instance.UnassignInput(InputManager.EAction.ExitDestructionMode, ExitDestructionMode, InputManager.EventType.Started);
        }

        private void ExitDestructionMode(InputAction.CallbackContext pContext) => ExitDestructionMode();

        #endregion

        private bool CanBuild(in int pX, in int pY, in BuildingSO pBuildingData)
        {
            Vector2Int lCurrentCell;
            foreach (Vector2Int lCell in pBuildingData.GetCells(currentRotation))
            {
                lCurrentCell = new Vector2Int(lCell.x + pX, lCell.y + pY);
                if (!gridSystem.IsCellWithinGrid(lCurrentCell) ||
                    gridSystem.GetGridObject(lCurrentCell.x, lCurrentCell.y) != null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanBuild(in int pX, in int pY) => CanBuild(pX, pY, currentBuildingData);

        private void Rotate(InputAction.CallbackContext pContext)
        {
            switch (currentRotation)
            {
                case EBuildingRotation.Forward:
                    currentRotation = EBuildingRotation.Right;
                    break;
                case EBuildingRotation.Right:
                    currentRotation = EBuildingRotation.Backward;
                    break;
                case EBuildingRotation.Backward:
                    currentRotation = EBuildingRotation.Left;
                    break;
                case EBuildingRotation.Left:
                    currentRotation = EBuildingRotation.Forward;
                    break;
            }

            // Rotate ghost
            buildingGhost.rotation = Quaternion.Euler(0, GetRealWorldRotation(), 0);

            // Update its look
            gridSystem.GetCellCoordinates(Utilities.GetMousePosition3D(), out int pX, out int pY);
            UpdateGhostLook(pX, pY);
        }

        private void UpdateGhostLook(in int pX, in int pY)
        {
            MeshRenderer[] lGhostRenderers = buildingGhost.GetComponentsInChildren<MeshRenderer>();
            if (CanBuild(pX, pY))
            {
                foreach (MeshRenderer lMeshRenderer in lGhostRenderers)
                    lMeshRenderer.material = mat_CanBuild;
            }
            else
            {
                foreach (MeshRenderer lMeshRenderer in lGhostRenderers)
                    lMeshRenderer.material = mat_CannotBuild;
            }
        }

        #region Getter and Setter

        public void SetGrid(in GridSystem<PlaceableBuilding> pGrid) => gridSystem = pGrid;

        private float GetRealWorldRotation()
        {
            switch (currentRotation)
            {
                default:
                case EBuildingRotation.Forward:
                    return 0;
                case EBuildingRotation.Right:
                    return 90;
                case EBuildingRotation.Backward:
                    return 180;
                case EBuildingRotation.Left:
                    return 270;
            }
        }

        #endregion
    }
}