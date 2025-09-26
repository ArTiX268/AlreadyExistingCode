using UnityEngine.InputSystem;

public class MovementInputManager : InputManager
{
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction crouchAction;

    private void OnEnable()
    {
        moveAction = InputActionScript.Player.Movement;
        moveAction.Enable();
        inputActions.Add(EAction.Move, moveAction);

        jumpAction = InputActionScript.Player.Jump;
        jumpAction.Enable();
        inputActions.Add(EAction.Jump, jumpAction);

        runAction = InputActionScript.Player.Run;
        runAction.Enable();
        inputActions.Add(EAction.Run, runAction);

        crouchAction = InputActionScript.Player.Crouch;
        crouchAction.Enable();
        inputActions.Add(EAction.Crouch, crouchAction);
    }
}