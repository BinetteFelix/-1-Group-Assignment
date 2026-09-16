using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private int haulWorth = 0;
    [SerializeField] List<Collectible> collectibles = new List<Collectible>();
    private void Awake()
    {
        Instance = this;
    }
    public void AddCollectible(Collectible item)
    {
        collectibles.Add(item);
        Debug.Log("Collectible added to inventory: " + item.name);
    }

    public void UnloadCollectibles()
    {
        foreach (var collectible in collectibles)
        {
            Debug.Log("Unloading collectible: " + collectible.name);
            Vector3 playerPos = Player.Instance.transform.position;
            Vector3 containerPos = Container.Instance.transform.position;

            Rigidbody rb = collectible.GetComponent<Rigidbody>();
            rb.transform.position = playerPos;
            Debug.Log("Collectible position set to player position: " + rb.transform.position);
            //rb.useGravity = true;
            collectible.gameObject.SetActive(true);
            rb.AddForce(0,50,0, ForceMode.Impulse);
            

            // Calculate score based on the collectible's value
            haulWorth += collectible.Value;
            Debug.Log("Score: " + haulWorth);
        }
        collectibles.Clear();
    }

}
