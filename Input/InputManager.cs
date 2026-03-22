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
    }

    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActionScript;
    private readonly Dictionary<EAction, InputAction> inputActions = new();

    // Actions
    private InputAction moveAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActionScript = new();
    }

    private void OnEnable()
    {
        moveAction = inputActionScript.Player.Movement;

        moveAction.Enable();

        inputActions.Add(EAction.Move, moveAction);
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