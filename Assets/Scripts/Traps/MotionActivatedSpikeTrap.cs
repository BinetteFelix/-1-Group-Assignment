using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider))]
public class MotionActivatedSpikeTrap : MonoBehaviour
{
    [Header("References")]
    public Transform spikeVisual;
    public DamageTrap spikeHitbox;

    [Header("Spike Heights")]
    public float pokeHeight = 0.15f;
    public float extendedHeight = 1f;

    [Header("Timing")]
    public float pokeDuration = 0.15f;         // rise from hidden to poke
    public float pokeHoldDuration = 0.2f;      // brief telegraph pause
    public float lungeDuration = 0.08f;        // fast strike to full extend
    public float extendedHoldDuration = 0.5f;  // stays out, damaging
    public float retractDuration = 0.3f;       // goes back down
    public float rearmCooldown = 0.5f;         // wait after hidden before it can trigger again

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip nudgeSound;
    public AudioClip lungeSound;
    public AudioClip retractSound;

    private enum State { Idle, Poking, Lunging, Retracting, Cooldown }
    private State state = State.Idle;

    private Vector3 basePos; // spikeVisual's resting local position

    void Awake()
    {
        if (spikeVisual != null)
            basePos = spikeVisual.localPosition;

        SetHitboxActive(false);
    }

    private void OnTriggerEnter(Collider other) => TryActivate(other);
    private void OnTriggerStay(Collider other) => TryActivate(other);

    private void TryActivate(Collider other)
    {
        if (state != State.Idle) return;
        if (other.GetComponent<PlayerHealth>() == null) return;

        StartCoroutine(ActivateSequence());
    }

    private IEnumerator ActivateSequence()
    {
        state = State.Poking;
        PlaySound(nudgeSound);
        yield return MoveSpike(0f, pokeHeight, pokeDuration);
        yield return new WaitForSeconds(pokeHoldDuration);

        state = State.Lunging;
        PlaySound(lungeSound);
        yield return MoveSpike(pokeHeight, extendedHeight, lungeDuration);
        SetHitboxActive(true);
        yield return new WaitForSeconds(extendedHoldDuration);

        state = State.Retracting;
        PlaySound(retractSound);
        SetHitboxActive(false);
        yield return MoveSpike(extendedHeight, 0f, retractDuration);

        state = State.Cooldown;
        yield return new WaitForSeconds(rearmCooldown);

        state = State.Idle;
    }

    private IEnumerator MoveSpike(float fromOffset, float toOffset, float duration)
    {
        if (spikeVisual == null) yield break;

        Vector3 start = basePos + Vector3.up * fromOffset;
        Vector3 end = basePos + Vector3.up * toOffset;

        if (duration <= 0f)
        {
            spikeVisual.localPosition = end;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            spikeVisual.localPosition = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        spikeVisual.localPosition = end;
    }

    private void SetHitboxActive(bool active)
    {
        if (spikeHitbox == null) return;

        spikeHitbox.enabled = active;

        Collider hitboxCollider = spikeHitbox.GetComponent<Collider>();
        if (hitboxCollider != null)
            hitboxCollider.enabled = active;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}