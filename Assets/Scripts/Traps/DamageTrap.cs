using UnityEngine;

public class DamageTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;

    private void OnCollisionEnter(Collision collision) => TryDamage(collision.gameObject);
    private void OnCollisionStay(Collision collision) => TryDamage(collision.gameObject);

    private void OnTriggerEnter(Collider other) => TryDamage(other.gameObject);
    private void OnTriggerStay(Collider other) => TryDamage(other.gameObject);

    private void TryDamage(GameObject other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        player.TakeDamage(damage);
    }
}