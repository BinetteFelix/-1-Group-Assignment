using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private float platformSpeed = 0.5f;
    [SerializeField] private Vector3 platformDirection = Vector3.right;
    [SerializeField] private bool isMoving = true;
    [SerializeField] private bool isLooping = true;
    [SerializeField] private bool isPingPong = true;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform endPosition;
    private Vector3 endPos;

    void Start()
    {
        endPos = endPosition.position;
        startPosition = platformPrefab.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPingPong && isLooping)
        {
            platformPrefab.transform.position = Vector3.Lerp(startPosition, endPos, Mathf.PingPong(Time.time * platformSpeed, 1f));
        }

        
    }
}
