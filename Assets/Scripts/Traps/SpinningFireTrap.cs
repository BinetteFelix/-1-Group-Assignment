using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpinningFireTrap : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 30f; // degrees per second
    public Vector3 rotationAxis = Vector3.up;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate; // smooths visual rotation between physics steps
    }

    void FixedUpdate()
    {
        Quaternion delta = Quaternion.Euler(rotationAxis.normalized * (rotationSpeed * Time.fixedDeltaTime));
        rb.MoveRotation(rb.rotation * delta);
    }
}