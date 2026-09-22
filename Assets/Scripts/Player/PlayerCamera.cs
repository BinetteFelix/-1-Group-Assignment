using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject camHolder;
    [SerializeField] private CinemachineCamera cam;
    public void DoFov(float endValue)
    {
        StopCoroutine(FovChange(endValue));
        StartCoroutine(FovChange(endValue));
    }
    private IEnumerator FovChange(float endValue)
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
    public void DoTilt(float zTilt)
    {
        StopCoroutine(TiltChange(zTilt));
        StartCoroutine(TiltChange(zTilt));
    }
    private IEnumerator TiltChange(float zTilt)
    {
        float time = 0.0f;
        float timer = 0.3f;

        while (time < timer)
        {
            cam.Lens.Dutch = Mathf.Lerp(cam.Lens.Dutch, zTilt, 0.25f);
            time += Time.deltaTime;

            yield return null;
        }
    }
}
