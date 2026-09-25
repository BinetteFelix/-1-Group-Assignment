using UnityEngine;
using Utility;

public class CollectiblesAudio : SingletonBehaviour<CollectiblesAudio>
{

    public AudioSource worldInteractiveAudioSource;
    public AudioClip[] heartPickupSound;
    public AudioClip[] valueablePickupSound;
    private int lastHeartIndex = -1;
    private int lastValueableIndex = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void CollectiblePickupHandler(bool heartPickupCheck)
    {
        
        
        if (heartPickupCheck)
        {
            int randomHeartIndex = Random.Range(0, heartPickupSound.Length);
            while (randomHeartIndex == lastHeartIndex)
            {
                randomHeartIndex = Random.Range(0, heartPickupSound.Length); 
            }
            worldInteractiveAudioSource.PlayOneShot(heartPickupSound[randomHeartIndex]);
            lastHeartIndex = randomHeartIndex;
        }
        else
        {
            int randomValueableIndex = Random.Range(0, valueablePickupSound.Length);
            while (randomValueableIndex == lastValueableIndex)
            {
                randomValueableIndex = Random.Range(0, valueablePickupSound.Length);
            }
            worldInteractiveAudioSource.PlayOneShot(valueablePickupSound[randomValueableIndex], 1.5f);
            lastValueableIndex = randomValueableIndex;
        }
    }
    void OnEnable()
    {
        Collectible.OnCollectiblePickedUp += CollectiblePickupHandler;
    }
    void OnDisable()
    {
        Collectible.OnCollectiblePickedUp -= CollectiblePickupHandler;
    }

    public override void Instantiate()
    {
    }
}
