using UnityEngine;
using UnityEngine.InputSystem;

public class TemporaryCameraRotation : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivity = 0.1f;

    [Header("Limits")]
    [Range(-90f, 90f)] public float minPitch = -80f;
    [Range(-90f, 90f)] public float maxPitch = 80f;

    private float pitch;
    private Transform player;

    void Start()
    {
        player = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 delta = mouse.delta.ReadValue();

        pitch = Mathf.Clamp(pitch - delta.y * sensitivity, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        player.Rotate(0f, delta.x * sensitivity, 0f);
    }
}