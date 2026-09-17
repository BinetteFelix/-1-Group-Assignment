using UnityEngine;

[CreateAssetMenu(fileName = "New Player data", menuName = "ScriptableObjects/Player data", order = 0)]
public class PlayerData : ScriptableObject
{
    [Header("Graviy")]
    public float gravityStrenght;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;

    [Space(20)]

    [Header("Jump")]
    public float jumpForce;
    public float maxFallSpeed;

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

    
}