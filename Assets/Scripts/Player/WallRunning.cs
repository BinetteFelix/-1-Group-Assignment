using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    [SerializeField] private InputAction moveAction;
    private Vector2 moveInput;
    [SerializeField] private InputAction climpUpAction;
    [SerializeField] private InputAction climpDownAction;
    private bool upwardsRunning;
    private bool downwardsRunning;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeftTouch;
    private bool wallRightTouch;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement movement;
    private Rigidbody RB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.Enable();
        climpUpAction.Enable();
        climpDownAction.Enable();
        RB = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if (movement.wallrunning)
            WallRunningMovement();
    }

    #region CHECK METHODS
    private void CheckForWall()
    {
        wallRightTouch = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeftTouch = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }
    

    private void StateMachine()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        upwardsRunning = climpUpAction.IsPressed();
        downwardsRunning = climpDownAction.IsPressed();
        if ((wallLeftTouch || wallRightTouch) && moveInput.y > 0 && AboveGround())
        {
            if (!movement.wallrunning)
                StartWallRun();
        }
        else
        {
            if (movement.wallrunning)
                StopWallRun();
        }
    }
    #endregion

    private void StartWallRun()
    {
        movement.wallrunning = true;
    }
    private void WallRunningMovement()
    {
        RB.useGravity = false;
        RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);

        Vector3 wallNormal = wallRightTouch ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        RB.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, wallClimbSpeed, RB.linearVelocity.z);
        if (downwardsRunning)
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, -wallClimbSpeed, RB.linearVelocity.z);

        if (!(wallLeftTouch && moveInput.x > 0) && !(wallRightTouch && moveInput.x < 0))
            RB.AddForce(-wallNormal * 100f, ForceMode.Force);
    }
    private void StopWallRun()
    {
        movement.wallrunning = false;
    }
}