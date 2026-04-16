using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable, ITriggerTurrets
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

    [Header("Misc")]
    [SerializeField] public Transform lastCheckpoint; // need to be accessed by the checkpoint script
    [SerializeField] private float maxHealth;

    [Header("Step Settings")]
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepSmooth;
    [SerializeField] private float stepCheckDistance;

    [SerializeField] private GameObject bulletExhaust;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float range;
    [SerializeField] private Camera playerCamera;

    private float timeBetweenShotsCounter = 0;
    private float health;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private bool jumpPressed = false;
    private bool jumpReleased = false;
    private bool jumpHeld = false;

    private bool isMoving = false; // for animation
    private bool isJumping = false;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private InputSystem_Actions inputAction;
    private Rigidbody rb;

    // Unity

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
    private void Awake()
    {
        inputAction = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>(); // yes, I want it to crash if it's not found
        health = maxHealth;
    }

    private void OnEnable()
    {
        inputAction.Player.Attack.Enable();
        inputAction.Player.Attack.performed += OnShoot;
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
        inputAction.Player.Attack.performed -= OnShoot;
        inputAction.Player.Attack.Disable();
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
        StepClimb();
        if (timeBetweenShotsCounter > 0)
        {
            timeBetweenShotsCounter -= Time.fixedDeltaTime;
        }
      
    }

    // Events
    private void OnShoot(InputAction.CallbackContext context)
    {
      
        if (timeBetweenShotsCounter > 0) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); ;//this is my line from here to 
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, range))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(range);
        }

      
        Vector3 direction = (targetPoint - bulletExhaust.transform.position).normalized;

        
        Quaternion rotation = Quaternion.LookRotation(direction);//to here

        GameObject instance = Instantiate(bullet, bulletExhaust.transform.position, rotation);
       // instance.GetComponent<Bullet>().FireBullet();

        timeBetweenShotsCounter = timeBetweenShots;
    }

    
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

        Vector3 moveDir = transform.right * move.x + transform.forward * move.y;

        rb.linearVelocity = new Vector3(
            Mathf.MoveTowards(
                rb.linearVelocity.x,
                moveDir.x * topSpeed,
                acceleration * Time.fixedDeltaTime
            ),
            yVelocity,
            Mathf.MoveTowards(
                rb.linearVelocity.z,
                moveDir.z * topSpeed,
                acceleration * Time.fixedDeltaTime
            ));
    }

    private void StepClimb()
    {
        Vector3 originLow = transform.position + Vector3.up * 0.05f;
        Vector3 originHigh = transform.position + Vector3.up * stepHeight;

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

       
        if (Physics.Raycast(originLow, forward, out RaycastHit hitLow, stepCheckDistance))
        {
            
            if (!Physics.Raycast(originHigh, forward, stepCheckDistance))
            {
               
                rb.position += Vector3.up * stepSmooth;
            }
        }
    }

   
    
    

    private void Look()
    {
        Vector2 look = inputAction.Player.Look.ReadValue<Vector2>();

        float mouseX = look.x;
        float mouseY = look.y;

        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

      
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
        
        
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

      //  bullet.transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
      // bullet.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    }


    public void Kill()
    {
        transform.position = lastCheckpoint.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            Kill();
        }
    }
}

