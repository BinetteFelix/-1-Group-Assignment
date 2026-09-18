using UnityEngine;

public class CannonTrap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Firing")]
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float startDelay = 0f; // useful for staggering multiple cannons

    private float timer;

    void Start()
    {
        timer = fireInterval - startDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            timer = 0f;
            Fire();
        }
    }

    private void Fire()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody projRb = proj.GetComponent<Rigidbody>();
        if (projRb != null)
            projRb.linearVelocity = shootPoint.forward * projectileSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        if (shootPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(shootPoint.position, shootPoint.forward * 2f);
    }
}