using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    public enum EventType : byte
    {
        Started,
        Performed,
        Canceled
    }
    public enum EAction : ushort
    {
        Move,
        Look,
        Jump,
        Run,
        Aiming,
        Zoom,
        Crouch,
        Prone,
        Interact,
        Drop,
        Shoot,
        Reload,
        SelectInventorySlot
    }

    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActionScript;
    private readonly Dictionary<EAction, InputAction> inputActions = new();

    // Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction crouchAction;
    private InputAction proneAction;
    private InputAction runAction;
    private InputAction amingAction;
    private InputAction zoomAction;
    private InputAction interactAction;
    private InputAction dropAction;
    private InputAction shootAction;
    private InputAction reloadAction;
    private InputAction selectInventorySlot;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActionScript = new();
    }

    private void OnEnable()
    {
        moveAction          = inputActionScript.Player.Movement;
        jumpAction          = inputActionScript.Player.Jump;
        lookAction          = inputActionScript.Player.Look;
        crouchAction        = inputActionScript.Player.Crouch;
        proneAction         = inputActionScript.Player.Prone;
        runAction           = inputActionScript.Player.Run;
        amingAction         = inputActionScript.Player.Aiming;
        zoomAction          = inputActionScript.Player.Zoom;
        interactAction      = inputActionScript.Player.Interact;
        dropAction          = inputActionScript.Player.Drop;
        shootAction         = inputActionScript.Player.Shoot;
        reloadAction        = inputActionScript.Player.Reload;
        selectInventorySlot = inputActionScript.Player.SelectSlot;

        moveAction         .Enable();
        jumpAction         .Enable();
        lookAction         .Enable();
        crouchAction       .Enable();
        proneAction        .Enable();
        runAction          .Enable();
        amingAction        .Enable();
        zoomAction         .Enable();
        interactAction     .Enable();
        dropAction         .Enable();
        shootAction        .Enable();
        reloadAction       .Enable();
        selectInventorySlot.Enable();

        inputActions.Add(EAction.Move,                moveAction);
        inputActions.Add(EAction.Jump,                jumpAction);
        inputActions.Add(EAction.Look,                lookAction);
        inputActions.Add(EAction.Run,                 runAction);
        inputActions.Add(EAction.Aiming,              amingAction);
        inputActions.Add(EAction.Zoom,                zoomAction);
        inputActions.Add(EAction.Crouch,              crouchAction);
        inputActions.Add(EAction.Prone,               proneAction);
        inputActions.Add(EAction.Interact,            interactAction);
        inputActions.Add(EAction.Drop,                dropAction);
        inputActions.Add(EAction.Shoot,               shootAction);
        inputActions.Add(EAction.Reload,              reloadAction);
        inputActions.Add(EAction.SelectInventorySlot, selectInventorySlot);
    }

    private void OnDisable()
    {
        inputActionScript.Disable();
    }

    #region Interaction Functions

    public void EnableInput(EAction pAction) => inputActions[pAction].Enable();
    public void DisableInput(EAction pAction) => inputActions[pAction].Disable();

    public void AssignInput(in EAction pAction, in Action<CallbackContext> pAssigningEvent, in EventType pEventType)
    {
        InputAction lInputAction = inputActions[pAction];

        switch (pEventType)
        {
            case EventType.Started:
                lInputAction.started += pAssigningEvent;
                break;
            case EventType.Performed:
                lInputAction.performed += pAssigningEvent;
                break;
            case EventType.Canceled:
                lInputAction.canceled += pAssigningEvent;
                break;
        }
    }

    public void UnassignInput(in EAction pAction, in Action<CallbackContext> pUnassigningEvent, in EventType pEventType)
    {
        InputAction lInputAction = inputActions[pAction];

        switch (pEventType)
        {
            case EventType.Started:
                lInputAction.started -= pUnassigningEvent;
                break;
            case EventType.Performed:
                lInputAction.performed -= pUnassigningEvent;
                break;
            case EventType.Canceled:
                lInputAction.canceled -= pUnassigningEvent;
                break;
        }
    }

    public InputAction GetInputAction(EAction pAction) => inputActions[pAction];

    #endregion
}