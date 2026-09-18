using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerOBJ;
    private Rigidbody RB;
    private PlayerMovement movement;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    private Vector2 moveInput;
    [SerializeField] private InputAction slideAction;
    [SerializeField] private InputAction moveAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction.Enable();
        slideAction.Enable();
        RB = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();

        startYScale = playerOBJ.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if (slideAction.WasPressedThisFrame() && moveInput != Vector2.zero)
            StartSlide();
        if (slideAction.WasReleasedThisFrame() && movement.IsSliding)
            StopSlide();
    }
    private void FixedUpdate()
    {
        if (movement.IsSliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        movement.IsSliding = true;

        playerOBJ.localScale = new Vector3(playerOBJ.localScale.x, slideYScale, playerOBJ.localScale.z);

        RB.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (!movement.OnSlope() || RB.linearVelocity.y > -0.1f)
        {
            RB.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            RB.AddForce(movement.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }
        if (slideTimer <= 0)
        {
            StopSlide();
        }

    }
    private void StopSlide()
    {
        movement.IsSliding = false;
        playerOBJ.localScale = new Vector3(playerOBJ.localScale.x, startYScale, playerOBJ.localScale.z);
    }
}
