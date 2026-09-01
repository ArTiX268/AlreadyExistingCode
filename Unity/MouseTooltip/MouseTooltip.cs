using ArTiX.FactoryGame.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ArTiX.Utils
{
    public class MouseTooltip : MonoBehaviour
    {
        [SerializeField] private RectTransform prefabTooltip;
        [SerializeField] private Font font;
        [SerializeField] private Vector2 backgroundSizeOffset;

        private RectTransform tooltipObj;

        private readonly TextGenerator textGenerator = new TextGenerator();
        private List<RaycastResult> uiRaycastResults;
        private TextGenerationSettings settings;

        private IMouseTooltip tooltip;

        private void Start()
        {
            tooltipObj = Instantiate(prefabTooltip, HUD.Instance.transform);
            tooltipObj.gameObject.SetActive(false);

            TextMeshProUGUI textObj = tooltipObj.GetComponentInChildren<TextMeshProUGUI>();
            settings = new TextGenerationSettings
            {
                fontSize = (int)textObj.fontSize,
                font = font,
                verticalOverflow = VerticalWrapMode.Overflow
            };
        }

        private void Update()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            IMouseTooltip mouseTooltip = null;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Check for UI tooltip
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = mouseScreenPos
                };

                uiRaycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, uiRaycastResults);
                foreach (RaycastResult result in uiRaycastResults)
                {
                    if (result.gameObject.transform.IsChildOf(tooltipObj)) continue;

                    mouseTooltip = result.gameObject.GetComponentInParent<IMouseTooltip>();

                    if (mouseTooltip != null) break;
                }
            }   
            // Check for 3D object tootlip
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseScreenPos), out RaycastHit hit, float.MaxValue))
            {
                hit.collider.TryGetComponent(out mouseTooltip);
            }

            if (mouseTooltip == null || mouseTooltip.Tooltip == "")
            {
                if (tooltip == null) return;
                tooltipObj.gameObject.SetActive(false);
                tooltip = null;
                return;
            }

            if (mouseTooltip != tooltip)
            {
                tooltipObj.gameObject.SetActive(true);
                tooltip = mouseTooltip;

                // Update text container size
                TextMeshProUGUI textObj = tooltipObj.GetComponentInChildren<TextMeshProUGUI>();
                textObj.text = tooltip.Tooltip;
                textObj.rectTransform.sizeDelta = backgroundSizeOffset + new Vector2(
                    x: textGenerator.GetPreferredWidth(tooltip.Tooltip, settings),
                    y: textGenerator.GetPreferredHeight(tooltip.Tooltip, settings));

                LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipObj);
            }

            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            tooltipObj.anchorMax = tooltipObj.anchorMin = mouseScreenPos / screenSize;
            tooltipObj.anchoredPosition = tooltipObj.sizeDelta / 2;
            tooltipObj.SetAsLastSibling(); // Otherwise, other objects would be in front
        }
    }
}