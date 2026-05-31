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

    private Vector2 smoothSpeed;
    private float smoothSpeedTimer;

    public bool lockX;
    public bool lockY;

    [SerializeField] private float zoom = 1;
    private float startingZoom;
    private float targetZoom;
    private float zoomTimer;

    private Camera cam;

    private delegate void DoMoveCamera(ref Vector3 targetPos);
    private DoMoveCamera doBoxLimit;
    private DoMoveCamera doInfluencePoint;

    private delegate void DoZoom();
    private DoZoom doZoom;

    private void Awake()
    {
        instance = this;

        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (datas.usesBoxLimits) doBoxLimit += BoxLimit;
        if (InfluencePoint.instances.Count > 0) doInfluencePoint += InfluencePoints;
    }

    private void Update()
    {
        // POSITION
        Vector3 targetPos = anchor.position;

        doBoxLimit?.Invoke(ref targetPos);
        doInfluencePoint?.Invoke(ref targetPos);

        #region Displacment

        Vector3 nextPos = transform.position;
        if (Vector2.Distance(nextPos, targetPos) > datas.distanceThreshold)
        {
            if (smoothSpeedTimer != datas.reachingMaxSmoothSpeedAnim.duration)
            {
                smoothSpeedTimer += Time.deltaTime;
                smoothSpeedTimer = Mathf.Clamp(smoothSpeedTimer, 0, datas.reachingMaxSmoothSpeedAnim.duration);
                smoothSpeed = Tween.InterpolateValue(datas.minSmoothSpeed, datas.maxSmoothSpeed, smoothSpeedTimer, datas.reachingMaxSmoothSpeedAnim);
            }
        }
        else
        {
            smoothSpeedTimer = 0;
            smoothSpeed = datas.minSmoothSpeed;
        }

        if (!lockX)
            nextPos.x = SmoothCam(datas.smoothX, transform.position.x, smoothSpeed.x, targetPos.x);

        if (!lockY)
            nextPos.y = SmoothCam(datas.smoothY, transform.position.y, smoothSpeed.y, targetPos.y);

        transform.position = nextPos;

        #endregion

        doZoom?.Invoke();

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

    private void BoxLimit(ref Vector3 targetPos)
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

    private void InfluencePoints(ref Vector3 targetPos)
    {
        Vector3 totalPos = Vector3.zero;
        float totalWeight = 0;
        float totalZoom = 0;

        float weight;
        foreach (InfluencePoint point in InfluencePoint.instances)
        {
            weight = point.GetWeight(anchor.position);
            if (weight == 0) continue;

            totalZoom += point.GetZoom() * weight;
            totalWeight += weight;
            totalPos += point.transform.position * weight;
        }

        if (totalWeight != 0)
        {
            totalZoom /= totalWeight;
            SetZoom(totalZoom);
            targetPos = totalPos / totalWeight;
        }
    }

    private float SmoothCam(in bool smooth, in float startPos, in float smoothSpeed, in float targetPos)
    {
        if (smooth)
            return Mathf.Lerp(startPos, targetPos, Time.deltaTime * smoothSpeed);
        else
            return targetPos;
    }

    public void SetZoom(float zoom)
    {
        zoomTimer = 0;
        startingZoom = this.zoom;
        targetZoom = Mathf.Clamp(zoom, datas.minZoom, datas.maxZoom);
        doZoom += Zoom;
    }

    private void Zoom()
    {
        zoomTimer = Mathf.Clamp(zoomTimer + Time.deltaTime, 0, datas.zoomAnim.duration);
        zoom = Tween.InterpolateValue(startingZoom, targetZoom, zoomTimer, datas.zoomAnim);

        if (zoomTimer == datas.zoomAnim.duration) doZoom -= Zoom;

        if (cam.orthographicSize != datas.defaultSize / zoom)
            cam.orthographicSize = datas.defaultSize / zoom;
    }
}