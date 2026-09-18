using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayMovement : MonoBehaviour
{
    [Header("Mouse Tilt")]
    public float maxAngle = 15f;
    public float speed = 5f;

    [Header("Click Animation")]
    public Animator animator;
    public string triggerName = "click";

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {

        MouseMove();
        MouseClick();
    }

    private void MouseMove()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        float normalizedX = (mousePos.x / Screen.width) * 2f - 1f;

        float targetAngle = -normalizedX * maxAngle; 
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, targetAngle, 0);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * speed);
    }

    private void MouseClick()
    {

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            animator.SetTrigger(triggerName);
        }
    }

}
