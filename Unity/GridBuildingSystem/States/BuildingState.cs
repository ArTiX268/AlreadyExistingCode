using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Input;
using ArTiX.Interaction;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class BuildingState : GameState
    {
        private const float ROTATION_DELTA = 90f;
        private const float MAX_ROTATION = 360;
        private float currentRotation;
        /// <summary>
        /// Because the Y axis goes UP and we want to rotate our building positively by looking FROM up,
        /// we need to invert the rotation.
        /// </summary>
        private float WorldRotation => -currentRotation;

        private BuildingSO currentBuildingDatas;
        private Ghost ghost;

        private Vector2Int currentCell;

        private bool isDragging;
        private Vector2Int dragStartCell;
        private List<Vector2Int> currentPath = new List<Vector2Int>();
        private readonly List<Ghost> pathGhosts = new List<Ghost>();

        public override void EnterState()
        {
            InputManager.Instance.OnStartCreatingBuilding += StartCreating;
            InputManager.Instance.OnFinishCreating += FinishCreating;
            InputManager.Instance.OnCancel += ExitState;
            InputManager.Instance.OnRotate += RotateBuilding;

            Interactor.Instance.Disable();
            currentRotation = 0;
            GridBuildingSystem.Instance.ToggleGridVisual(true);
        }

        public override void Update()
        {
            Vector2Int cellPos = GridBuildingSystem.Instance.GetMouseCellPos();
            bool cellChanged = currentCell != cellPos;

            if (!currentBuildingDatas.AllowDragPlacement)
            {
                // Snap to cell pos
                if (!cellChanged) return;
                currentCell = cellPos;

                ghost.SetPosition(cellPos);
                return;
            }

            currentCell = cellPos;

            if (isDragging && cellChanged && GridBuildingSystem.Instance.CanBuild(currentBuildingDatas, cellPos))
                UpdatePathPreview(cellPos);
            else if (ghost != null && cellChanged)
                ghost.SetPosition(cellPos);
        }

        public override void ExitState()
        {
            currentRotation = 0;
            isDragging = false;
            ghost?.Destroy();
            ghost = null;
            ClearPathGhosts();
            GridBuildingSystem.Instance.ToggleGridVisual(false);

            Interactor.Instance.Enable();
            InputManager.Instance.OnStartCreatingBuilding -= StartCreating;
            InputManager.Instance.OnFinishCreating -= FinishCreating;
            InputManager.Instance.OnCancel -= ExitState;
            InputManager.Instance.OnRotate -= RotateBuilding;

            base.ExitState();
        }

        public void SetBuildingDatas(in BuildingSO buildingDatas)
        {
            currentBuildingDatas = buildingDatas;
            isDragging = false;
            ghost?.Destroy();
            ghost = null;
            ClearPathGhosts();

            ghost = Ghost.Create(buildingDatas.Ghost);
        }

        private void StartCreating()
        {
            if (currentBuildingDatas.AllowDragPlacement)
            {
                isDragging = true;
                dragStartCell = GridBuildingSystem.Instance.GetMouseCellPos();
                UpdatePathPreview(dragStartCell);
                ghost.Destroy();
                ghost = null;
            }
        }

        private void FinishCreating()
        {
            if (currentBuildingDatas.AllowDragPlacement)
            {  
                foreach (Vector2Int cell in currentPath)
                {
                    GridBuildingSystem.Instance.CreateBuilding(currentBuildingDatas, cell, currentRotation);
                }
                isDragging = false;
                ClearPathGhosts();
                ghost = Ghost.Create(currentBuildingDatas.Ghost);
            }
            else
                GridBuildingSystem.Instance.CreateBuilding(currentBuildingDatas, currentCell, currentRotation);
        }

        private void RotateBuilding(int rotateDirection)
        {
            currentRotation = (currentRotation + (ROTATION_DELTA * rotateDirection)) % MAX_ROTATION;
            ghost?.SetRotation(WorldRotation);
        }

        private void UpdatePathPreview(in Vector2Int end)
        {
            List<Vector2Int> path = GridBuildingSystem.Instance.FindPath(dragStartCell, end, currentBuildingDatas);

            currentPath.Clear();
            if (path != null) currentPath = path;

            while (pathGhosts.Count < currentPath.Count)
                pathGhosts.Add(Ghost.Create(currentBuildingDatas.Ghost));

            while (pathGhosts.Count > currentPath.Count)
            {
                int lastIndex = pathGhosts.Count - 1;
                pathGhosts[lastIndex].Destroy();
                pathGhosts.RemoveAt(lastIndex);
            }

            for (int i = 0; i < currentPath.Count; i++)
                pathGhosts[i].SetPosition(path[i]);
        }

        private void ClearPathGhosts()
        {
            foreach (Ghost pathGhost in pathGhosts)
                pathGhost.Destroy();

            pathGhosts.Clear();
            currentPath.Clear();
        }
    }
}