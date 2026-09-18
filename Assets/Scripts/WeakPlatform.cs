using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class WeakPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float breakDelay = 1f;
    public float respawnDelay = 3f;

    [Header("Warning Shake")]
    public bool shakeBeforeBreak = true;
    public float shakeIntensity = 0.03f;

    private Collider col;
    private Renderer[] renderers;
    private Vector3 startLocalPos;

    private enum State { Idle, Breaking, Broken }
    private State state = State.Idle;

    void Awake()
    {
        col = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
        startLocalPos = transform.localPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Idle) return;

        if (collision.gameObject.tag == "Player")
            StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        state = State.Breaking;

        if (shakeBeforeBreak)
        {
            float t = 0f;
            while (t < breakDelay)
            {
                t += Time.deltaTime;
                Vector3 shakeOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * shakeIntensity;
                transform.localPosition = startLocalPos + shakeOffset;
                yield return null;
            }
            transform.localPosition = startLocalPos;
        }
        else
        {
            yield return new WaitForSeconds(breakDelay);
        }

        state = State.Broken;
        SetVisible(false);
        col.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    private void Respawn()
    {
        transform.localPosition = startLocalPos;
        SetVisible(true);
        col.enabled = true;
        state = State.Idle;
    }

    private void SetVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;
    }
}