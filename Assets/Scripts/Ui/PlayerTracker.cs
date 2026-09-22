using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static PlayerTracker Instance;
    public Transform player;
    public float startHeight; //TODO: Need the y-coordinate
    public float endHeight;

    [SerializeField] private float normalizedHeight;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        float currentY = player.position.y;
        normalizedHeight = Mathf.InverseLerp(startHeight, endHeight, currentY);
    }

    public float GetNormalizedHeight() => normalizedHeight;
    public float GetCurrentMeters() => player.position.y - startHeight;
}
