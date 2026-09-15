using UnityEngine;

[CreateAssetMenu(fileName = "New Player data", menuName = "ScriptableObjects/Player data", order = 0)]
public class PlayerData : ScriptableObject
{
    [Header("Graviy")]
    [HideInInspector] public float gravityStrenght;
    [HideInInspector] public float gravityScale;
    [Space(5)]
    public float fallGravityMult;
    public float maxFallSpeed;
    [Space(5)]
    public float fastFallGravityMult;
    public float maxFastFallSpeed;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    public float runDecceleration;
    [HideInInspector] public float runAccelAmount;
    [HideInInspector] public float runDeccelAmount;
    [Space(5)]
    [Range(0f, 1f)] public float accelInAir;
    [Range(0f, 1f)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    public float jumpCutGravityMult;
    [Space(5)]
    [Range(0f, 1f)] public float jumpHangGravityMult;
    [Space(5)]
    public float jumpHangTimeThreshold;
    [Space(5)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunlerp;
    [Range(0f, 1.5f)] public float wallJumpTime;
    public bool doTurnOnWalljump;

    [Space(20)]

    [Header("Slide")]
    public float maxWallSlideTime;
    public float slideSpeed;
    public float slideAccel;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;

    [Space(20)]

    [Header("Health & Damage")]
    public float baseHealth;
    public float levelHealthRatio;
    [Space(15)]
    public float baseDamageMult;
    public float criticalHitMult;

    //Unity Callback, called when the inspector updates
    private void OnValidate()
    {
        gravityStrenght = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        gravityScale = gravityStrenght / Physics2D.gravity.y;

        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        jumpForce = Mathf.Abs(gravityStrenght) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed * 1.5f);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}