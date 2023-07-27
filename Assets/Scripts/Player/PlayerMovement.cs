using UnityEngine;
using TMPro;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float maxYSpeed;
    
    public float dashSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Sliding")]
    public float minSlideSpeed;
    public float slideDeceleration;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Wall Running")] 
    public float wallRunSpeed;
    public float wallDistance;
    public float wallRunGravityScale;
    public float wallRunCooldown;
    private Vector3 lastNormal;
    private bool canWallRunSameSurface;
    private RaycastHit wallHit;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        dashing,
        wallrunning,
        air
    }
    
    public bool dashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (state ==  MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && (grounded || state == MovementState.wallrunning))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {   
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            moveSpeed = dashSpeed;
        }
        
        // Mode - Sliding
        else if (rb.velocity.magnitude > minSlideSpeed && Input.GetKey(crouchKey))
        {
            state = MovementState.sliding;
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        
        // Mode - Wallrunning
        else if (checkWallRun())
        {
            state = MovementState.wallrunning;
            moveSpeed = wallRunSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        
        if (state == MovementState.wallrunning)
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, wallHit.normal);
            rb.velocity -= Physics.gravity * ((1 - wallRunGravityScale) * Time.fixedDeltaTime);
        } else if (state == MovementState.sliding)
        {
            moveDirection -= moveDirection.normalized * (slideDeceleration * Time.fixedDeltaTime);
        }
        else moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * (moveSpeed * 20f), ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if(grounded)
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        //Quang: Changed from 0f to 2.5f for higher jump
        rb.velocity = new Vector3(rb.velocity.x, 2.5f, rb.velocity.z);

        Vector3 jumpVector = transform.up;

        if (state == MovementState.wallrunning)
        {
            jumpVector += wallHit.normal;
            lastNormal = wallHit.normal;
            canWallRunSameSurface = false;
            Invoke(nameof(ResetWallRun), wallRunCooldown);
        }
        
        rb.AddForce(jumpVector * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void ResetWallRun()
    {
        canWallRunSameSurface = true;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private bool checkWallRun()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, orientation.right, out hit, wallDistance, whatIsGround)
            || Physics.Raycast(transform.position, -orientation.right, out hit, wallDistance, whatIsGround))
        {
            if (!canWallRunSameSurface && hit.normal == lastNormal)
            {
                return false;
            }
            wallHit = hit;
            return true;
        }
        return false;
    }
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}