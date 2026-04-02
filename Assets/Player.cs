using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Move Speed")]
    [SerializeField] private float topSpeed;
    [SerializeField] private float acceleration; // acceleration is very small because it is multiplied by Time.fixedDeltaTime in order to make it frame independent
    // Time.fixedDeltaTime by default is 0.2
    // this means acceleration is always divided by 5, making it likely to need to be of an higher value than topSpeed.

    private bool isMoving = false; // for animation

    private InputSystem_Actions inputAction;
    private Rigidbody rb;

    private void Awake()
    {
        inputAction = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>(); // yes, I want it o crash if it's not found
    }

    private void OnEnable()
    {
        inputAction.Player.Move.Enable();
        inputAction.Player.Move.performed += OnMovePerformed;
        inputAction.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        inputAction.Player.Move.canceled -= OnMoveCanceled;
        inputAction.Player.Move.performed -= OnMovePerformed;
        inputAction.Player.Move.Disable();
    }

    void FixedUpdate()
    {
        Vector2 move = inputAction.Player.Move.ReadValue<Vector2>();

        rb.linearVelocity = new Vector3(
            Mathf.MoveTowards( // x
                rb.linearVelocity.x,
                topSpeed * move.x,
                acceleration * Time.fixedDeltaTime
                ),
            rb.linearVelocity.y, // y
            Mathf.MoveTowards( // z
                rb.linearVelocity.z,
                topSpeed * move.y,
                acceleration * Time.fixedDeltaTime
                ));
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        isMoving = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
    }
}
