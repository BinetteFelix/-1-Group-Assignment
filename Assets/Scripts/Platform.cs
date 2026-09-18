using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private bool isTrigger = false;
    [SerializeField] private bool isPausing = true;
    [SerializeField] private bool isPingPong = true;
    private bool isTriggr = false;
    private bool isWaiting = false;

    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float moveDuration = 5f;

    [Header("Start and End Position References")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform endPosition;

    private float elapsedTime = 0f;
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        startPos = platformPrefab.transform.position;
        endPos = endPosition.position;
        
    }

    void Update()
    {
        if (isTrigger && !isTriggr) return;
        if (isWaiting) return;

        if (elapsedTime >= moveDuration && isPausing && isPingPong && isTrigger)
        {
            elapsedTime = 0f;
            ReturnTrip();
            isWaiting = true;
            isTriggr = false;
        }
        else if (elapsedTime >= moveDuration && isPausing && isPingPong)
        {
            elapsedTime = 0f;
            ReturnTrip();
            isWaiting = true;
        }
        else if (elapsedTime >= moveDuration && isPingPong && isTrigger)
        {
            elapsedTime = 0f;
            ReturnTrip();
            isTriggr = false;
        }

        else if (elapsedTime >= moveDuration && isPingPong)
        {
            elapsedTime = 0f;
            ReturnTrip();
        }

        else if (elapsedTime >= moveDuration)
        {
            elapsedTime = 0f;
            isTriggr = false;
        }

        else if (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
        }

        CalculatePlatformPosition();

    }

   
    void ReturnTrip()
    {
            Vector3 temp = startPos;
            startPos = endPos;
            endPos = temp;
    }

    void CalculatePlatformPosition()
    {
        float linearT = (elapsedTime / moveDuration);
        float angleInRadians = (linearT * Mathf.PI) - (Mathf.PI * 0.5f);
        float smoothT = (Mathf.Sin(angleInRadians) * 0.5f) + 0.5f;

        if ((isPingPong && !isTrigger) || isTriggr)
        {
            transform.position = Vector3.Lerp(startPos, endPos, smoothT); 
        }

        if (isWaiting && isPausing)
        {
            StartCoroutine(DelayedFunction());
        }
    }

    IEnumerator DelayedFunction()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isTrigger && other.CompareTag("Player"))
            if (other.CompareTag("Player"))
            {
                isTriggr = true;
            }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPos, endPos);
    }

}
