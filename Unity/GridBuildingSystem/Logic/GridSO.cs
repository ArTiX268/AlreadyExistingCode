using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "GridDatas", menuName = "Datas/Grid")]
    public class GridSO : ScriptableObject
    {
        public const string WIDTH = "_Width";
        public const string HEIGHT = "_Height";
        public const string CELL_SIZE = "_CellSize";
        public const string LINE_WIDTH = "_LineWidth";
        public const string OCCUPANCY_MAP = "_OccupancyMap";

        [Title("Grid")]
        [SerializeField, Min(0)] private int width = 10;
        public int Width => width;

        [SerializeField, Min(0)] private int height = 10;
        public int Height => height;

        [SerializeField, Min(0)] private float cellSize = 1;
        public float CellSize => cellSize;

        [SerializeField, Min(0)] private float lineWidth = 0.002f;
        public float LineWidth => lineWidth;

        [SerializeField] private Color color = Color.white;
        public Color Color => color;

        [SerializeField] private Vector3 origin;
        public Vector3 Origin => origin;

        [SerializeField] private GameObject prfbGridVisual;
        public GameObject PrfbGridVisual => prfbGridVisual;

        [field: SerializeField] public ParticleSystem PrefabSpawnSmoke { get; private set; }
    }
}