using ArTiX.GridBuildingSystem.Datas;
using ArTiX.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.GridBuildingSystem.Tools
{
    [ExecuteInEditMode, RequireComponent(typeof(PlayerInput))]
    public class SetupGridValue : MonoBehaviour
    {
        private static SetupGridValue instance;
        public static SetupGridValue Instance
        {
            get
            {
                if (instance == null) 
                    instance = new GameObject("SetupGridValue", typeof(SetupGridValue)).GetComponent<SetupGridValue>();
                
                return instance;
            }
            private set => instance = value;
        }

        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private SpriteRenderer prefabCell;

        private const float SPACE_BETWEEN_CELLS = 1;

        private readonly Color selectedColor = Color.green;
        private readonly Color unselectedColor = Color.white;

        [SerializeField] private PlacedObjectSO building;
        public PlacedObjectSO Building
        {
            set
            {
                building = value;
            }
        }

        public string previousScenePath;

        private struct SCell
        {
            public SpriteRenderer sprite;
            public Vector2Int cellValue;
            public bool active;
        }

        private Dictionary<SpriteRenderer, Vector2Int> spriteCellDic = new Dictionary<SpriteRenderer, Vector2Int>();

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UpdateGridVisual();
        }

        public void ClickOnGrid(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                {
                    SpriteRenderer hitRenderer = hit.collider.GetComponent<SpriteRenderer>();
                    hitRenderer.color = hitRenderer.color == unselectedColor ? selectedColor : unselectedColor;
                }
            }
        }

        private void UpdateGridVisual()
        {
            spriteCellDic.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            List<Vector2Int> currentCells = building.CellArray?.ConvertToList();
            currentCells ??= new List<Vector2Int>();
            if (currentCells.Count == 0) currentCells.Add(Vector2Int.zero);

            Vector2Int middleCell = gridSize / 2;
            int y;
            Vector2Int checkedCell;
            SpriteRenderer cellSprite;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (y = 0; y < gridSize.y; y++)
                {
                    checkedCell = new Vector2Int(x, y) - middleCell;
                    cellSprite = Instantiate(prefabCell,
                        position: new Vector3(checkedCell.x, checkedCell.y) * SPACE_BETWEEN_CELLS,
                        rotation: Quaternion.identity,
                        parent: transform);

                    cellSprite.color = currentCells.Contains(checkedCell) ? selectedColor : unselectedColor;

                    spriteCellDic.Add(cellSprite, checkedCell);
                }
            }
        }

        public void Validate()
        {
            List<Vector2Int> currentCells = new List<Vector2Int>();

            foreach (KeyValuePair<SpriteRenderer, Vector2Int> spriteCellPair in spriteCellDic)
            {
                if (spriteCellPair.Key.color == selectedColor) 
                    currentCells.Add(spriteCellPair.Value);
            }

            building.CellArray = currentCells.ToArray();
            EditorApplication.ExitPlaymode();
            EditorApplication.playModeStateChanged += OnClosingPlayMode;
        }

        private void OnClosingPlayMode(PlayModeStateChange obj)
        {
            EditorApplication.playModeStateChanged -= OnClosingPlayMode;
            EditorSceneManager.OpenScene(previousScenePath);
        }
    }
}