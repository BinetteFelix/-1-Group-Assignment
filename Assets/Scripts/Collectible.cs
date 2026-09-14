using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] private int value;
    [SerializeField] private GameObject prefab;
    //[SerializeField] private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefab = prefab.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnTriggerEnter(Collider other)
    {
        // Handle trigger enter logic
        if(other.CompareTag("Player"))
        {
            // Add value to player's score or inventory
            //player = other.GetComponent<player>();
            //if (player != null)
            //{
            //    player.AddScore(value);
            //}
            // Destroy the collectible object
            prefab.SetActive(false);
        }
    }
}
