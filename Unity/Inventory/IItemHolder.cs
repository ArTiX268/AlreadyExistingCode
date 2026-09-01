namespace ArTiX.Inventory
{
    public interface IItemHolder
    {
        public ItemSO Item { get; }
        public ItemSO GiveItem(ItemSO item = null, int quantity = 1);
        public bool CanGiveItem(ItemSO item = null, int quantity = 1);
        public void TakeItem(ItemSO item);
        public bool CanTakeItem(ItemSO item);
    }
}