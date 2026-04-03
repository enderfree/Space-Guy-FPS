using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float topSpeed;
    [SerializeField] private float acceleration; // acceleration is very small because it is multiplied by Time.fixedDeltaTime in order to make it frame independent
    // Time.fixedDeltaTime by default is 0.2
    // this means acceleration is always divided by 5, making it likely to need to be of an higher value than topSpeed.

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float jumpCutMltp;
    [SerializeField] private float fallMltp;
    [SerializeField] private float lowJumpMltp;
    [SerializeField] private float maxFallSpeed;

    [Header("Ground Info")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool jumpHeld = false;

    private bool isMoving = false; // for animation
    private bool isJumping = false;

    private InputSystem_Actions inputAction;
    private Rigidbody rb;

    // Unity
    private void Awake()
    {
        inputAction = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>(); // yes, I want it to crash if it's not found
    }

    private void OnEnable()
    {
        inputAction.Player.Move.Enable();
        inputAction.Player.Move.performed += OnMovePerformed;
        inputAction.Player.Move.canceled += OnMoveCanceled;

        inputAction.Player.Look.Enable();

        inputAction.Player.Jump.Enable();
        inputAction.Player.Jump.performed += OnJumpPerformed;
        inputAction.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        inputAction.Player.Jump.canceled -= OnJumpCanceled;
        inputAction.Player.Jump.performed -= OnJumpPerformed;
        inputAction.Player.Jump.Disable();

        inputAction.Player.Look.Disable();

        inputAction.Player.Move.canceled -= OnMoveCanceled;
        inputAction.Player.Move.performed -= OnMovePerformed;
        inputAction.Player.Move.Disable();
    }

    void FixedUpdate()
    {
        Move();
        Look();
    }

    // Events
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        isMoving = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
            jumpHeld = true;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            jumpReleased = true;
            jumpHeld = false;
        }
    }

    // Misc
    private bool IsGrounded()
    {
        return Physics.OverlapSphere(groundCheck.position, groundDistance, ground).Length > 0;
    }

    private void Move()
    {
        // y
        if (jumpPressed)
        {
            jumpBufferTimer = jumpBuffer;
            jumpPressed = false;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (IsGrounded())
        {
            coyoteTimer = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        float yVelocity = rb.linearVelocity.y;
        if (jumpBufferTimer > 0f && coyoteTimer > 0f && !isJumping)
        {
            yVelocity = jumpForce;
            isJumping = true;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }

        if (jumpReleased && yVelocity > 0f)
        {
            yVelocity *= jumpCutMltp;
            jumpReleased = false;
        }

        if (yVelocity < 0f)
        {
            yVelocity += Vector2.up.y * Physics.gravity.y * (fallMltp - 1) * Time.fixedDeltaTime;
        }
        else if (yVelocity > 0f && jumpHeld)
        {
            yVelocity += Vector2.up.y * Physics.gravity.y * (lowJumpMltp - 1) * Time.fixedDeltaTime;
        }

        if (yVelocity < maxFallSpeed)
        {
            yVelocity = maxFallSpeed;
        }

        // x, z
        Vector2 move = inputAction.Player.Move.ReadValue<Vector2>();

        rb.linearVelocity = new Vector3(
            Mathf.MoveTowards( // x
                rb.linearVelocity.x,
                topSpeed * move.x,
                acceleration * Time.fixedDeltaTime
                ),
            yVelocity, // y
            Mathf.MoveTowards( // z
                rb.linearVelocity.z,
                topSpeed * move.y,
                acceleration * Time.fixedDeltaTime
                ));
    }

    private void Look()
    {
        Vector2 look = inputAction.Player.Look.ReadValue<Vector2>();

        if (inputAction.Player.Look.activeControl != null && inputAction.Player.Look.activeControl.device is Gamepad)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(look.y, look.x, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(look.x, look.y, 0));
        }
    }
}
