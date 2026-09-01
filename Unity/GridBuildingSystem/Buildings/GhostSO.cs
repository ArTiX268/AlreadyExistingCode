using ArTiX.Effects.Tween;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "GhostSO", menuName = "Datas/GhostSO")]
    public class GhostSO : ScriptableObject
    {
        [SerializeField] private Tween.AnimParams positionAnim;
        public Tween.AnimParams PositionAnim => positionAnim;

        [SerializeField] private Tween.AnimParams rotationAnim;
        public Tween.AnimParams RotationAnim => rotationAnim;

        [SerializeField] private Tween.AnimParams colorChangeAnim;
        public Tween.AnimParams ColorChangeAnim => colorChangeAnim;

        [SerializeField] private Color validColor = Color.green;
        public Color ValidColor => validColor;

        [SerializeField] private Color unvalidColor = Color.red;
        public Color UnvalidColor => unvalidColor;
    }
}