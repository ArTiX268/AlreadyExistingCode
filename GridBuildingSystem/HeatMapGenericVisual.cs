using UnityEngine;

public class HeatMapGenericVisual : MonoBehaviour
{
    private Grid<HeatMapGridObject> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid<HeatMapGridObject> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();

        //grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    //private void Grid_OnGridValueChanged(object sender, Grid<HeatMapGridObject>.OnGridValueChangedEventArgs e)
    //{
    //    UpdateHeatMapVisual();
    //    updateMesh = true;
    //}

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1f, 1f) * grid.GetCellSize();

                HeatMapGridObject gridObject = grid.GetGridObject(x, y);
                float gridValueNormalized = gridObject.GetValueNormalized();
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0);
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
