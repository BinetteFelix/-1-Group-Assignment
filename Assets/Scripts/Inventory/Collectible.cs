using System;
using Unity.Cinemachine;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    [SerializeField] public int value;
    public int Value => value;
    [SerializeField] private GameObject prefab;
    [SerializeField] Collider col;
    [SerializeField] float impulseForce;
    CinemachineImpulseSource impulseSource;
    public static event Action questItemTrap;
    public static event Action<bool> OnCollectiblePickedUp;

    void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

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
                Invoke(nameof(QuestTrapActivated), 1f);
                OnCollectiblePickedUp?.Invoke(false);
                prefab.SetActive(false);
                Inventory.Instance.AddCollectible(this);
                col.isTrigger = false;
                impulseSource.GenerateImpulseWithForce(impulseForce);
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

    void QuestTrapActivated()
    {
        questItemTrap?.Invoke();
    }
}
