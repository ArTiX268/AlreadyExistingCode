using UnityEngine;

namespace ArTiX.Utils.Window
{
    public class BaseWindow : MonoBehaviour
    {
        public void Focus()
        {
            transform.SetAsLastSibling();
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.one * .5f;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public virtual void CloseWindow()
        {
            Destroy(gameObject);
        }
    }
}