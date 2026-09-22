using UnityEngine;

public class VictoryAnimation : MonoBehaviour
{
    [SerializeField] private Transform TossWP;
    [SerializeField] private Transform ChestWP;
    Inventory item;
    Vector3 tossPos;
    Vector3 chestPos;
    float interpolateAmount;
    
    void Start()
    {
        tossPos = TossWP.position;
        chestPos = ChestWP.position;
    }

    // Update is called once per frame
    void Update()
    {
        interpolateAmount += Time.deltaTime / moveDuration;
        Vector3.Lerp()
    }
}
