using ArTiX.Effects;
using ArTiX.Utils;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public struct ZoomAnim
    {
        public Tween.AnimParams zoomInParams;
        public float holdDuration;
        public Tween.AnimParams zoomOutParams;
    }

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

    [SerializeField] private float currentZoom = 1;
    private Tween zoomTween;

    private Camera cam;

    private delegate void DoMoveCamera(ref Vector3 targetPos);
    private DoMoveCamera doBoxLimit;
    private DoMoveCamera doInfluencePoint;

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

        if (zoomTween == null || !zoomTween.IsActive)
        {
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
        }

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
            SetZoom(totalZoom, new Tween.AnimParams
            {
                duration = 1,
                transition = Tween.ETransition.SmoothStep5
            });
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

    public void SetZoom(float zoom, in Tween.AnimParams zoomAnim)
    {
        if (zoomTween == null) zoomTween = Tween.Create("Zoom Tween", false);
        if (zoomTween.IsActive) zoomTween.Clear();

        zoomTween.TweenEvent(currentZoom, zoom, TweenZoom, zoomAnim, timeScaled: true);
    }

    public void ZoomOnPoint(Vector3 point, in float zoomLevel, in ZoomAnim zoomAnim)
    {
        if (zoomTween == null) zoomTween = Tween.Create("Zoom Tween", false);
        if (zoomTween.IsActive) zoomTween.Clear();

        point.z = transform.position.z;
        zoomTween.TweenEvent(currentZoom, zoomLevel, TweenZoom, zoomAnim.zoomInParams, timeScaled: true);
        zoomTween.TweenEvent(transform.position, point, TweenPos, zoomAnim.zoomInParams, timeScaled: true, parrallel: true);

        zoomTween.TweenEvent(zoomLevel, currentZoom, TweenZoom, zoomAnim.zoomOutParams, timeScaled: true, delay: zoomAnim.holdDuration);
        zoomTween.TweenEvent(point, transform.position, TweenPos, zoomAnim.zoomOutParams, timeScaled: true, parrallel: true);
    }

    private void TweenZoom(float newZoom) => cam.orthographicSize = datas.defaultSize / newZoom;

    private void TweenPos(Vector3 newPos) => transform.position = newPos;
}