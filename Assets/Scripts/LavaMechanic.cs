using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LavaMechanic : MonoBehaviour
{ 
    [SerializeField] Transform lava;
    [SerializeField] Vector3 lavaEndPos;
    [SerializeField] Vector3 lavaStartPos;
    [SerializeField] float lavaTravelDuration = 30;

    public static event Action<bool> lavaSound;
    bool isLavaTravelling;
    
    float elapsedTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lavaStartPos = lava.transform.position;
        TowerTracker.Instance.SetLavaTransform(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLavaTravelling) return;
        elapsedTime += Time.deltaTime;
        lava.transform.position = Vector3.Lerp(lavaStartPos, lavaEndPos, elapsedTime / lavaTravelDuration);
    }

    private void OnEnable()
    {
        Collectible.questItemTrap += LavaTrapEngaged;
    }

    private void OnDisable()
    {
        Collectible.questItemTrap -= LavaTrapEngaged;
    }

    void LavaTrapEngaged()
    {
        isLavaTravelling = true;
        lavaSound?.Invoke(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            HeartsUI.Instance.RemoveHeart(3);
        }
    }

}
