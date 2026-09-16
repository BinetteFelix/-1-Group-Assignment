using UnityEngine;

public class Cube : MonoBehaviour
{
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        Vector3 playerPos = Player.Instance.transform.position;
        Vector3 containerPos = Container.Instance.transform.position;
        Debug.Log("Player Position: " + playerPos);
        Debug.Log("Container Position: " + containerPos);
        Debug.Log("Difference: " + (playerPos - containerPos));
        rb = GetComponent<Rigidbody>();
        rb.AddForce(containerPos - playerPos, ForceMode.Impulse);
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
