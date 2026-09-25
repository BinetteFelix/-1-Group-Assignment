using System.Collections;
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
    public bool IsMoving => moveAction.ReadValue<Vector2>() != Vector2.zero;
    public float VerticalVelocity => RB.linearVelocity.y;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump;
    public static event System.Action OnPlayerJumped;

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

    [SerializeField] public PlayerCamera camera;
    Camera Main;

    Vector3 moveDirection;
    Rigidbody RB;

    [Header("Audio")]
    private PlayerAudio audioPlayer;
    [SerializeField] AudioSource slidingAudioSource;
    [SerializeField] AudioSource SFXAudioSource;

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
    private void Awake()
    {
        TowerTracker.Instance.SetPlayerTransform(transform);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        #region Get Components
        audioPlayer = GetComponent<PlayerAudio>();
        RB = GetComponent<Rigidbody>();
        #endregion

        RB.freezeRotation = true;
        readyToJump = true;

        #region Enable Input Actions
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        crouchAction.Enable();
        #endregion

        Main = Camera.main;
        SetAudioManagerReferences();

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
            OnCrouchDown();
        if (crouchAction.WasReleasedThisFrame())
            OnCrouchUp();

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

    #region CHECK METHODS
    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
            sprintAction.Disable();
            crouchAction.Disable();
            jumpAction.Disable();
        }
        else if (IsSliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && RB.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }
        else if (crouchAction.IsPressed() && crouchAction.enabled)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            if (crouchAction.WasPressedThisFrame())
                camera.DoFov(75, 0.25f);
        }
        else if (crouchAction.WasReleasedThisFrame())
            camera.DoFov(80, 0.25f);
        else if (IsGrounded && sprintAction.IsPressed())
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            if (sprintAction.WasPressedThisFrame())
                camera.DoFov(85, 0.25f);

        }
        else if (sprintAction.WasReleasedThisFrame())
            camera.DoFov(80, 0.25f);
        else if (IsGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            sprintAction.Enable();
            crouchAction.Enable();
            jumpAction.Enable();
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
    private bool CanJump()
    {
        return readyToJump && IsGrounded;
    }
    #endregion

    #region MOVEMENT METHODS
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

        if (!wallrunning)
            RB.useGravity = !OnSlope();
    }
    private void Jump()
    {
        OnPlayerJumped?.Invoke();

        exitingSlope = true;
        RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        RB.AddForce(transform.up * jumpForce, ForceMode.Impulse);


    }
    private void OnCrouchDown()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        RB.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    private void OnCrouchUp()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
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
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
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
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
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
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }
    void RotatePlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, Main.transform.eulerAngles.y, 0));
    }
    private void SetAudioManagerReferences()
    {
        PlayerSFX.Instance.playerMovement = this;
        PlayerSFX.Instance.audioPlayer = audioPlayer;
        PlayerSFX.Instance.slidingAudioSource = slidingAudioSource;

        CollectiblesAudio.Instance.worldInteractiveAudioSource = SFXAudioSource;
    }
    #endregion
}
