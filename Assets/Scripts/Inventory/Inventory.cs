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

    [SerializeField] private Transform TossWP;
    [SerializeField] private Transform ChestWP;
    [SerializeField] private Vector3 offset = new Vector3(0, 20f, 0f);

    [SerializeField] float tossDelay = 0.3f;
    [SerializeField] AnimationCurve curve;
    float explosionXAngle = 60f;
    int initialCount;
    //private Vector3 tossPos;
    private Vector3 chestPos;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //tossPos = TossWP.position;
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

    public void TriggerUnload()
    {
        if (collectibles.Count > 0)
        {
            StartCoroutine(UnloadCollectiblesRoutine());
        }
        
    }

    private IEnumerator UnloadCollectiblesRoutine()
    {
        initialCount = collectibles.Count;

        for (int i = collectibles.Count - 1; i >= 0; i--)
        {
            Collectible item = collectibles[i];
            float explosionYAngle = 360f / initialCount * i;
            Quaternion target = Quaternion.Euler(explosionXAngle, explosionYAngle, 0);
            item.transform.position = Player.Instance.transform.position + offset;
            item.gameObject.SetActive(true);

            if (item.TryGetComponent<Collider>(out var collider))
            {
                collider.isTrigger = false;
            }

            if (item.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                Vector3 throwDirection = (target * item.transform.position * Time.deltaTime);
                item.transform.position = Vector3.Lerp(Vector3.Lerp(item.transform.position, throwDirection, 0.5f), Vector3.Lerp(item.transform.position, chestPos, 0.8f), 0.3f);
                //rb.AddForce(throwDirection * UnityEngine.Random.Range(5f, 8f), ForceMode.Impulse);
            }
                CoinUI.Instance.AddCoin(item.value);

            collectibles.RemoveAt(i);

            yield return new WaitForSeconds(tossDelay);
        }
    }
}
