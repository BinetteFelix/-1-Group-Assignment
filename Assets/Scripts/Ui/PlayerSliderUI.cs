using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSliderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI height;
    [SerializeField] private Slider playerSlider;

    void Update()
    {
        UpdateText();
        UpdateSlider();
    }

    private void UpdateText()
    {
        float meters = TowerTracker.Instance.GetCurrentMeters();
        height.text = $"{meters:F0}m";
    }

    private void UpdateSlider()
    {
        float normalized = TowerTracker.Instance.GetPlayerNormalizedHeight();
        playerSlider.value = normalized;
    }
}
