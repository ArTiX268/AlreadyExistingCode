using ArTiX.GridBuildingSystem.Datas;
using UnityEngine;

namespace ArTiX.GridBuildingSystem
{
    public class PlacedObject : MonoBehaviour
    {
        [SerializeField] private PlacedObjectDatas datas;
        public PlacedObjectDatas Datas => datas;
    }
}