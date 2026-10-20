using ArTiX.FactoryGame.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ArTiX.Utils.Window
{
    public abstract class ObjectWindow<T> : BaseWindow
    {
        public enum EWindowType
        {
            Default,
            Convertor,
            Grabber,
            Inventory
        }

        public static ObjectWindow<T> Create(in EWindowType windowType, in T owner, in string title = null)
        {
            string windowPath;
            switch (windowType)
            {
                default:
                case EWindowType.Default:
                    windowPath = "Assets/_PROJECT/Shared/Window/prfb_BaseWindow.prefab";
                    break;
                case EWindowType.Convertor:
                    windowPath = "Assets/_PROJECT/Features/GridBuildingSystem/Buildings/PlayerToBuild/Convertor/Window/prfb_ConvertorWindow.prefab";
                    break;
                case EWindowType.Grabber:
                    windowPath = "Assets/_PROJECT/Features/GridBuildingSystem/Buildings/PlayerToBuild/Grabber/Window/prfb_GrabberWindow.prefab";
                    break;
                case EWindowType.Inventory:
                    windowPath = "Assets/_PROJECT/Shared/Inventory/Window/prfb_InventoryWindow.prefab";
                    break;
            }

            GameObject windowObj = AssetDatabase.LoadAssetAtPath<GameObject>(windowPath);
            windowObj = Instantiate(windowObj, HUD.Instance.transform);
            ObjectWindow<T> window = windowObj.GetComponent<ObjectWindow<T>>();
            window.owner = owner;
            window.Initialize();

            if (title != null)
                window.title.text = title;

            return window;
        }

        [SerializeField] private TextMeshProUGUI title;

        protected T owner;

        protected abstract void Initialize();
    }
}