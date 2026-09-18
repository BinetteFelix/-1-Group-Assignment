using UnityEngine;

public class KnockbackTrap : MonoBehaviour
{
    public enum KnockbackDirectionMode
    {
        Radial, // from trap center to target
        Fixed   // always along the trap's own forward axis
    }

    [Header("Knockback Settings")]
    public KnockbackDirectionMode directionMode = KnockbackDirectionMode.Fixed;
    public float knockbackForce = 10f;
    public float upwardBoost = 2f;
    public ForceMode forceMode = ForceMode.Impulse;
    public float knockbackLockoutDuration = 0.25f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody targetRb = other.GetComponent<Rigidbody>();
        if (targetRb == null) return;

        Vector3 direction;

        if (directionMode == KnockbackDirectionMode.Fixed)
        {
            direction = transform.forward; // orient this in the Inspector to point away from the wall
        }
        else
        {
            Vector3 diff = other.transform.position - transform.position;
            diff.y = 0f;
            direction = diff.sqrMagnitude > 0.0001f ? diff.normalized : -transform.forward;
        }

        direction = (direction + Vector3.up * (upwardBoost / knockbackForce)).normalized;

        targetRb.linearVelocity = Vector3.zero;
        targetRb.AddForce(direction * knockbackForce, forceMode);

        var movement = other.GetComponent<TemporaryMovement>();
        if (movement != null)
            movement.ApplyKnockbackLock(knockbackLockoutDuration);
    }

    private void OnDrawGizmosSelected()
    {
        if (directionMode == KnockbackDirectionMode.Fixed)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}