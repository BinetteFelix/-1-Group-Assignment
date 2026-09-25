using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField] public List<Collectible> collectibles = new List<Collectible>();

    [SerializeField] private Vector3 offset = new Vector3(0, 20f, 0f);

    [SerializeField] float tossDelay = 0.3f;
    [SerializeField] private Transform TossWP;
    [SerializeField] private Transform ChestWP;
    [SerializeField] float duration;
    Vector3 tossPos;
    Vector3 chestPos;
    
    float moveDuration;

    PlayerMovement player;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tossPos = TossWP.position;
        chestPos = ChestWP.position;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    public void AddCollectible(Collectible item)
    {
        collectibles.Add(item);
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
            item.transform.position = player.transform.position + offset;
            item.gameObject.SetActive(true);

            StartCoroutine(AnimateItemToChest(item));
            collectibles.RemoveAt(i);

            yield return new WaitForSeconds(tossDelay);
        }

    }
    private IEnumerator AnimateItemToChest(Collectible item)
    {
        Vector3 startPos = item.transform.position;
        float elapsed = 0;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            item.transform.position = Vector3.Lerp(Vector3.Lerp(startPos, tossPos, elapsed / duration), Vector3.Lerp(tossPos, chestPos, elapsed / duration), elapsed / duration);
            yield return null;
        }
        CoinUI.Instance.AddCoin(item.value);
        UIManager.Instance.Success();
        item.gameObject.SetActive(false);
    }
}
