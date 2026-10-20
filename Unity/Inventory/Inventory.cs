using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Inventory
{
    public class Inventory : MonoBehaviour, IItemHolder
    {
        public class ItemSlot
        {
            public ItemSO Item {get; private set;}
            public int Quantity { get; private set; }

            public ItemSlot(ItemSO item, int quantity)
            {
                Item = item;
                Quantity = quantity;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="quantity">quantity will keep the value of the number of item it couldn't take.</param>
            public void AddItem(ref int quantity)
            {
                if (Quantity + quantity <= Item.MaxStackSize)
                {
                    Quantity += quantity;
                    quantity = 0;
                }
                else
                {
                    quantity -= Item.MaxStackSize - Quantity;
                    Quantity = Item.MaxStackSize;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="quantity">quantity will keep the value of the number of item it couldn't remove.</param>
            public void RemoveItem(ref int quantity)
            {
                if (Quantity >= quantity)
                {
                    Quantity -= quantity;
                    quantity = 0;
                }
                else
                {
                    quantity -= Quantity;
                    Quantity = 0;
                }
            }
        }

        [SerializeField] private InventorySO datas;

        private readonly List<ItemSlot> inventory = new List<ItemSlot>();

        public event EventHandler<ItemSlot> OnAddItem;
        public event EventHandler<ItemSlot> OnRemoveItem;

        public ItemSO Item
        {
            get
            {
                if (inventory.Count == 0) return null;
                return inventory[0].Item;
            }
        }

        public bool CanGiveItem(ItemSO item = null, int quantity = 1) 
        {
            if (item == null) return Item != null;

            foreach (ItemSlot slot in inventory)
            {
                if (slot.Item == item && slot.Quantity >= quantity) return true;
            }

            return false;
        }
        public ItemSO GiveItem(ItemSO item = null, int quantity = 1)
        {
            ItemSlot slot = null;

            if (item == null) slot = inventory[0];
            else
            {
                foreach (ItemSlot itemSlot in inventory)
                {
                    if (itemSlot.Item == item)
                    {
                        slot = itemSlot;
                        break;
                    }
                }
            }

            slot.RemoveItem(ref quantity);
            item = slot.Item;
            if (slot.Quantity <= 0) 
            {
                inventory.Remove(slot);
            }

            OnRemoveItem?.Invoke(this, slot);
            return item;
        }

        public bool CanTakeItem(ItemSO item)
        {
            if (item == null) return false;

            foreach (ItemSlot slot in inventory)
            {
                if (slot.Item == item && slot.Quantity < item.MaxStackSize) return true;
            }

            if (inventory.Count < datas.NbMaxDifferentItem) return true;

            return false;
        }
        public void TakeItem(ItemSO item)
        {
            int inventorySize = inventory.Count;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i].Item == item)
                {
                    if (inventory[i].Quantity == item.MaxStackSize) continue;

                    int quantity = 1;
                    inventory[i].AddItem(ref quantity);
                    OnAddItem?.Invoke(this, inventory[i]);
                    return;
                }
            }

            inventory.Add(new ItemSlot(item, 1));

            OnAddItem?.Invoke(this, inventory[inventory.Count - 1]);
        }

        public Dictionary<ItemSO, int> GetInventory()
        {
            Dictionary<ItemSO, int> inventoryDictionary = new Dictionary<ItemSO, int>();
            foreach (ItemSlot slot in inventory)
            {
                inventoryDictionary.Add(slot.Item, slot.Quantity);
            }

            return inventoryDictionary;
        }
    }
}