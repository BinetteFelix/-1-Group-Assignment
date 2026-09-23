using System;
using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] public int value;
    public int Value => value;
    [SerializeField] private GameObject prefab;
    [SerializeField] Collider col;
    public static event Action questItemTrap;
    public static event Action<bool> OnCollectiblePickedUp;

    public void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }

        else if (other.CompareTag("Player"))
        {
            if(prefab.name.Contains("HeartGem"))
            {
                OnCollectiblePickedUp?.Invoke(true);
                HeartsUI.Instance.AddHeart(Value);
                Destroy(prefab);
            }
            else if(prefab.name.Contains("Quest"))
            {
                questItemTrap?.Invoke();
                OnCollectiblePickedUp?.Invoke(false);
                prefab.SetActive(false);
                Inventory.Instance.AddCollectible(this);
                col.isTrigger = false;
            }
            else
            {
                OnCollectiblePickedUp?.Invoke(false);
                prefab.SetActive(false);
                Inventory.Instance.AddCollectible(this);
                col.isTrigger = false;
            }
            
        }
    }
}
