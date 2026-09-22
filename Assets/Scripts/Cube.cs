using System.Collections;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [Header("Anchors")]
    [SerializeField] private Transform peakAnchor;
    [SerializeField] private Transform landingAnchor;

    [Header("Settings")]
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool tossOnStart = true;

    private void Start()
    {
        if (tossOnStart)
        {
            Toss(gameObject);
        }
    }

    public void Toss(GameObject item)
    {
        StartCoroutine(AnimateToss(item));
    }

    private IEnumerator AnimateToss(GameObject item)
    {
        Vector3 startPos = transform.position;
        Vector3 peakPos = peakAnchor.position;
        Vector3 landPos = landingAnchor.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float linearT = Mathf.Clamp01(elapsedTime / duration);
            float t = speedCurve.Evaluate(linearT);

            // Double Lerp creates the exact high-arc path using your anchors
            Vector3 m0 = Vector3.Lerp(startPos, peakPos, t);
            Vector3 m1 = Vector3.Lerp(peakPos, landPos, t);
            transform.position = Vector3.Lerp(m0, m1, t);

            yield return null;
        }

        //transform.position = landPos;
    }
}