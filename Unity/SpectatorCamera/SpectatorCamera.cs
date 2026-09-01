using UnityEngine;
using UnityEngine.InputSystem;

namespace ArTiX.Utils
{
    [RequireComponent(typeof(PlayerInput), typeof(Camera))]
    public class SpectatorCamera : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private bool holdToRotate;

        private Vector3 moveDirection;
        private float upDown;

        private Camera cam;
        private InputAction rotateAction;

        private void Awake()
        {
            rotateAction = GetComponent<PlayerInput>().actions.actionMaps[0]
                .FindAction(actionNameOrId: "Rotate");

            rotateAction.Disable();

            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            transform.position += speed * Time.deltaTime *
                ((transform.rotation * moveDirection) + (Vector3.up * upDown));
        }

        public void OnWASD(InputAction.CallbackContext ctxt)
        {
            Vector2 moveInput = ctxt.ReadValue<Vector2>();
            moveDirection.x = moveInput.x;
            moveDirection.z = moveInput.y;
        }

        public void OnUpDown(InputAction.CallbackContext ctxt)
        {
            upDown = ctxt.ReadValue<float>();
        }

        public void OnRotate(InputAction.CallbackContext ctxt)
        {
            Vector2 rotateInput = ctxt.ReadValue<Vector2>();
            transform.Rotate(
                xAngle: -rotateInput.y * sensitivity,
                yAngle: rotateInput.x * sensitivity,
                zAngle: 0
            );

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }

        public void ToggleRotation(InputAction.CallbackContext ctxt)
        {
            if (holdToRotate)
            {
                if (ctxt.started) rotateAction.Enable();
                else if (ctxt.canceled) rotateAction.Disable();
            }
            else
            {
                if (rotateAction.enabled) rotateAction.Disable();
                else rotateAction.Enable();
            }
        }
    }
}