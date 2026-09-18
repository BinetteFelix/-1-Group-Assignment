using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float wallRunSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    [SerializeField] InputAction jumpAction;
    [SerializeField] InputAction moveAction;
    [SerializeField] InputAction sprintAction;
    [SerializeField] InputAction crouchAction;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool IsGrounded { get; private set; }

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;
    [SerializeField] PlayerData Data;

    Camera Main;

    Vector3 moveDirection;
    Rigidbody RB;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air
    }

    public bool IsSliding { get; set; }
    public bool wallrunning { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        readyToJump = true;
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        crouchAction.Enable();
        Main = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        SpeedControl();
        StateHandler();
        RotatePlayer();

        #region INPUT HANDLER
        if (crouchAction.WasPressedThisFrame())
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            RB.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (crouchAction.WasReleasedThisFrame())
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
        if (jumpAction.IsPressed() && CanJump())
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        #endregion

        if (IsGrounded)
            RB.linearDamping = groundDrag;
        else
            RB.linearDamping = 0f;
    }
    private void FixedUpdate()
    {
        MovePlayer();
        Gravity();
    }
    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if (IsSliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && RB.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        else if (crouchAction.IsPressed())
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (IsGrounded && sprintAction.IsPressed())
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (IsGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
            state = MovementState.air;

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
            moveSpeed = desiredMoveSpeed;

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    #region MOVEMENT METHODS
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * moveAction.ReadValue<Vector2>().y + orientation.right * moveAction.ReadValue<Vector2>().x;

        if (OnSlope() && !exitingSlope)
        {
            RB.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20, ForceMode.Force);
            if (RB.linearVelocity.y > 0)
                RB.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (IsGrounded)
            RB.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            RB.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        RB.useGravity = !OnSlope();
    }
    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (RB.linearVelocity.y > desiredMoveSpeed)
                RB.linearVelocity = RB.linearVelocity.normalized * moveSpeed;
        }

        Vector3 flatVel = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        if (flatVel.magnitude > desiredMoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            RB.linearVelocity = new Vector3(limitedVel.x, RB.linearVelocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        exitingSlope = true;
        RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        RB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.6f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    #endregion

    #region INPUT CHECK
    private bool CanJump()
    {
        return readyToJump && IsGrounded;
    }
    #endregion

    #region HELP METODS
    private void Gravity()
    {
        if (!IsGrounded && !OnSlope())
        {
            if (RB.linearVelocity.y < Data.maxFallSpeed)
                RB.linearVelocity = new Vector3(RB.linearVelocity.x, Data.maxFallSpeed, RB.linearVelocity.z);
            else
                RB.AddForce(Vector3.down * Data.gravityStrenght, ForceMode.Force);
        }

    }
    void RotatePlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, Main.transform.eulerAngles.y, 0));
    }
    #endregion
}
