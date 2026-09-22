using UnityEngine;

public class CrusherHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => TryKill(other);
    private void OnTriggerStay(Collider other) => TryKill(other);

    private void TryKill(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        player.Kill();
    }
}