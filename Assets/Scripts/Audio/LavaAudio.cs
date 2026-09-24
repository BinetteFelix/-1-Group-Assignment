using UnityEngine;

public class LavaAudio : MonoBehaviour
{

    [SerializeField] AudioSource lavaSoundAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartLavaSoundHandler(bool lavaSoundStart)
    {
        if (lavaSoundStart)
        {
            lavaSoundAudioSource.Play();
        }
            
    }
    void OnEnable()
    {
        LavaMechanic.lavaSound += StartLavaSoundHandler;
    }
    void OnDisable()
    {
        LavaMechanic.lavaSound -= StartLavaSoundHandler;
    }
}
