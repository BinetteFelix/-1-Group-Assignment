using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CrusherTrap : MonoBehaviour
{
    [Header("References")]
    public Transform crusherVisual;
    public CrusherHitbox crusherHitbox;

    [Header("Telegraph Nudge")]
    public float nudgeDistance = 0.2f;   // small dip before the real slam
    public float nudgeDuration = 0.1f;   // time to dip down
    public float nudgeHoldDuration = 0.3f; // time spent held at the nudge before slamming

    [Header("Slam Settings")]
    public float slamDistance = 3f;         // how far down it drops, relative to raised position
    public float slamDuration = 0.12f;      // fast drop
    public float slamHoldDuration = 0.4f;   // stays down
    public float riseDuration = 0.6f;       // returns to raised position
    public float rearmCooldown = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip nudgeSound;
    public AudioClip slamSound;
    public AudioClip riseSound;

    private enum State { Idle, Telegraphing, Slamming, Holding, Rising, Cooldown }
    private State state = State.Idle;

    private Vector3 raisedPos;

    void Awake()
    {
        if (crusherVisual != null)
            raisedPos = crusherVisual.localPosition;

        SetHitboxActive(false);
    }

    private void OnTriggerEnter(Collider other) => TryActivate(other);
    private void OnTriggerStay(Collider other) => TryActivate(other);

    private void TryActivate(Collider other)
    {
        if (state != State.Idle) return;
        if (other.GetComponent<PlayerHealth>() == null) return;

        StartCoroutine(SlamSequence());
    }

    private IEnumerator SlamSequence()
    {
        state = State.Telegraphing;
        PlaySound(nudgeSound);
        yield return MoveCrusher(0f, nudgeDistance, nudgeDuration);
        yield return new WaitForSeconds(nudgeHoldDuration);

        state = State.Slamming;
        PlaySound(slamSound);
        SetHitboxActive(true);
        yield return MoveCrusher(nudgeDistance, slamDistance, slamDuration);

        state = State.Holding;
        yield return new WaitForSeconds(slamHoldDuration);

        state = State.Rising;
        PlaySound(riseSound);
        SetHitboxActive(false);
        yield return MoveCrusher(slamDistance, 0f, riseDuration);

        state = State.Cooldown;
        yield return new WaitForSeconds(rearmCooldown);

        state = State.Idle;
    }

    private IEnumerator MoveCrusher(float fromOffset, float toOffset, float duration)
    {
        if (crusherVisual == null) yield break;

        Vector3 start = raisedPos + Vector3.down * fromOffset;
        Vector3 end = raisedPos + Vector3.down * toOffset;

        if (duration <= 0f)
        {
            crusherVisual.localPosition = end;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            crusherVisual.localPosition = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        crusherVisual.localPosition = end;
    }

    private void SetHitboxActive(bool active)
    {
        if (crusherHitbox == null) return;

        crusherHitbox.enabled = active;

        Collider hitboxCollider = crusherHitbox.GetComponent<Collider>();
        if (hitboxCollider != null)
            hitboxCollider.enabled = active;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}