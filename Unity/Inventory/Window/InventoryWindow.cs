using ArTiX.Utils.Window;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Inventory.UI
{
    public class InventoryWindow : ObjectWindow<Inventory>
    {
        [SerializeField] private Transform resourceDisplayContainer;
        [SerializeField] private ItemSlotDisplay prefabResourceDisplay;

        private readonly Dictionary<ItemSO, ItemSlotDisplay> resourceDisplayDictionary = 
            new Dictionary<ItemSO, ItemSlotDisplay>();

        protected override void Initialize() 
        {
            owner.OnAddItem += DisplayAddedItem;
            owner.OnRemoveItem += OnRemoveItemFromInventory;
            foreach (KeyValuePair<ItemSO, int> slot in owner.GetInventory())
            {
                DisplayAddedItem(this, new Inventory.ItemSlot(slot.Key, slot.Value));
            }
        }

        private void DisplayAddedItem(object sender, Inventory.ItemSlot itemSlot)
        {
            if (!resourceDisplayDictionary.ContainsKey(itemSlot.Item))
            {
                resourceDisplayDictionary.Add(
                    key: itemSlot.Item, 
                    value: Instantiate(prefabResourceDisplay, resourceDisplayContainer));
            }

            resourceDisplayDictionary[itemSlot.Item].SetItemSlot(itemSlot);
        }
        
        private void OnRemoveItemFromInventory(object sender, Inventory.ItemSlot itemSlot)
        {
            if (itemSlot.Quantity <= 0)
            {
                Destroy(resourceDisplayDictionary[itemSlot.Item].gameObject);
                resourceDisplayDictionary.Remove(itemSlot.Item);
            }
            else
                resourceDisplayDictionary[itemSlot.Item].SetItemSlot(itemSlot);
        }

        public override void CloseWindow()
        {
            owner.OnAddItem -= DisplayAddedItem;
            owner.OnRemoveItem -= OnRemoveItemFromInventory;
            base.CloseWindow(); 
        }
    }
}