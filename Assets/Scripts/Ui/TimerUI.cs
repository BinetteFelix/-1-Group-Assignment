using UnityEngine;
using TMPro;
using System;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float time = 120f; //TODO: Need to confirme

    //TODO: Need warning changing color/size/animation?

    public event Action OnTimeUp;
    private void Update()
    {
        time -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);
        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";

        if (time <= 0)
        {
            TriggerTimeUp();
        }

    }

    private void TriggerTimeUp()
    {
        OnTimeUp?.Invoke();
        GameOver.Instance.GameOverSceen();
    }

}
