using Sirenix.OdinInspector;
using UnityEngine;

namespace ArTiX.Interaction
{
    [CreateAssetMenu(fileName = "so_Interactor", menuName = "Datas/InteractorSO")]
    public class InteractorSO : ScriptableObject
    {
        [SerializeField] private LayerMask interactionMask;
        public LayerMask InteractionMask => interactionMask;

        [SerializeField] private bool infinite;
        [SerializeField, HideIf("infinite")] private float interactionDistance;
        public float InteractionDistance 
        {
            get
            {
                return infinite ? float.MaxValue : interactionDistance;
            }
        }

        [SerializeField] private float interactionRadius;
        public float InteractionRadius => interactionRadius;

        [SerializeField] private Outline.Mode outlineMode;
        public Outline.Mode OutlineMode => outlineMode;

        [SerializeField] private Color outlineColor = Color.white;
        public Color OutlineColor => outlineColor;

        [SerializeField, Range(0f, 10f)] private float outlineWidth = 2f;
        public float OutlineWidth => outlineWidth;
    }
}