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
        public event Action<Vector2> OnDragging;
        public event Action OnSimpleTapStarted;
        public event Action OnSimpleTapCanceled;
        public event Action OnDebugNitro;
        public event Action OnDebugInvincible;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void GetTouchPos(InputAction.CallbackContext ctxt)
        {
            OnDragging?.Invoke(ConvertTouchPosToWorldPos(ctxt.ReadValue<Vector2>()));
        }

        public void OnSimpleTouch(InputAction.CallbackContext ctxt)
        {
            if (ctxt.performed) OnSimpleTapStarted?.Invoke();
            else if (ctxt.canceled) OnSimpleTapCanceled?.Invoke();
        }

        public void DebugNitro()
        {
            OnDebugNitro?.Invoke();
        }

        public void DebugInvincible(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started)
                OnDebugInvincible?.Invoke();
        }

        private Vector3 ConvertTouchPosToWorldPos(Vector2 touchPos)
        {
            Vector3 realWorldPos = Camera.main.ScreenToWorldPoint(touchPos);
            realWorldPos.z = 0;
            return realWorldPos;
        }

        public void ClearAllEvents()
        {
            OnDragging = null;
            OnSimpleTapStarted = null;
            OnSimpleTapCanceled = null;
        }
    }
}