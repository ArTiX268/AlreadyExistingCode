using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX
{
    [RequireComponent(typeof(Button))]
    public class Button<T> : MonoBehaviour
    {
        public event Action<T> OnClick;

        private T value;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        public void Setup(T value) => this.value = value;

        private void OnClicked()
        {
            OnClick?.Invoke(value);
        }
    }
}