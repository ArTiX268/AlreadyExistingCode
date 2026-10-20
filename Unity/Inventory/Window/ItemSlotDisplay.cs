using ArTiX.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArTiX.Inventory.Inventory;

namespace ArTiX.Inventory.UI
{
    public class ItemSlotDisplay : MonoBehaviour, IMouseTooltip
    {
        [SerializeField] private Image icon;

        private string itemName = "";
        public string Tooltip => itemName;

        public void SetItemSlot(ItemSlot slot)
        {
            itemName = slot.Item.Name;
            icon.sprite = slot.Item.Icon;
            GetComponentInChildren<TextMeshProUGUI>().text = slot.Quantity.ToString();
        }
    }
}