using System;
using UnityEngine;

public class Torch : MonoBehaviour
{
    Light lightSource;
    [SerializeField] bool snuffTorch = false;
    [SerializeField] Audio
    LayerMask layerMask;

    void Start()
    {
        layerMask = LayerMask.GetMask("Lava");
        lightSource = GetComponentInChildren<Light>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,0.1f, layerMask))
        {
            SnuffTorch();
        }
    }

    void SnuffTorch()
    {
        lightSource.enabled = false;
    }

    void SnuffAllTorches()
    {

    }

}
