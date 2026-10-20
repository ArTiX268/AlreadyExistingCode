using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArTiX.Utils
{
    public sealed class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField, Required] private RectTransform draggedWindow;
        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            if (canvas == null)
            {
                Transform currentTransform = transform.parent;
                while (canvas == null)
                {
                    if (!currentTransform.TryGetComponent<Canvas>(out canvas))
                        currentTransform = currentTransform.parent;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            draggedWindow.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            draggedWindow.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}