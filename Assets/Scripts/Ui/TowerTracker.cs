using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerTracker : MonoBehaviour
{
    public static TowerTracker Instance;
    public GameObject trackers;

    [Header("Player")]
    public Transform player;
    public float p_startHeight; //TODO: Need the y-coordinate
    public float p_endHeight;


    [Header("Lava")]
    public Transform lava;
    public float l_startHeight;
    public float l_endHeight;


    public float p_normalizedHeight;
    public float l_normalizedHeight;

    [SerializeField] Slider LavaSlider;
    [SerializeField] Slider PlayerSlider;

    [SerializeField] TextMeshProUGUI heightText;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        UpdatePlayerHeight();
        UpdateLavaHeight();

        SetPlayerSliderValue();
        SetLavaSliderValue();

        SetCurrentHeight();
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
    private void SetCurrentHeight()
    {
        heightText.text = Mathf.RoundToInt(GetCurrentMeters()) + " M";
    }
    public void showTracker()
    {
        trackers.SetActive(true);
    }
    private void SetLavaSliderValue()
    {
        LavaSlider.value = GetLavaNormalizedHeight();
    }
    private void SetPlayerSliderValue()
    {
        PlayerSlider.value = GetPlayerNormalizedHeight();
    }

    public float GetPlayerNormalizedHeight() => p_normalizedHeight;

    public float GetLavaNormalizedHeight() => l_normalizedHeight;
    public float GetCurrentMeters() => player.position.y - p_endHeight;

    public void SetPlayerTransform(Transform transform)
    {
        player = transform;
    }
    public void SetLavaTransform(Transform transform)
    {
        lava = transform;
    }
}
