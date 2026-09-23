using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;


public class UIManager : SingletonBehaviour<UIManager>
{
    #region UI PANELS
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject InGameUIPanel;
    [SerializeField] private GameObject GameOverPanel;
    #endregion

    #region INPUT ACTIONS
    [SerializeField] private InputAction pauseAction;
    #endregion

    #region FAIL VARIABLES
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime;
    private float timeTillFail; //TODO: Need to confirm
    //TODO: Need warning changing color/size/animation?

    public event Action OnTimeUp;
    #endregion

    public bool IsPaused {  get; private set; }
    private void Start()
    {
        timeTillFail = startTime;
        pauseAction.Disable();
        InGameUIPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
            Pause();

        #region FAIL TIMER
        timeTillFail -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timeTillFail / 60f);
        int seconds = Mathf.FloorToInt(timeTillFail % 60f);
        int milliseconds = Mathf.FloorToInt((timeTillFail * 100f) % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";

        if (timeTillFail <= 0)
        {
            TriggerTimeUp();
        }
        #endregion
    }

    #region GAME STATE
    public void Pause()
    {
        IsPaused = !IsPaused;

        switch (IsPaused)
        {
            case true:
                {
                    Cursor.lockState = CursorLockMode.None;
                    pausePanel.SetActive(true);
                    Time.timeScale = 0;
                    break;
                }
            case false:
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    pausePanel.SetActive(false);
                    Time.timeScale = 1;
                    break;
                }
        }
    }
    public void RestartButton()
    {
        ResetTimer();

        GameOverPanel.SetActive(false);
        SceneManager.LoadScene(1);
        Pause();
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);

        mainMenuPanel.SetActive(true);
        InGameUIPanel.SetActive(false);

        Pause();
        pauseAction.Disable();
        Cursor.lockState = CursorLockMode.None;
    }
    public void StartGame()
    {
        ResetTimer();
        SceneManager.LoadScene(1);

        mainMenuPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        InGameUIPanel.SetActive(true);

        pauseAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void GameOverSceen()
    {
        GameOverPanel.SetActive(true);
        Pause();
    }
    #endregion

    #region FAIL METHODS
    private void TriggerTimeUp()
    {
        OnTimeUp?.Invoke();
        GameOverSceen();
        Cursor.lockState = CursorLockMode.None;
    }
    public void ResetTimer()
    {
        timeTillFail = startTime;
    }
    #endregion

    #region SINGLETON HANDLER
    public override void Instantiate()
    {
    }
    #endregion
}
