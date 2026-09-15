using UnityEngine;

public class Container : MonoBehaviour
{
    public static Container Instance;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        animator.SetBool("open", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered container trigger");
            animator.SetBool("open", true);
            Inventory.Instance.UnloadCollectibles();
        }
    }
}
