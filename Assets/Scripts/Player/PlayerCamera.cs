using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject camHolder;
    [SerializeField] private CinemachineCamera cam;
    public void DoFov(float endValue, float lerpTime)
    {
        StopCoroutine(FovChange(endValue, lerpTime));
        StartCoroutine(FovChange(endValue, lerpTime));
    }
    private IEnumerator FovChange(float endValue, float lerpTime)
    {
        float time = 0.0f;
        float timer = 0.3f;

        while (time < timer)
        {
            cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, endValue, 0.25f);
            time += Time.deltaTime;

            yield return null;
        }
    }
    public void DoTilt(float zTilt, float lerpTime)
    {
        StopCoroutine(TiltChange(zTilt, lerpTime));
        StartCoroutine(TiltChange(zTilt, lerpTime));
    }
    private IEnumerator TiltChange(float zTilt, float lerpTime)
    {
        float time = 0.0f;
        float timer = 0.3f;

        while (time < timer)
        {
            cam.Lens.Dutch = Mathf.Lerp(cam.Lens.Dutch, zTilt, lerpTime);
            time += Time.deltaTime;

            yield return null;
        }
    }
}
