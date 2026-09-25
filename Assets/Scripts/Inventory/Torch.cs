using UnityEngine;

public class Torch : MonoBehaviour
{
    Light lightSource;
    [SerializeField] bool snuffTorch = false;
    [SerializeField] Transform lavaPosition;

    Vector3 lightPos;
    Vector3 lavaPos;

    void Start()
    {
        lavaPos = lavaPosition.position;
        lightSource = GetComponentInChildren<Light>();
        lightPos = GetComponentInChildren<Vector3>();
    }

    void Update()
    {
        if (snuffTorch) SnuffTorch();
    }

    void SnuffTorch()
    {
        lightSource.enabled = false;
    }

    void SnuffAllTorches()
    {

    }

}
