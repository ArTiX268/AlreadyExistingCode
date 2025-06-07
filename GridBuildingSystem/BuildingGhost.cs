using System.Collections.Generic;
using UnityEngine;
using static PlacedObjectTypeSO;

public class BuildingGhost : MonoBehaviour {

    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private void Start() {
        RefreshVisual();

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetPosition + placedObjectTypeSO.offset, Time.deltaTime * 15f), Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f));

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f))
        {
            GridBuildingSystem.Instance.grid.GetXZ(hit.point, out int x, out int z);

            List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), GridBuildingSystem.Instance.dir);
            if (GridBuildingSystem.Instance.grid.GetGridObject(hit.point).CanBuild() && GridBuildingSystem.Instance.CanBuild(gridPositionList))
                visual.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
            else
                visual.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        }
    }

    private void RefreshVisual()
    {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null) {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 3);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}

