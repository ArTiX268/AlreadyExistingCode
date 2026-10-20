using UnityEngine;

namespace ArTiX.Inventory
{
    [CreateAssetMenu(fileName = "InventorySO", menuName = "Datas/Inventory/InventorySO")]
    public class InventorySO : ScriptableObject
    {
        [SerializeField, Min(1)] private int nbMaxDifferentItem;
        public int NbMaxDifferentItem => nbMaxDifferentItem;
    }
}