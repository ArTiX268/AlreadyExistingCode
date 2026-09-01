using ArTiX.Utils.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArTiX.Utils
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonExtension<T> : MonoBehaviour
    {
        public event Action<T> OnClickEvent;
        protected T parameter;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public virtual void AddClickEvent(Action<T> onClickEvent, in T parameter)
        {
            OnClickEvent += onClickEvent;
            this.parameter = parameter;
        }

        public void PlayClickSound(AudioDatasSO datas) => AudioManager.Instance.Play(datas);

        private void OnClick() => OnClickEvent?.Invoke(parameter);
    }
}