using ArTiX.Effects;
using UnityEngine;

/// <summary>
/// The fields of this class must not be modified in code. They are just data holders.
/// </summary>
[CreateAssetMenu(fileName = "CameraDatas", menuName = "Scriptable Objects/CameraDatas")]
public class CameraDatas : ScriptableObject
{
    public float defaultSize = 5;

    [Header("Smooth")]
    public bool smoothX;
    public bool smoothY;
    [Min(0)] public float xSmoothSpeed;
    [Min(0)] public float ySmoothSpeed;

    [Header("Lock")]
    public bool allowLockX;
    public bool allowLockY;

    [Header("Zoom")]
    [Range(0.5f, 3f)] public float minZoom = 1;
    [Range(0.5f, 6f)] public float maxZoom = 1;
    [Tooltip("Do not use the Arch transitions otherwise you won't reach your target value.")] public Tween.AnimParams zoomAnim;

    [Header("InfluencePoints")]
    [Tooltip("Using influence points will override the box limits.")] public bool useInfluencePoints;

    [Header("BoxLimits")]
    [Tooltip("If true, the camera will move only if the target is outside the bounds of the box")] public bool usesBoxLimits;
    public bool drawBoxLimits;
    [SerializeField, Range(0, 1)]
    private float boxWidth;
    [SerializeField, Range(0, 1)]
    private float boxHeight;
    [SerializeField] private Vector2 boxOffset;

    private Vector2[,] boxLimits;

    public Vector2[,] GetBoxLimits()
    {
#if UNITY_RUNTIME
        if (boxLimits == null)
#endif
        {
            float halfWidth = boxWidth * 0.5f;
            float halfHeight = boxHeight * 0.5f;
            boxLimits = new Vector2[2, 2];
            boxLimits[0, 0] = new Vector2(boxOffset.x - halfWidth, boxOffset.y - halfHeight) + Vector2.one * 0.5f;
            boxLimits[1, 0] = new Vector2(boxOffset.x + halfWidth, boxOffset.y - halfHeight) + Vector2.one * 0.5f;
            boxLimits[0, 1] = new Vector2(boxOffset.x - halfWidth, boxOffset.y + halfHeight) + Vector2.one * 0.5f;
            boxLimits[1, 1] = new Vector2(boxOffset.x + halfWidth, boxOffset.y + halfHeight) + Vector2.one * 0.5f;
        }
        return boxLimits;
    }

    [Header("Debug")]
    public bool drawAnchorCross;
    public Color anchorCrossColor;
    public float anchorCrossSize;

    public bool drawTargetAnchorCross;
    public Color targetAnchorCrossColor;
    public float targetAnchorCrossSize;

    public bool drawTargetCross;
    public Color targetCrossColor;
    public float targetCrossSize;

    public bool drawFocusCross;
    public Color focusCrossColor;
    public float focusCrossSize;
}
