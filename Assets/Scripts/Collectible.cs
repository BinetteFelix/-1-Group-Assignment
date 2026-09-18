using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] public int value;
    public int Value => value;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        // Handle trigger enter logic
        else if (other.CompareTag("Player"))
        {
            if(prefab.name.Contains("HeartGem"))
            {
                HeartsUI.Instance.AddHeart(value);
                Destroy(prefab);
            }
            else
            {
                prefab.SetActive(false);
                Inventory.Instance.AddCollectible(this);
            }
            
        }
    }
}
