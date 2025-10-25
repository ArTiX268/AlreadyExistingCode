using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamera : MonoBehaviour
{
    [SerializeField, SuffixLabel("m/s")] private float minSpeed;
    [SerializeField, SuffixLabel("m/s")] private float maxSpeed;
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    private float currentSpeed;
    private bool isInCameraMode = false;

    private Vector2 targetRotation;

    private InputManager inputManager;

    private void Awake()
    {
        inputManager = InputManager.Instance;

        inputManager.AssignInput(InputManager.EAction.CameraMode, ToggleCameraMode, InputManager.EventType.Started);
        inputManager.AssignInput(InputManager.EAction.CameraMode, ToggleCameraMode, InputManager.EventType.Canceled);
        inputManager.EnableInput(InputManager.EAction.CameraMode);

        currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.5f);

        targetRotation = new Vector2(transform.rotation.x, transform.rotation.y);
    }

    private void Update()
    {
        if (!isInCameraMode) return;

        float lDeltaTime = Time.deltaTime;

        if (inputManager.GetInputAction(InputManager.EAction.Movement).IsPressed())
        {
            Vector2 lMovementInput = inputManager.GetInputAction(InputManager.EAction.Movement).ReadValue<Vector2>();
            transform.position += (transform.forward * lMovementInput.y + transform.right * lMovementInput.x) * (lDeltaTime * currentSpeed);
        }

        if (inputManager.GetInputAction(InputManager.EAction.Look).IsPressed())
        {
            Vector2 lLookInput = inputManager.GetInputAction(InputManager.EAction.Look).ReadValue<Vector2>();

            /* We need to use the Y of the mouse on the X of the vector and the opposite for the X of the mouse to the Y of the vector because
             the X property of transform.rotation is used for the pitch and the Y for the yaw. Pitch is for up/down and yaw for left/right.*/
            targetRotation.x -= lLookInput.y * sensitivityY * lDeltaTime;
            targetRotation.y += lLookInput.x * sensitivityX * lDeltaTime;

            transform.rotation = Quaternion.Euler(targetRotation);
        }
    }

    private void ToggleCameraMode(InputAction.CallbackContext pContext)
    {
        if (pContext.started)
        {
            isInCameraMode = true;
            inputManager.EnableInput(InputManager.EAction.Movement);
            inputManager.EnableInput(InputManager.EAction.Look);
        }
        else if (pContext.canceled)
        {
            isInCameraMode = false;
            inputManager.DisableInput(InputManager.EAction.Movement);
            inputManager.DisableInput(InputManager.EAction.Look);
        }
    }
}