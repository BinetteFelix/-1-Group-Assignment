using UnityEngine;

public class KnockbackTrap : MonoBehaviour
{
    public enum KnockbackDirectionMode
    {
        Radial, // from trap center to target 
        Fixed   // uses fixedDirection below (in world space)
    }

    [Header("Knockback Settings")]
    public KnockbackDirectionMode directionMode = KnockbackDirectionMode.Fixed;
    public float knockbackForce = 10f;
    public float upwardBoost = 2f;
    public ForceMode forceMode = ForceMode.Impulse;
    public float knockbackLockoutDuration = 0.5f;

    [Header("Fixed Direction Settings")]
    public Vector3 fixedDirection = Vector3.forward;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip knockbackSound;

    private void OnTriggerEnter(Collider other)
    {
        KnockbackReceiver player = other.GetComponent<KnockbackReceiver>();
        if (player == null) return;

        Vector3 direction;

        if (directionMode == KnockbackDirectionMode.Fixed)
        {
            direction = fixedDirection.sqrMagnitude > 0.0001f ? fixedDirection.normalized : transform.forward;
        }
        else
        {
            Vector3 diff = other.transform.position - transform.position;
            diff.y = 0f;
            direction = diff.sqrMagnitude > 0.0001f ? diff.normalized : -transform.forward;
        }

        direction = (direction + Vector3.up * (upwardBoost / knockbackForce)).normalized;

        player.ApplyKnockback(direction * knockbackForce, forceMode, knockbackLockoutDuration);

        PlaySound(knockbackSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        if (directionMode == KnockbackDirectionMode.Fixed)
        {
            Vector3 dir = fixedDirection.sqrMagnitude > 0.0001f ? fixedDirection.normalized : transform.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, dir * 4f);
        }
    }
}