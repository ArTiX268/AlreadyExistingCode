using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
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
    }

    private PlayerInputActions inputActionScript;
    public Dictionary<EAction, InputAction> inputActions;

    // Create your inputs here and declare them as static.
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    /// <summary>
    /// Dictionary that links the input action to an ID.
    /// Used for inventory slots.
    /// </summary>
    private Dictionary<InputAction, uint> inventorySlotActionIdDictionary = new Dictionary<InputAction, uint>();

    private void OnEnable()
    {
        inputActionScript = new();
        inputActionScript.Enable();
        inputActions = new Dictionary<EAction, InputAction>();

        // Assign and enable inputs here.
        // Exemple :
        jumpAction = inputActionScript.Player.Jump;
        jumpAction.Enable();
        inputActions.Add(EAction.Jump, jumpAction);

        moveAction = inputActionScript.Player.Movement;
        moveAction.Enable();
        inputActions.Add(EAction.Move, moveAction);

        lookAction = inputActionScript.Player.Look;
        lookAction.Enable();
        inputActions.Add(EAction.Look, lookAction);
    }

    private void OnDisable()
    {
        inputActionScript.Disable();
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

    public InputAction GetAction(EAction action) => inputActions[action];

    public uint GetInventorySlotID(InputAction inputAction) => inventorySlotActionIdDictionary[inputAction];
}