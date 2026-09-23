using UnityEngine;
using UnityEngine.UI;

public class LavaSliderUI : MonoBehaviour
{
    [SerializeField] private Slider lavaSlider;

    void Update()
    {
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        float normalized = TowerTracker.Instance.GetLavaNormalizedHeight();
        lavaSlider.value = normalized;
    }
}
