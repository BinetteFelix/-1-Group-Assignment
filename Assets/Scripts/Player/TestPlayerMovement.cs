using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMovement : MonoBehaviour
{
   [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private PlayerData Data;

    Rigidbody RB;
    Vector2 moveInput;
    bool isGrounded;
    private PlayerInput playerInput;

    Camera Main;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
        Main = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        RotatePlayer();
    }
    private void FixedUpdate()
    {
        Debug.Log(RB.linearVelocity.y);
        MovePlayer();
        if (!isGrounded)
        {
            if (RB.linearVelocity.y < Data.maxFallSpeed)
                RB.linearVelocity = new Vector3(RB.linearVelocity.x, Data.maxFallSpeed, RB.linearVelocity.z);
            else
                RB.AddForce(Vector3.down * Data.gravityStrenght, ForceMode.Force);
        }
    }
    void OnJump()
    {

        if (isGrounded)
        {
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
            RB.AddForce(new Vector3(0, Data.jumpForce, 0), ForceMode.Impulse);
        }
    }
    void RotatePlayer()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, Main.transform.eulerAngles.y, 0));
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void MovePlayer()
    {
        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        direction.Normalize();
        RB.linearVelocity = new Vector3(direction.x * Data.runMaxSpeed, RB.linearVelocity.y, direction.z * Data.runMaxSpeed);
    }
    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    }


}