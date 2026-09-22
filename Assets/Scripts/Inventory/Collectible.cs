using Unity.VisualScripting;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] public int value;
    public int Value => value;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Rigidbody rb;
    public static event System.Action<bool> OnCollectiblePickedUp;

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
                OnCollectiblePickedUp?.Invoke(true);
                HeartsUI.Instance.AddHeart(Value);
                Destroy(prefab);
            }
            else
            {
                OnCollectiblePickedUp?.Invoke(false);
                prefab.SetActive(false);
                Inventory.Instance.AddCollectible(this);
            }
            
        }
    }
}
