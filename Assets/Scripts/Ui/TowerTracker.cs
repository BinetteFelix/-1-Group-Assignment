using UnityEngine;

public class TowerTracker : MonoBehaviour
{
    public static TowerTracker Instance;

    [Header("Player")]
    public Transform player;
    public float p_startHeight; //TODO: Need the y-coordinate
    public float p_endHeight;

    [Header("Lava")]
    public Transform lava;
    public float l_startHeight;
    public float l_endHeight;

    [SerializeField] private float p_normalizedHeight;
    [SerializeField] private float l_normalizedHeight;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdatePlayerHeight();
        UpdateLavaHeight();
    }

    private void UpdatePlayerHeight()
    {
        float p_currentY = player.position.y;
        p_normalizedHeight = Mathf.InverseLerp(p_startHeight, p_endHeight, p_currentY);
    }

    private void UpdateLavaHeight()
    {
        float l_currentY = lava.position.y;
        l_normalizedHeight = Mathf.InverseLerp(l_startHeight, l_endHeight, l_currentY);
    }

    public float GetPlayerNormalizedHeight() => p_normalizedHeight;
    public float GetLavaNormalizedHeight() => l_normalizedHeight;
    public float GetCurrentMeters() => player.position.y - p_startHeight;
}
