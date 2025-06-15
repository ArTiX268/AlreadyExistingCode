using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum EventType
{
    Started,
    Performed,
    Canceled
}

public class InputManager : MonoBehaviour
{
    private InputManager instance;
    private PlayerInputActions inputActions;

    // Create your inputs here.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            inputActions = new PlayerInputActions();
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Assign and enable inputs here.
        // Exemple :
        //jumpInput = inputActions.Player.Jump;
        //jumpInput.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public static void EnableInput(ref InputAction action) { action.Enable(); }
    public static void DisableInput(ref InputAction action) { action.Disable(); }

    private static void AssignInput(ref InputAction action, Action<CallbackContext> assigningEvent, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Started:
                action.started += assigningEvent;
                break;
            case EventType.Performed:
                action.performed += assigningEvent;
                break;
            case EventType.Canceled:
                action.canceled += assigningEvent;
                break;
        }
    }

    private static void UnassignInput(ref InputAction action, Action<CallbackContext> unassigningEvent, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Started:
                action.started -= unassigningEvent;
                break;
            case EventType.Performed:
                action.performed -= unassigningEvent;
                break;
            case EventType.Canceled:
                action.canceled -= unassigningEvent;
                break;
        }
    }

    public static void AssignEvent(ref InputAction action, Action<CallbackContext> assigningEvent, EventType eventType)
    {
        AssignInput(ref action, assigningEvent, eventType);
    }

    public static void AssignMultipleEvents(ref InputAction action, Action<CallbackContext>[] events, EventType eventType)
    {
        for (int i = 0; i < events.Length; i++)
            AssignEvent(ref action, events[i], eventType);
    }

    public static void AssignMultipleInputsToEvent(ref InputAction[] actions, Action<CallbackContext> assigningEvent, EventType eventType)
    {
        for (int i = 0; i < actions.Length; i++)
            AssignEvent(ref actions[i], assigningEvent, eventType);
    }

    public static void ReassignEvent(ref InputAction previousAction, ref InputAction newAction, Action<CallbackContext> eventToAssign, EventType eventType)
    {
        UnassignInput(ref previousAction, eventToAssign, eventType);
        AssignInput(ref newAction, eventToAssign, eventType);
    }

    public static void ReassignInput(ref InputAction action, Action<CallbackContext> previousEvent, Action<CallbackContext> newEvent, EventType eventType)
    {
        UnassignInput(ref action, previousEvent, eventType);
        AssignInput(ref action, newEvent, eventType);
    }

    public static void UnasssignEvent(ref InputAction action, Action<CallbackContext> eventToUnassign, EventType eventType)
    {
        UnassignInput(ref action, eventToUnassign, eventType);
    }

    public static void UnasssignMultipleEvents(ref InputAction action, Action<CallbackContext>[] events, EventType eventType)
    {
        for (int i = 0; i < events.Length; i++)
        {
            UnasssignEvent(ref action, events[i], eventType);
        }
    }

    public static void UnasssignMultipleInputs(ref InputAction[] actions, Action<CallbackContext> eventToUnassign, EventType eventType)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            UnasssignEvent(ref actions[i], eventToUnassign, eventType);
        }
    }
}