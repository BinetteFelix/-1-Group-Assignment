using UnityEngine;

public class HeartController : MonoBehaviour
{
    private Camera heartCamera;
    private GameObject uiRawImageObj;

    public void Init(Camera cam, GameObject uiObj)
    {
        heartCamera = cam;
        uiRawImageObj = uiObj;
    }

    public void Hide()
    {
        heartCamera.enabled = false;
        uiRawImageObj.SetActive(false);
    }

    public void Show()
    {
        heartCamera.enabled = true;
        uiRawImageObj.SetActive(true);
    }
}
