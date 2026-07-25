using ArTiX.Effects.Tween;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.GridBuildingSystem.Datas
{
    [CreateAssetMenu(fileName = "HudDatas", menuName = "Datas/Hud")]
    public class HUDDatas : ScriptableObject
    {
        [SerializeField] private MasterTween.AnimParams buildingPanelAnim;
        public MasterTween.AnimParams BuildingPanelAnim => buildingPanelAnim;

        [SerializeField] private Button<PlacedObjectDatas> prfbBtn;
        public Button<PlacedObjectDatas> PrfbBtn => prfbBtn;

        [SerializeField] private List<PlacedObjectDatas> placedObjectsDatas;
        public List<PlacedObjectDatas> PlacedObjectsDatas => placedObjectsDatas;
    }
}