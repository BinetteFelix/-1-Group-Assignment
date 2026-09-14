using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    List<Collectible> collectibles = new List<Collectible>();
    
    public void AddCollectible(Collectible item)
    {
        collectibles.Add(item);
    }

    public void UnloadCollectibles(Collectible item)
    {
        foreach (var collectible in collectibles)
        {
            
                collectible.gameObject.SetActive(false);
              
        }
        collectibles.Remove(item);
    }

}
