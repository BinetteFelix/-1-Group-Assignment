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

    [SerializeField] private PlayerCamera camera;
    Camera Main;

    Vector3 moveDirection;
    Rigidbody RB;

    [Header("Audio")]
    private PlayerAudio audioPlayer;
    public AudioClip jumpSound;                                             // Making a public field so we can add the actual audio file through Inspector.
    public AudioClip landingSound;                                          // Making a public field so we can add the actual audio file through Inspector.
    public AudioClip hardLandingSound;                                      // Public field for another landing sound (when you land with higher downward velocity). 
    public AudioClip[] footstepSounds;                                      // Public field for footstep sounds. Made it into an array so that we can have different footstep sounds playing in randomized order (makes it sound less flat and boring).
    public AudioClip[] gruntSounds;                                         // Public field for grunt sounds. Made as array to play random grunt sound out of three on jump.
    private bool wasGroundedLastFrame;                                      // Making a bool variable that will check for us if the player was grounded last frame or not. We need to know this so we can identify when the player lands after a jump.
    private float lastLandingSoundTime;                                     // Variable for last time a landing sound played so we can have a cooldown check on it.
    private float landingSoundCooldown = 0.2f;                              // The actual cooldown time on the landing time.
    public float hardLandingThreshold = -15f;                               // The value of how much velocity the player must have at the very least to play the hard landing sound.
    private float footstepSoundCd = 0.45f;                                  // Cooldown on when next footstep sound can play so it doesn't play continously without no pause in between actual footsteps. More realistic.
    private float lastFootstepSound;                                        // Variable to check when the last footstep sound was played so that we can actually use the cooldown we created for the footsteps.
    private int lastFootstepIndex = -1;                                     // Variable for storing the last index that was used that plays one of the audio files for footstep sound. Starts as -1 so that we don't accidentally play the index with value 0 twice in the beginning.
    private int lastGruntIndex = -1;                                        // Same idea as the starting value of index for footstep sounds.

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
        audioPlayer = GetComponent<PlayerAudio>();
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        readyToJump = true;
        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        crouchAction.Enable();
        Main = Camera.main;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(RB.linearVelocity.y);
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

        #region Landing AUDIO
        float timeSinceLastLanding = Time.time - lastLandingSoundTime;
        if (!wasGroundedLastFrame && IsGrounded && timeSinceLastLanding >= landingSoundCooldown)                        /* The variable wasGroundedLastFrame is by default a false since I didn't explicitly say if it was false or true when creating it. */
        {                                                                                                               /* We check if it stays false and also if the isGrounded variable turns to true. As soon as it matches up it plays the sound in that frame. */
            float verticalVelocity = RB.linearVelocity.y;                                                               /* Added a cooldown for landing sound so it does trigger multiple times when ground check flickers (raycasting being unreliable). */
            if (verticalVelocity > hardLandingThreshold)                                                             /* Wanted to add a "harder landing sound" when landing from a specific height (or specific velocity).*/
            {                                                                                                           /* Checks how fast the player is going vertically and plays a harder land sound if player is falling from a higher place.*/
                audioPlayer.PlaySFXAudio(landingSound);                                                                 // Plays hard landing sound if conditions are met.
            }
            else
            {
                audioPlayer.PlaySFXAudio(hardLandingSound);                                                                 // We call on PlayOneShot to play the sound once using the Audio Source reference and playing the audio file that is in the Audio Clip field.
            }
            lastLandingSoundTime = Time.time;                                                                           // Change the variable lastLandingSoundTime to equal the current time that has passed so we can correctly check how much time has passed since the last time the landing sound played.

        }
        wasGroundedLastFrame = IsGrounded;                                                                              // We change the value of wasGroundedLastFrame to the same value as the player isGrounded check to not play the landing sound outside of cases where the player isn't landing from a jump.
        #endregion

        #region Footstep AUDIO
        float timeSinceLastFootstep = Time.time - lastFootstepSound;
        if (moveAction.ReadValue<Vector2>() != Vector2.zero && IsGrounded && timeSinceLastFootstep >= footstepSoundCd)  // Checks if player is moving, is grounded and if enough time has elapsed since last time the footstep sound was played (so it doesn't spam the sound).
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);                                                   // Made a dynamic array that can check how many indeces we have within the array dependent on how many audio files we input through the Inspector. Stores the index value in randomIndex.
            while (randomIndex == lastFootstepIndex)                                                                    // Created a while loop so we can check which sound out of the 4 elements in the array was played last. Doing this to we don't get any repeat sounds.
            {
                randomIndex = Random.Range(0, footstepSounds.Length);                                                   // If the statement in the while loop stays true than we continue looking for another random index until it's not the same.
            }

            audioPlayer.PlaySFXAudio(footstepSounds[randomIndex]);                                                      // Plays the audio file of the random index that was selected.
            lastFootstepSound = Time.time;                                                                              // Resets the timer for the last played footstep variable. Basically makes out cooldown always valid since it now counts from when last the sound was played and not from 0 like in the beginning.
            lastFootstepIndex = randomIndex;                                                                            // Setting the value of the randomIndex (that just played) to the last played index variable. Next time it checks for repeat sounds it will know which one was played last
        }
        #endregion
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
                camera.DoFov(75);
        }
        else if (crouchAction.WasReleasedThisFrame())
            camera.DoFov(80);
        else if (IsGrounded && sprintAction.IsPressed())
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            if (sprintAction.WasPressedThisFrame())
                camera.DoFov(85);

        }
        else if (sprintAction.WasReleasedThisFrame())
            camera.DoFov(80);
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
        exitingSlope = true;
        RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
        RB.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        #region Jump Audio
        audioPlayer.PlaySFXAudio(jumpSound);                                                           // Plays the sound of the jump when the spacebar key is pressed down without it being interrupted by new spacebar inputs or interuppting previous spacebar input.
                                                                                                       // Sound basically doesn't restart on a new spacebar input.
        int randomGrunt = Random.Range(0, gruntSounds.Length);                                         // Getting random index to play of three grunt sounds when jumping.               
        while (randomGrunt == lastGruntIndex)                                                          // While loop to make sure that the same grunt sounds doesn't play twice in a row.
        {
            randomGrunt = Random.Range(0, gruntSounds.Length);                                         // If the same index gets picked again, then it picks another index again randomly. Continues looking for different index so same grunt sound is not played twice. 
        }
        audioPlayer.PlaySFXAudio(gruntSounds[randomGrunt]);                                            // Playing grunt sounds through Audio Source.
        lastGruntIndex = randomGrunt;                                                                  // Remembering which grunt sound was played last by equaling it to the value of the grunt sound that was just played.
        #endregion
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
    #endregion
}
