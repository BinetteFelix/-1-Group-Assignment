using System;
using UnityEngine;

public class Torch : MonoBehaviour
{
    Light lightSource;
    [SerializeField] private AudioSource torchOffAudioSource;
    public AudioClip torchSoundOff;
    bool hasSnuffed = false;
    LayerMask layerMask;

    void Start()
    {
        layerMask = LayerMask.GetMask("Lava");
        lightSource = GetComponentInChildren<Light>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit,0.5f, layerMask))
        {
            Debug.Log("Raycast");
            SnuffTorch();
        }
    }

    void SnuffTorch()
    {
        lightSource.enabled = false;

        if (!hasSnuffed)
        {
           torchOffAudioSource.PlayOneShot(torchSoundOff);
           hasSnuffed = true;
        }
        
    }

    void SnuffAllTorches()
    {

    }

}
