using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "GridDatas", menuName = "Datas/Grid")]
    public class GridDatas : ScriptableObject
    {
        public const string WIDTH = "_Width";
        public const string HEIGHT = "_Height";
        public const string CELL_SIZE = "_CellSize";
        public const string LINE_WIDTH = "_LineWidth";

        [Title("Grid")]
        [SerializeField, Min(0)] private int width;
        public int Width => width;

        [SerializeField, Min(0)] private int height;
        public int Height => height;

        [SerializeField, Min(0)] private float cellSize;
        public float CellSize => cellSize;

        [SerializeField, Min(0)] private float lineWidth;
        public float LineWidth => lineWidth;

        [SerializeField] private Color color = Color.white;
        public Color Color => color;

        [SerializeField] private Vector3 origin;
        public Vector3 Origin => origin;

        [SerializeField] private GameObject prfbGridVisual;
        public GameObject PrfbGridVisual => prfbGridVisual;

        [Title("Building Ghost")]
        [SerializeField] private Color validColor;
        public Color ValidColor => validColor;

        [SerializeField] private Color unvalidColor;
        public Color UnvalidColor => unvalidColor;
    }
}