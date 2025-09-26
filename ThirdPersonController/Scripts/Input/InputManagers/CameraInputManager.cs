using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputManager : InputManager
{
    private InputAction lookAction;
    private InputAction aimAction;

    private void OnEnable()
    {
        lookAction = InputActionScript.Player.Look;
        lookAction.Enable();
        inputActions.Add(EAction.Look, lookAction);
    }
}