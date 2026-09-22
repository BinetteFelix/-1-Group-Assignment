using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Impact")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackLockoutDuration = 0.3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        KnockbackReceiver player = collision.gameObject.GetComponent<KnockbackReceiver>();
        if (player == null) return;

        Vector3 direction = collision.GetContact(0).normal * -1f;
        player.ApplyKnockback(direction * knockbackForce, ForceMode.Impulse, knockbackLockoutDuration);

        Destroy(gameObject);
    }
}