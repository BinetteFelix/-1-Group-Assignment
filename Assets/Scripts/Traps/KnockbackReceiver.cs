using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMovement movement;
    private Coroutine activeKnockback;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
    }

    public void ApplyKnockback(Vector3 force, ForceMode mode = ForceMode.Impulse, float controlLockDuration = 0.5f)
    {
        if (activeKnockback != null)
            StopCoroutine(activeKnockback);

        activeKnockback = StartCoroutine(KnockbackRoutine(force, mode, controlLockDuration));
    }

    private IEnumerator KnockbackRoutine(Vector3 force, ForceMode mode, float duration)
    {
        bool cachedUseGravity = rb.useGravity;
        float cachedDrag = rb.linearDamping;

        if (movement != null) movement.enabled = false;
        rb.useGravity = true;   // don't let a frozen "useGravity = false" from a slope strand the player mid-air
        rb.linearDamping = 0f;  // don't let leftover ground drag eat the knockback

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, mode);

        yield return new WaitForSeconds(duration);

        rb.useGravity = cachedUseGravity;
        rb.linearDamping = cachedDrag;
        if (movement != null) movement.enabled = true;
        activeKnockback = null;
    }
}