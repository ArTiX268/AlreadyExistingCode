using ArTiX.Effects.Tween;
using ArTiX.GridBuildingSystem.Datas;
using System;
using ArTiX.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.GridBuildingSystem
{
    public class HUD : MonoBehaviour
    {
        private static HUD instance;
        public static HUD Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject(nameof(HUD), components: typeof(HUD)).GetComponent<HUD>();
                
                return instance;
            }
        }

        [SerializeField] private HUDDatas datas;

        [SerializeField] private Transform buildingPanel;
        [SerializeField] private Transform buildingBtnsHolder;

        private const float HEIGHT_BUILDING_PANEL_ACTIVE = 100;
        private const float HEIGHT_BUILDING_PANEL_INACTIVE = -100;

        private bool isBuildingPanelActive;

        private MasterTween.Tween tween;

        public event Action<PlacedObjectDatas> OnBuildingSelected;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            Button<PlacedObjectDatas> btn = null;
            foreach (PlacedObjectDatas buildingDatas in datas.PlacedObjectsDatas)
            {
                btn = Instantiate(datas.PrfbBtn, buildingBtnsHolder);
                btn.GetComponent<Image>().sprite = buildingDatas.Icon;
                btn.GetComponentInChildren<TextMeshProUGUI>().text = buildingDatas.Name;
                btn.OnClick += OnBuildingBtnClicked;
                btn.Setup(buildingDatas);
            }
        }

        private void Start()
        {
            InputManager.Instance.OnToggleBuildingPanel += ToggleBuildingPanel;
        }

        private void OnBuildingBtnClicked(PlacedObjectDatas buildingDatas)
        {
            OnBuildingSelected?.Invoke(buildingDatas);
        }

        public void ToggleBuildingPanel()
        {
            isBuildingPanelActive = !isBuildingPanelActive;

            tween?.Kill();
            tween = MasterTween.Create();
            buildingPanel.TweenPosition(
                targetPos: new Vector3(
                    x: buildingPanel.position.x, 
                    y: isBuildingPanelActive ? HEIGHT_BUILDING_PANEL_ACTIVE : HEIGHT_BUILDING_PANEL_INACTIVE),
                animParams: datas.BuildingPanelAnim
                );
        }
    }
}