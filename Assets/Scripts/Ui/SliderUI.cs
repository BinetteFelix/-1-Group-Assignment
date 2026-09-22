using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderUI : MonoBehaviour
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
        float meters = PlayerTracker.Instance.GetCurrentMeters();
        height.text = $"{meters:F0}m";
    }

    private void UpdateSlider()
    {
        float normalized = PlayerTracker.Instance.GetNormalizedHeight();
        playerSlider.value = normalized;
    }
}
