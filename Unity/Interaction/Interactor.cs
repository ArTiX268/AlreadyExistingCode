using ArTiX.Utils.TickSystem;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ArTiX.Interaction
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interactor : MonoBehaviour
    {
        private static Interactor instance;
        public static Interactor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(nameof(Interactor), typeof(Interactor))
                        .GetComponent<Interactor>();
                }
                return instance;
            }
        }

        [SerializeField] private Camera interactionCamera;
        [SerializeField] private InteractorSO datas;

        private IInteractable interactable;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            interactionCamera ??= Camera.main;
            Enable();
        }

        public void Interact(InputAction.CallbackContext ctxt)
        {
            if (ctxt.started)
                interactable?.Interact();
        }

        private void CheckForInteractable(object sender, EventArgs e)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (interactable == null) return;

                interactable.ExitCanInteract();
                interactable = null;
                return;
            }

            Ray ray = interactionCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, datas.InteractionDistance, datas.InteractionMask))
            {
                if (!hit.collider.TryGetComponent(out IInteractable foundInteractable))
                {
                    Debug.LogWarning(
                        $"Object is on interaction layer but does not implement {nameof(IInteractable)} interface.");
                    return;
                }

                if (foundInteractable != interactable)
                {
                    interactable?.ExitCanInteract();
                    interactable = foundInteractable;
                    interactable.EnterCanInteract();
                }
            }
            else
            {
                interactable?.ExitCanInteract();
                interactable = null;
            }
        }

        public void Enable()
        {
            TickSystem.Instance.OnTick += CheckForInteractable;
        }
        public void Disable()
        {
            TickSystem.Instance.OnTick -= CheckForInteractable;
        }
    }
}