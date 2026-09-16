using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] List<Collectible> collectibles = new List<Collectible>();

    [SerializeField] private Transform TossWP;
    [SerializeField] private Transform ChestWP;
    private Vector3 tossPos;
    private Vector3 chestPos;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tossPos = TossWP.position;
        chestPos = ChestWP.position;
    }

    public void AddCollectible(Collectible item)
    {
        collectibles.Add(item);
        Debug.Log("Collectible added to inventory: " + item.name);
    }

    private void Update()
    {
      
    }

    public async Task UnloadCollectibles()
    {
        Time.timeScale = 0f;
        foreach (var collectible in collectibles)
        {
            Debug.Log("Unloading collectible: " + collectible.name);
            Vector3 playerPos = Player.Instance.transform.position;
            Rigidbody rb = collectible.GetComponent<Rigidbody>();
            //rb.transform.position = playerPos;
            // Debug.Log("Collectible position set to player position: " + rb.transform.position);
            //rb.useGravity = true;
            //collectible.gameObject.SetActive(true);
            //rb.AddForce(0,50,0, ForceMode.Impulse);

            CoinUI.Instance.AddCoin(collectible.value);
            await Task.Delay(100);
        }
        collectibles.Clear();
    }

}
