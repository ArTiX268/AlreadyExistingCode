using UnityEngine;
using UnityEngine.InputSystem;

public class Third_Person_CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;

    [Header("Camera")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform followTarget;

    private Camera cam;

    private Rigidbody rb;

    private Vector2 movementVector;
    private Vector2 lookVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        cam = Camera.main;

        InputManager.AssignEvent(ref InputManager.jumpAction, Jump, EventType.Started);
    }

    private void Update()
    {
        movementVector = InputManager.moveAction.ReadValue<Vector2>();
        lookVector = InputManager.lookAction.ReadValue<Vector2>();

        RotateCamera();
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.velocity = CalculateNewVelocity(transform.forward, transform.right);
    }

    private Vector3 CalculateNewVelocity(Vector3 forward, Vector3 right)
    {
        forward *= speed * movementVector.y;
        right *= speed * movementVector.x;

        return forward + right + new Vector3(0, rb.velocity.y);
    }

    private void RotateCamera()
    {
        transform.rotation *= Quaternion.AngleAxis(lookVector.x * sensitivity, Vector3.up);

        RotateCameraAroundX_Axis();
    }

    private void RotateCameraAroundX_Axis()
    {
        followTarget.transform.rotation *= Quaternion.AngleAxis(-lookVector.y * sensitivity, Vector3.right);

        var angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTarget.transform.localEulerAngles.x;

        if (angle > 180 && angle < 320)
            angles.x = 320;
        else if (angle < 180 && angle > 60)
            angles.x = 60;

        followTarget.transform.localEulerAngles = angles;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (IsOnGround())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            1.5f,
            groundLayer);
    }
}