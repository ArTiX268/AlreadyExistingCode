using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.Utils
{
    [RequireComponent(typeof(Button))]
    public class SpriteToggle : MonoBehaviour
    {
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Image image;

        private bool isOn;
        public bool IsOn
        {
            get => isOn;
            set
            {
                if (isOn == value) return;
                isOn = value;
                image.sprite = isOn ? onSprite : offSprite;
            }
        }

        private void Awake()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            IsOn = !IsOn;
        }
    }
}