using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerData Data;
    // [SerializeField] private HealthManager Health;
    private Rigidbody _rb;

    #region INPUT PARAMETERS
    public Vector2 _moveInput;
    public bool _jumpInputDown;
    public bool _jumpInputUp;
    public float LastPressedJumpTime { get; private set; }
    #endregion

    #region STATE PARAMETERS
    // Action variables for diverse player movements
    // Publicly accessable variables that can only be read through other scripts but not changed
    // They only allow private writes
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    // World Timers, could be set to private
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float WallSlideTime { get; private set; }

    // Jumping
    private bool _isJumpCut;
    private bool _isJumpFalling;

    // Wall Jumping
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Vector3 _wallCheckSize = new Vector3(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    #endregion

    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction jumpAction;

    private void Start()
    {
        moveAction.Enable();
        jumpAction.Enable();
        _rb = GetComponent<Rigidbody>();
        //SetGravityScale(Data.gravityScale);
    }
    private void Update()
    {
        Debug.Log(CanJump());

        //Debug.Log(LastOnWallLeftTime);

        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region INPUTHANDLER
        _moveInput.x = moveAction.ReadValue<Vector2>().x;
        _moveInput.y = moveAction.ReadValue<Vector2>().y;

        _jumpInputDown = jumpAction.WasPressedThisFrame();
        _jumpInputUp = jumpAction.WasReleasedThisFrame();

        if (_jumpInputDown)
            OnJumpInput();

        if (_jumpInputUp)
            OnJumpUpInput();
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {

            // Right Wall Check
           /* if (Physics.OverlapBoxNonAlloc(_rightWallCheckPoint.position, _wallCheckSize / 2, walls, Quaternion.identity) > 0 && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            // Left Wall Check
            if (Physics.OverlapBoxNonAlloc(_leftWallCheckPoint.position, _wallCheckSize / 2, walls, Quaternion.identity) > 0 && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;*/

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion

        #region JUMP CHECKS
        if ((IsJumping || IsWallJumping) && _rb.linearVelocity.y < -0.5f)
        {
            IsJumping = false;

            _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;

            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;
        }

        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        //Wall Jump
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;

            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            IsSliding = true;
        }
        else
        {
            IsSliding = false;
            WallSlideTime = Data.maxWallSlideTime;
        }
        #endregion

        #region GRAVITY
        if (IsSliding)
        {
            if ((WallSlideTime < 0 && LastOnWallLeftTime > 0 && _moveInput.x < 0) || (WallSlideTime < 0 && LastOnWallRightTime > 0 && _moveInput.x > 0))
                SetGravityScale(Data.gravityScale);
            else
                SetGravityScale(0);
        }
        else if (_rb.linearVelocity.y < 0 && _moveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -Data.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (_rb.linearVelocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Max(_rb.linearVelocity.y, -Data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(Data.gravityScale);
        }
        #endregion*/
    }
    private void FixedUpdate()
    {
        if (IsWallJumping)
            Run(Data.wallJumpRunlerp);
        else
            Run(3);

        if (IsSliding)
        {
            Slide();
            WallSlideTime -= Time.deltaTime;
        }
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }
    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        _rb.AddForce(Vector3.down * scale);
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.y * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(_rb.linearVelocity.z, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(_rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(_rb.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - _rb.linearVelocity.x;
        float movement = speedDif * accelRate;

        _rb.AddForce(movement * Vector3.right * 100, ForceMode.Force);
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        float force = Data.jumpForce;
        if (_rb.linearVelocity.y < 0)
            force -= _rb.linearVelocity.y;

        _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector3 force = new Vector3(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(_rb.linearVelocity.y) != Mathf.Sign(force.x))
            force.x -= _rb.linearVelocity.x;

        if (_rb.linearVelocity.y < 0)
            force.y -= _rb.linearVelocity.y;

        _rb.AddForce(force, ForceMode.Impulse);
        #endregion
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        if (WallSlideTime > 0)
        {
            if (_rb.linearVelocity.y > 0)
            {
                _rb.AddForce(-_rb.linearVelocity.y * (Vector3.up / 10), ForceMode.Impulse);
            }

            float speedDif = Data.slideSpeed - _rb.linearVelocity.y;
            float movement = speedDif * Data.slideAccel;

            movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

            _rb.AddForce(movement * Vector3.up);
        }
    }
    #endregion

    #region CHECK METHODS
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }
    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
            (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }
    private bool CanJumpCut()
    {
        return IsJumping && _rb.linearVelocity.y > 0;
    }
    private bool CanWallJumpCut()
    {
        return IsWallJumping && _rb.linearVelocity.y > 0;
    }
    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        
    }
    #endregion

    #region TEMPORARY METHODS
    private void OnTriggerStay(Collider other)
    {
        GameObject obj = other.gameObject;
        if (obj != null && obj.layer == 6)
        {
            LastOnGroundTime = 0.1f;
        }
    }
    #endregion
}