using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float time = 120f; //TODO: Need to confirme
    [SerializeField] private bool isRunning = true;

    //TODO: Need warning changing color/size/animation?

    public event Action OnTimeUp;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (!isRunning) return;

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
        GameOver.Instance.Fail();
    }

    public void StopTimer() => isRunning = false;


}
