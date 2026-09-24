using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
            animator.SetBool("open", true);
            playerCamera.gameObject.SetActive(false);
            TimerUI.Instance.StopTimer();
            Invoke(nameof(VictoryScene), 2f);
        }
    }

    void VictoryScene()
    {
        Inventory.Instance.TriggerUnload();
    }
}
