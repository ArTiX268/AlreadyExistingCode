using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public enum MovementState
{
    Standing,
    Crouching,
    Jumping,
    Crawling
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    #region Serialized

    [Title("Speed")]
    [SerializeField, SuffixLabel("m/s")] private float maxWalkSpeed;

    [SerializeField, SuffixLabel("m/s")] private float maxBackwardSpeed;
    [SerializeField, SuffixLabel("m/s")] private float maxRunSpeed;
    [SerializeField, SuffixLabel("m/s")] private float maxCrouchedSpeed;
    [SerializeField, SuffixLabel("m/s²")] private float acceleration;
    [SerializeField, SuffixLabel("m/s²")] private float deceleration;

    [Title("Air Movement")]
    [SerializeField, SuffixLabel("m")] private float jumpHeight;

    [SerializeField, SuffixLabel("s")] private float timeToReachJumpHeight;
    [SerializeField] private float fallingGravityMultiplier = 1;
    [SerializeField, Range(0, 1), Tooltip("The closer to one, the most air control you have.")] private float airControl = 1;

    [Title("Rotation")]
    [SerializeField, Tooltip("The player will rotate to face the direction it's moving. If false, it will rotate to face where the camera is facing.")] private bool rotateToMovement = false;

    [SerializeField, Tooltip("Rotates the player even when he's not moving."), HideIf("rotateToMovement")] private bool rotateWhenIdle = false;
    [SerializeField] private float rotationSpeed;

    [Title("Crouching")]
    [SerializeField] private bool holdToCrouch;

    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float capsuleCrouchedHeight;
    [SerializeField] private float capsuleStandingHeight;
    [SerializeField] private Vector3 capsuleCrouchedCenter;
    [SerializeField] private Vector3 capsuleStandingCenter;
    [SerializeField] private bool jumpUncrouches;

    [Title("References")]
    [SerializeField, Required, SceneObjectsOnly] private Transform cameraTransform;

    #endregion Serialized

    #region Non Serialized

    // Components
    private CharacterController characterController;

    // Used to determine the displacement.
    private Vector3 movementVector = new();

    private Vector2 movementInputVector;
    private Vector3 velocity;
    private float currentSpeed;
    private float targetSpeed;
    private bool wantsToRun;

    // Jump
    private float jumpForce;

    private float gravity;

    // Camera
    private Vector3 cameraForward;

    private Vector3 cameraRight;

    // Crouch
    private Vector3 capsuleTargetCenter;

    private float capsuleTargetHeight;
    private bool isCrouched;

    public event EventHandler<bool> OnCrouching;
    public event EventHandler OnJumping;

    #region Constants & Readonlies

    private const float SPEED_DEAD_ZONE = 0.1f;

    #endregion Constants & Readonlies

    #endregion Non Serialized

    #endregion Variables

    #region Unity Functions

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        jumpForce = (2 * jumpHeight) / timeToReachJumpHeight;
        gravity = (-2 * jumpHeight) / (timeToReachJumpHeight * timeToReachJumpHeight);

        if (rotateToMovement)
        {
            movementVector = (cameraForward * movementInputVector.y) + (cameraRight * movementInputVector.x);
            rotateWhenIdle = false;
        }

        capsuleTargetCenter = characterController.center;
        capsuleTargetHeight = characterController.height;
    }

    private void Start()
    {
        InputManager lInstance = InputManager.Instance;

        lInstance.AssignInput(InputManager.EAction.Jump, Jump, InputManager.EventType.Started);

        lInstance.AssignInput(InputManager.EAction.Run, Run, InputManager.EventType.Started);
        lInstance.AssignInput(InputManager.EAction.Run, Run, InputManager.EventType.Canceled);

        lInstance.AssignInput(InputManager.EAction.Crouch, CallCrouch, InputManager.EventType.Started);

        if (holdToCrouch)
            lInstance.AssignInput(InputManager.EAction.Crouch, CallCrouch, InputManager.EventType.Canceled);
    }

    private void Update()
    {
        AssignCameraVectors();
        HorizontalMovement();

        if (!characterController.isGrounded)
            ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);
        Rotate();

        LerpCapsule();
    }

    #endregion Unity Functions

    private void AssignCameraVectors()
    {
        cameraForward = cameraTransform.forward.PutVectorOnXZPlane();
        cameraRight = cameraTransform.right.PutVectorOnXZPlane();
    }

    #region GroundMovement

    private void HorizontalMovement()
    {
        void CalculateMovementVector()
        {
            movementVector = Vector3.Lerp(
                                          a: movementVector,
                                          b: (cameraForward * movementInputVector.y) + (cameraRight * movementInputVector.x),
                                          t: Time.deltaTime * acceleration * (characterController.isGrounded ? 1 : airControl));
        }
        void CalculateSpeed()
        {
            void SelectTargetSpeed()
            {
                if (movementInputVector == Vector2.zero)
                {
                    targetSpeed = 0;
                    return;
                }

                if (isCrouched)
                {
                    targetSpeed = maxCrouchedSpeed;
                    return;
                }

                if (movementInputVector.y > 0)
                    targetSpeed = wantsToRun ? maxRunSpeed : maxWalkSpeed;
                else if (movementInputVector.y < 0)
                    targetSpeed = maxBackwardSpeed;
                else if (movementInputVector.x != 0)
                    targetSpeed = wantsToRun ? maxRunSpeed : maxWalkSpeed;
            }

            SelectTargetSpeed();

            if (Mathf.Abs(currentSpeed - targetSpeed) < SPEED_DEAD_ZONE) currentSpeed = targetSpeed;
            else
            {
                currentSpeed = Mathf.Lerp(
                                          a: currentSpeed,
                                          b: targetSpeed,
                                          t: Time.deltaTime * (movementInputVector != Vector2.zero ? acceleration : deceleration) * (characterController.isGrounded ? 1 : airControl));
            }
        }

        movementInputVector = InputManager.Instance.GetInputAction(InputManager.EAction.Move).ReadValue<Vector2>();

        if (movementInputVector != Vector2.zero)
            CalculateMovementVector();

        CalculateSpeed();

        velocity.x = (currentSpeed * movementVector).x;
        velocity.z = (currentSpeed * movementVector).z;
    }

    private void Run(InputAction.CallbackContext pContext)
        => wantsToRun = pContext.started;

    #endregion GroundMovement

    #region Air Movement

    private void ApplyGravity()
        => velocity += (characterController.velocity.y < 0 ? fallingGravityMultiplier : 1) * gravity * Time.deltaTime * Vector3.up;

    private void Jump(InputAction.CallbackContext pContext)
    {
        if (characterController.isGrounded)
        {
            if (isCrouched)
            {
                if (jumpUncrouches)
                    Crouch(false);

                return;
            }

            velocity.y = jumpForce;
            OnJumping?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion Air Movement

    #region Rotation

    private void Rotate()
    {
        void RotationWhileMoving()
            => transform.rotation = Quaternion.Slerp(
                                                  a: transform.rotation,
                                                  b: Quaternion.LookRotation(forward: rotateToMovement ? movementVector : cameraForward),
                                                  t: Time.deltaTime * rotationSpeed);
        void RotationWhileIdle()
            => transform.rotation = Quaternion.Slerp(
                                                  a: transform.rotation,
                                                  b: Quaternion.LookRotation(cameraForward),
                                                  t: Time.deltaTime * rotationSpeed);

        if (characterController.velocity != Vector3.zero)
            RotationWhileMoving();
        else if (rotateWhenIdle)
            RotationWhileIdle();
    }

    #endregion Rotation

    #region Crouching

    private void CallCrouch(InputAction.CallbackContext pContext)
    {
        if (pContext.canceled && !isCrouched) return;

        Crouch(!isCrouched);
    }

    private void Crouch(in bool pCrouch)
    {
        isCrouched = pCrouch;
        capsuleTargetCenter = pCrouch ? capsuleCrouchedCenter : capsuleStandingCenter;
        capsuleTargetHeight = pCrouch ? capsuleCrouchedHeight : capsuleStandingHeight;

        OnCrouching?.Invoke(this, isCrouched);
    }

    private void LerpCapsule()
    {
        if (Mathf.Abs(capsuleTargetHeight - characterController.height) < 0.01f && (capsuleTargetCenter - characterController.center).sqrMagnitude < 0.01f * 0.01f)
            return;

        characterController.height = Mathf.Lerp(characterController.height, capsuleTargetHeight, Time.deltaTime * crouchingSpeed);
        characterController.center = Vector3.Lerp(characterController.center, capsuleTargetCenter, Time.deltaTime * crouchingSpeed);
    }

    #endregion Crouching
}