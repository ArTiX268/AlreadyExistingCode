using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                instance ??= new GameObject("InputManager", typeof(InputManager)).GetComponent<InputManager>();
                return instance;
            }
        }

        // Actions
        public event Action OnSelect;
        public event Action OnStartCreatingBuilding;
        public event Action OnFinishCreating;
        public event Action OnCancel;
        public event Action OnToggleBuildingPanel;
        public event Action<int> OnRotate;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void LeftMouseButton(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started) OnStartCreatingBuilding?.Invoke();
            if (ctxt.performed) OnSelect?.Invoke();
            if (ctxt.canceled) OnFinishCreating?.Invoke();
        }

        public void Cancel(InputAction.CallbackContext ctxt)
        {
            if (ctxt.performed) OnCancel?.Invoke();
        }

        public void Rotate(InputAction.CallbackContext ctxt)
        {
            if (ctxt.performed) OnRotate?.Invoke(Mathf.CeilToInt(ctxt.ReadValue<float>()));
        }

        public void ToggleBuildingPanel(InputAction.CallbackContext ctxt)
        {
            if (ctxt.performed) OnToggleBuildingPanel?.Invoke();
        }

        public void ClearAllEvents()
        {
            OnSelect = null;
            OnCancel = null;
        }
    }
}