using UnityEngine;

public class CrusherHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => TryKill(other);
    private void OnTriggerStay(Collider other) => TryKill(other);
    [SerializeField] private bool killToggle = true;

    private void TryKill(Collider other)
    {
        if (!killToggle) return;
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null) return;

        player.Kill();
    }
}