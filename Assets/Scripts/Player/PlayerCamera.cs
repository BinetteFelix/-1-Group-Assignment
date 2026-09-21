using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameObject camHolder;
    [SerializeField] private CinemachineCamera cam;
    public void DoFov(float endValue)
    {
        cam.Lens.FieldOfView = Mathf.Lerp(cam.Lens.FieldOfView, endValue, 0.25f);
    }
    public void DoTilt(float zTilt)
    {
        cam.Lens.Dutch = Mathf.Lerp(cam.Lens.Dutch, zTilt, 0.25f);
    }
}
