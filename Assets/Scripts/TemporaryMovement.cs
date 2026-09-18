using UnityEngine;
using UnityEngine.InputSystem;

public class TemporaryMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpHeight = 2f;
    public float acceleration = 50f;

    [Header("Knockback Recovery")]
    // How long it takes to go from no control back to full control after a hit
    private float controlBlockTimer = 0f;
    private float controlBlockDuration = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        Vector3 wishDir = (transform.right * x + transform.forward * z).normalized;
        Vector3 targetVelocity = wishDir * speed;
        Vector3 currentHorizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // 0 = no control (right after a hit), 1 = full control
        float controlFactor = 1f;
        if (controlBlockTimer > 0f)
        {
            controlBlockTimer -= Time.deltaTime;
            controlFactor = controlBlockDuration > 0f
                ? 1f - Mathf.Clamp01(controlBlockTimer / controlBlockDuration)
                : 1f;
        }

        Vector3 velocityDelta = (targetVelocity - currentHorizontal) * controlFactor;
        velocityDelta = Vector3.ClampMagnitude(velocityDelta, acceleration * Time.deltaTime);

        rb.linearVelocity += new Vector3(velocityDelta.x, 0f, velocityDelta.z);

        if (kb.spaceKey.wasPressedThisFrame && rb.linearVelocity.y <= 0f)
            rb.AddForce(Vector3.up * Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude), ForceMode.Impulse);
    }

    public void ApplyKnockbackLock(float duration)
    {
        controlBlockTimer = Mathf.Max(controlBlockTimer, duration);
        controlBlockDuration = Mathf.Max(controlBlockDuration, duration);
    }
}