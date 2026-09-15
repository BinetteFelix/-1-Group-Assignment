using UnityEngine;
using UnityEngine.InputSystem;

public class TemporaryMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpHeight = 2f;

    private Rigidbody rb;
    private float knockbackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            return; // let physics carry the knockback velocity uninterrupted
        }

        var kb = Keyboard.current;
        if (kb == null) return;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        Vector3 move = transform.right * x + transform.forward * z;
        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);

        if (kb.spaceKey.wasPressedThisFrame && rb.linearVelocity.y <= 0f)
            rb.AddForce(Vector3.up * Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude), ForceMode.Impulse);
    }

    public void ApplyKnockbackLock(float duration)
    {
        knockbackTimer = Mathf.Max(knockbackTimer, duration);
    }
}