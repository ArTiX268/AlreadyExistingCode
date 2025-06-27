using UnityEngine;
using UnityEngine.InputSystem;

public class Third_Person_CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float groudSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool orientePlayerToMovement;

    [Header("Camera")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 followTargetOffset;

    private Rigidbody rb;

    private Transform mainCamera;

    private Vector2 movementVector;
    private Vector2 lookVector;

    private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        InputManager.AssignEvent(ref InputManager.jumpAction, Jump, EventType.Started);

        mainCamera = Camera.main.transform;

        followTargetOffset = followTarget.position - transform.position;
    }

    private void Update()
    {
        movementVector = InputManager.moveAction.ReadValue<Vector2>();
        lookVector = InputManager.lookAction.ReadValue<Vector2>();

        followTarget.position = transform.position + followTargetOffset;
        RotateCamera();
    }

    private void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb.velocity = CalculateNewVelocity();
    }

    private Vector3 CalculateNewVelocity()
    {
        GetCameraForward_Right(out Vector3 forward, out Vector3 right);

        speed = IsOnGround() ? groudSpeed : airSpeed;

        forward *= speed * movementVector.y;
        right *= speed * movementVector.x;

        OrientPlayerToMovement();

        return forward + right + new Vector3(0, rb.velocity.y);
    }

    private void GetCameraForward_Right(out Vector3 forward, out Vector3 right)
    {
        forward = FlattenVector(mainCamera.forward);
        right = FlattenVector(mainCamera.right);
    }

    private Vector3 FlattenVector(Vector3 vector)
    {
        vector.y = 0;
        return vector.normalized;
    }

    private void OrientPlayerToMovement()
    {
        if (movementVector.x != 0 || movementVector.y != 0)
        {
            Vector3 cameraForward = FlattenVector(Camera.main.transform.forward);

            transform.forward = Vector3.Slerp(transform.forward, cameraForward, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void RotateCamera()
    {
        followTarget.Rotate(Vector3.up, lookVector.x * sensitivity, Space.World);
        followTarget.Rotate(Vector3.right, -lookVector.y * sensitivity, Space.Self);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (IsOnGround())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
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
