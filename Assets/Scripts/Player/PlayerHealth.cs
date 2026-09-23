using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 0.75f;

    private float invincibilityTimer = 0f;
    public bool IsInvincible => invincibilityTimer > 0f;

    void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible || amount <= 0) return;

        HeartsUI.Instance.RemoveHeart(amount);
        invincibilityTimer = invincibilityDuration;
    }

    public void Kill()
    {
        // Instakill that bypasses grace period
        HeartsUI.Instance.RemoveHeart(HeartsUI.Instance.maxHP);
    }
}