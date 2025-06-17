using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Parameters")]
    // Displacement
    [SerializeField] private float maxSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float groundDesceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airDesceleration;

    // Jump
    [SerializeField, Range(0, 100)] private float maxJumpHeight;
    [SerializeField, Range(0, 100)] private float minJumpHeight;
    [SerializeField, Range(0, 10)] private float timeToJumpApex;
    [SerializeField, Range(0, 10)] private float fallingGravityScale;
    [SerializeField, Range(0, 10)] private float releasingGravityMultiplyer;

    [SerializeField] private LayerMask groundLayer;

    [Header("Cheats")]
    [SerializeField] private float coyoteTime;

    [SerializeField] private float jumpBufferTime;

    private float left_RightValue;
    private float acceleration;
    private float desceleration;
    private float jumpTimer;
    private float upwardGravityScale;

    private bool hasUsedCoyoteTime;

    private Timer coyoteTimer;
    private Timer jumpBufferTimer;

    // COMPONENTS
    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Assign inputs
        InputManager.AssignEvent(ref InputManager.jumpInput, Jump, EventType.Started);
        InputManager.AssignEvent(ref InputManager.jumpInput, StopJump, EventType.Canceled);

        coyoteTimer = TimerManager.CreateTimer("coyoteTimer");
        coyoteTimer.InitializeValues(coyoteTime, 0, 0, false);

        jumpBufferTimer = TimerManager.CreateTimer("jumpBufferTimer");
        jumpBufferTimer.InitializeValues(jumpBufferTime, 0, 0, false);
    }

    private void Update()
    {
        left_RightValue = InputManager.leftRightInput.ReadValue<float>();

        // Cheats
        if (!coyoteTimer.isActive && !IsOnGround() && !hasUsedCoyoteTime)
            coyoteTimer.StartTimer();

        if (coyoteTimer.timerFinished)
            hasUsedCoyoteTime = true;

        if (IsOnGround() && rb2D.velocity.y < 0)
            hasUsedCoyoteTime = false;

        if (jumpBufferTimer.isActive)
            Jump(default);

        // Variable jump height
        if (jumpTimer > 0)
        {
            jumpTimer += Time.deltaTime;

            if (jumpTimer >= timeToJumpApex)
            {
                KillJump();
            }
        }
    }

    private void FixedUpdate()
    {
        rb2D.gravityScale = CalculateGravityScale();
        rb2D.velocity = new Vector2(CalculateHorizontalMovement(), rb2D.velocity.y);
    }

    private float CalculateHorizontalMovement()
    {
        float horizontalVelocity = rb2D.velocity.x;
        if (left_RightValue != 0)
        {
            acceleration = IsOnGround() ? groundAcceleration : airAcceleration;
            horizontalVelocity = Mathf.MoveTowards(horizontalVelocity, left_RightValue * maxSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            desceleration = IsOnGround() ? groundDesceleration : airDesceleration;
            horizontalVelocity = Mathf.MoveTowards(horizontalVelocity, 0, desceleration * Time.fixedDeltaTime);
        }
        return horizontalVelocity;
    }

    private float CalculateJumpVelocity()
    {
        return 2 * maxJumpHeight / timeToJumpApex;
    }

    private float CalculateJumpGravityScale(float _jumpHeight)
    {
        return (-2 * _jumpHeight) / (timeToJumpApex * timeToJumpApex * Physics2D.gravity.y);
    }

    private float CalculateGravityScale()
    {
        if (IsOnGround())
            return 1;
        else
        {
            if (rb2D.velocity.y > .01f)
                return upwardGravityScale;
            else if (rb2D.velocity.y < .01f)
                return fallingGravityScale;
        }
        return 1;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (IsOnGround() || coyoteTimer.isActive && !hasUsedCoyoteTime)
        {
            upwardGravityScale = CalculateJumpGravityScale(maxJumpHeight);
            rb2D.velocity = new Vector2(rb2D.velocity.x, CalculateJumpVelocity());
            hasUsedCoyoteTime = true;

            jumpTimer += Time.deltaTime;
        }
        else if (!jumpBufferTimer.isActive)
        {
            jumpBufferTimer.StartTimer();
        }
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        KillJump();
    }

    private void KillJump()
    {
        upwardGravityScale *= releasingGravityMultiplyer;
        jumpTimer = 0;
    }

    private bool IsOnGround()
    {
        return Physics2D.Raycast(
        transform.position,
        Vector2.down,
        1.1f,
        groundLayer);
    }
}