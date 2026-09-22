using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] List<Collectible> collectibles = new List<Collectible>();

    [SerializeField] private Vector3 offset = new Vector3(0, 20f, 0f);

    [SerializeField] float tossDelay = 0.3f;
    
    private void Awake()
    {
        Instance = this;
    }

    public void AddCollectible(Collectible item)
    {
        collectibles.Add(item);
        Debug.Log("Collectible added to inventory: " + item.name);
    }
    public void TriggerUnload()
    {
        if (collectibles.Count > 0)
        {
            StartCoroutine(UnloadCollectiblesRoutine());
        }
        
    }

    private IEnumerator UnloadCollectiblesRoutine()
    {
        for (int i = collectibles.Count - 1; i >= 0; i--)
        {
            Collectible item = collectibles[i];
            item.transform.position = Player.Instance.transform.position + offset;
            item.gameObject.SetActive(true);

            CoinUI.Instance.AddCoin(item.value);

            collectibles.RemoveAt(i);

            yield return new WaitForSeconds(tossDelay);
        }
    }
}
