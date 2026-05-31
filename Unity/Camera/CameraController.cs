using ArTiX.Effects;
using ArTiX.Utils;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance == null) instance = new CameraController();
            return instance;
        }
    }

    [SerializeField, Tooltip("The point it tries to reach.")] private Transform anchor;
    [SerializeField] private CameraDatas datas;

    private bool lockX;
    private bool lockY;

    [SerializeField] private float zoom = 1;
    private float startingZoom;
    private float targetZoom;
    private float zoomTimer;
    private bool changeZoom;

    private Camera cam;

    private void Awake()
    {
        instance = this;

        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetZoom(0.5f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetZoom(1f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetZoom(1.5f);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetZoom(2f);

        // POSITION
        Vector3 targetPos = anchor.position;

        #region Box Limits

        if (datas.usesBoxLimits) // draw box
        {
            Vector2[,] worldBoxBoundaries = datas.GetBoxLimits();
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    worldBoxBoundaries[x, y] = new Vector2(worldBoxBoundaries[x, y].x * Screen.width, worldBoxBoundaries[x, y].y * Screen.height);
                    worldBoxBoundaries[x, y] = cam.ScreenToWorldPoint(worldBoxBoundaries[x, y]);
                }
            }
            cam = GetComponent<Camera>();

#if UNITY_EDITOR // Draw a rectangle

            if (datas.drawBoxLimits)
            {
                Debug.DrawLine(worldBoxBoundaries[0, 0], worldBoxBoundaries[1, 0], Color.red, 0.01f);
                Debug.DrawLine(worldBoxBoundaries[1, 0], worldBoxBoundaries[1, 1], Color.red, 0.01f);
                Debug.DrawLine(worldBoxBoundaries[1, 1], worldBoxBoundaries[0, 1], Color.red, 0.01f);
                Debug.DrawLine(worldBoxBoundaries[0, 1], worldBoxBoundaries[0, 0], Color.red, 0.01f);
            }
#endif
            // Move to make the anchor inside the box
            Vector3 targetAnchorPos = new Vector3
            {
                x = Mathf.Clamp(targetPos.x, worldBoxBoundaries[0, 0].x, worldBoxBoundaries[1, 0].x),
                y = Mathf.Clamp(targetPos.y, worldBoxBoundaries[0, 0].y, worldBoxBoundaries[0, 1].y)
            };

            if (datas.drawTargetAnchorCross)
                MyDebug.DrawCrossXY(targetAnchorPos, datas.targetAnchorCrossColor, 0.01f, datas.targetAnchorCrossSize);

            targetPos = transform.position + targetPos - targetAnchorPos;
        }

        #endregion

        #region Influence points

        if (datas.useInfluencePoints)
        {
            Vector3 totalPos = Vector3.zero;
            float totalWeight = 0;
            Vector2 cameraPos = new Vector2(transform.position.x, transform.position.y);

            float weight;
            foreach (InfluencePoint point in InfluencePoint.instances)
            {
                weight = point.GetWeight(anchor.position);
                if (weight == 0) continue;

                totalWeight += weight;
                totalPos += point.transform.position * weight;
            }

            if (totalWeight != 0)
                targetPos = totalPos / totalWeight;
        }

        #endregion

        #region Displacment

        Vector3 nextPos = transform.position;
        if (!lockX)
        {
            if (datas.smoothX)
                nextPos.x = Mathf.Lerp(transform.position.x, targetPos.x, Time.deltaTime * datas.xSmoothSpeed);
            else
                nextPos.x = targetPos.x;
        }

        if (!lockY)
        {
            if (datas.smoothY)
                nextPos.y = Mathf.Lerp(transform.position.y, targetPos.y, Time.deltaTime * datas.ySmoothSpeed);
            else
                nextPos.y = targetPos.y;
        }

        transform.position = nextPos;

        #endregion

        #region Zoom

        if (changeZoom)
        {
            zoomTimer = Mathf.Clamp(zoomTimer + Time.deltaTime, 0, datas.zoomAnim.duration);
            zoom = Tween.InterpolateValue(startingZoom, targetZoom, zoomTimer, datas.zoomAnim);

            if (zoomTimer == datas.zoomAnim.duration)
                changeZoom = false;
        }

        if (cam.orthographicSize != datas.defaultSize / zoom)
            cam.orthographicSize = datas.defaultSize / zoom;

        #endregion

        #region Debug

#if UNITY_EDITOR
        if (datas.drawAnchorCross)
            MyDebug.DrawCrossXY(anchor.position, datas.anchorCrossColor, 0.01f, datas.anchorCrossSize);
        if (datas.drawFocusCross)
            MyDebug.DrawCrossXY(new Vector3(transform.position.x, transform.position.y), datas.focusCrossColor, 0.01f, datas.focusCrossSize);
        if (datas.drawTargetCross)
            MyDebug.DrawCrossXY(targetPos, datas.targetCrossColor, 0.01f, datas.targetCrossSize);
#endif
        #endregion

    }

    public void ToggleLockX(bool lockX)
    {
        if (datas.allowLockX)
            this.lockX = lockX;
    }

    public void ToggleLockY(bool lockY)
    {
        if (datas.allowLockY)
            this.lockY = lockY;
    }

    public void SetZoom(float zoom)
    {
        zoomTimer = 0;
        startingZoom = this.zoom;
        targetZoom = Mathf.Clamp(zoom, datas.minZoom, datas.maxZoom);
        changeZoom = true;
    }
}