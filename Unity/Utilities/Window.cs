using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Window : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;

    public static Window Create(RectTransform parent, Vector2 position, Vector2 size)
    {
        GameObject prefab = Resources.Load<GameObject>("Window");

        return prefab.AddComponent<Window>();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData) 
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
}
