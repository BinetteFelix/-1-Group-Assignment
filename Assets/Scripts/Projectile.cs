using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Impact")]
    [SerializeField] private float knockbackForce = 8f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only react to the player — ignore walls, props, other physics objects
        TemporaryMovement player = collision.gameObject.GetComponent<TemporaryMovement>();
        if (player == null) return;

        Rigidbody targetRb = collision.rigidbody;
        if (targetRb != null)
        {
            Vector3 direction = collision.GetContact(0).normal * -1f; // push along travel direction
            targetRb.AddForce(direction * knockbackForce, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}