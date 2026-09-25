using Unity.Cinemachine;
using UnityEngine;

public class Container : MonoBehaviour
{
    public static Container Instance;
    Animator animator;
    [SerializeField] CinemachineCamera playerCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
        animator.SetBool("open", false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        else if (other.CompareTag("Player") && Inventory.Instance.collectibles.Count > 0)
        {
            animator.SetBool("open", true);
            Invoke(nameof(VictoryScene), 2f);
        }
    }

    void VictoryScene()
    {
        Inventory.Instance.TriggerUnload();
    }
}
