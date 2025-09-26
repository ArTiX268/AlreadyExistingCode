using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public abstract class InputManager : MonoBehaviour
{
    public enum EventType
    {
        Started,
        Performed,
        Canceled
    }

    public enum EAction
    {
        Move,
        Look,
        Jump,
        Run,
        Crouch
    }

    public InputManager Instance { get; private set; }

    protected PlayerInputActions InputActionScript {  get; private set; }
    protected readonly Dictionary<EAction, InputAction> inputActions = new();

    private void Awake()
    {
        Instance = this;

        InputActionScript = new();
    }

    public void EnableInput(EAction action) => inputActions[action].Enable();
    public void DisableInput(EAction action) => inputActions[action].Disable();

    public void AssignInput(EAction action, Action<CallbackContext> assigningEvent, EventType eventType)
    {
        InputAction inputAction = inputActions[action];

        switch (eventType)
        {
            case EventType.Started:
                inputAction.started += assigningEvent;
                break;
            case EventType.Performed:
                inputAction.performed += assigningEvent;
                break;
            case EventType.Canceled:
                inputAction.canceled += assigningEvent;
                break;
        }
    }

    public void UnassignInput(EAction action, Action<CallbackContext> unassigningEvent, EventType eventType)
    {
        InputAction inputAction = inputActions[action];

        switch (eventType)
        {
            case EventType.Started:
                inputAction.started -= unassigningEvent;
                break;
            case EventType.Performed:
                inputAction.performed -= unassigningEvent;
                break;
            case EventType.Canceled:
                inputAction.canceled -= unassigningEvent;
                break;
        }
    }

    public void AssignMultipleEvents(EAction action, EventType eventType, params Action<CallbackContext>[] events)
    {
        for (int i = 0; i < events.Length; i++)
            AssignInput(action, events[i], eventType);
    }

    public void AssignMultipleInputsToEvent(Action<CallbackContext> assigningEvent, EventType eventType, params EAction[] actions)
    {
        foreach (EAction action in actions)
            AssignInput(action, assigningEvent, eventType);
    }

    public InputAction GetInputAction(EAction action) => inputActions[action];
}