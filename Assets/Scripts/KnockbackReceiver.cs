using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody rb;
    private TestPlayerMovement movement;
    private Coroutine activeKnockback;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<TestPlayerMovement>();
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

        if (movement != null) movement.enabled = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(force, mode);

        yield return new WaitForSeconds(duration);

        rb.useGravity = cachedUseGravity;
        if (movement != null) movement.enabled = true;
        activeKnockback = null;
    }
}