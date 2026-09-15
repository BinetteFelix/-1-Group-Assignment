using UnityEngine;

public class KnockbackTrap : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float upwardBoost = 2f; // Launch with small arc
    public ForceMode forceMode = ForceMode.Impulse;
    public float knockbackLockoutDuration = 0.25f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody targetRb = other.GetComponent<Rigidbody>();
        if (targetRb == null) return;

        // Flatten to horizontal plane so side hits knock sideways, not up
        Vector3 diff = other.transform.position - transform.position;
        diff.y = 0f;

        if (diff.sqrMagnitude < 0.0001f)
            diff = -transform.forward; // fallback if positions overlap

        Vector3 direction = diff.normalized + Vector3.up * (upwardBoost / knockbackForce);
        direction = direction.normalized;

        // Zero existing velocity so knockback feels consistent every time
        targetRb.linearVelocity = Vector3.zero;
        targetRb.AddForce(direction * knockbackForce, forceMode);

        // Tell the movement script to back off for a moment
        var movement = other.GetComponent<TemporaryMovement>();
        if (movement != null)
            movement.ApplyKnockbackLock(knockbackLockoutDuration);
    }
}