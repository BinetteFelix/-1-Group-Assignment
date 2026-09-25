using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;
using UnityEngine.UI;


public class UIManager : SingletonBehaviour<UIManager>
{
    #region UI PANELS
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject InGameUIPanel;
    [SerializeField] private GameObject youDiedPanel;
    [SerializeField] private GameObject youWinPanel;

    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    #endregion

    #region INPUT ACTIONS
    [SerializeField] private InputAction pauseAction;
    #endregion

    #region FAIL VARIABLES
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime;
    private float timeTillFail;
    private bool isTimerRunning;
    public event Action OnTimeUp;
    #endregion

    #region TOWER TRACKER
    [SerializeField] private GameObject trackersPanel;
    [SerializeField] private Slider playerSlider;
    [SerializeField] private Slider lavaSlider;
    [SerializeField] private TextMeshProUGUI heightText;
    #endregion

    #region SCORE
    [SerializeField] private TextMeshProUGUI scoreText;
    #endregion

    #region REFERENCES
    [SerializeField] HeartsUI heartsUI;
    [SerializeField] CoinUI coinUI;
    #endregion
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    private void Start()
    {
        timeTillFail = startTime;
        pauseAction.Disable();
        InGameUIPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            if (optionsPanel.activeSelf)
                CloseOptionsButton();
            else if (!IsGameOver)
                Pause();
        }

        #region FAIL TIMER

        if(isTimerRunning && !IsGameOver)
        {
            timeTillFail -= Time.deltaTime;
            if (timeTillFail <= 0)
            {
                timeTillFail = 0;
                TriggerTimeUp();
            }
            UpdateTimerText();
        }

        Debug.Log(coinUI.GetCoinCount());
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
    public void ResumeButton()
    {
        if (IsPaused) Pause();
    }
    public void MainMenuButton()
    {
        ResetGameState();
        isTimerRunning = false;

        mainMenuPanel.SetActive(true);
        InGameUIPanel.SetActive(false);

        pauseAction.Disable();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
    public void RestartButton()
    {
        ResetTimer();
        ResetGameState();

        youDiedPanel.SetActive(false);
        youWinPanel.SetActive(false);
        InGameUIPanel.SetActive(true);

        pauseAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
    }

    public void StartGame()
    {
        ResetTimer();
        ResetGameState();

        mainMenuPanel.SetActive(false);
        youDiedPanel.SetActive(false);
        youWinPanel.SetActive(false);
        InGameUIPanel.SetActive(true);

        pauseAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
        TowerTracker.Instance.showTracker();
    }

    private void ResetGameState()
    {
        IsPaused = false;
        IsGameOver = false;
        Time.timeScale = 1;

        pausePanel.SetActive(false);
        youDiedPanel.SetActive(false);
        youWinPanel.SetActive(false);

        heartsUI.ResetHeart();
        coinUI.ResetCoins();
    }

    #endregion

    #region FAIL METHODS
    private void TriggerTimeUp()
    {
        isTimerRunning = false;
        OnTimeUp?.Invoke();
        Fail();
        Cursor.lockState = CursorLockMode.None;

    }
    public void Fail()
    {
        if (IsGameOver) return;
        IsGameOver = true; 
        youDiedPanel.SetActive(true);
        Pause();
    }
    public void Success()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        youWinPanel.SetActive(true);
        UpdateScoreDisplay();
    }
    public void ResetTimer()
    {
        timeTillFail = startTime;
        isTimerRunning = true;
        UpdateTimerText();
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeTillFail / 60f);
        int seconds = Mathf.FloorToInt(timeTillFail % 60f);
        int milliseconds = Mathf.FloorToInt((timeTillFail * 100f) % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
    #endregion

    #region MENU BUTTONS
    public void OptionsButton()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsButton()
    {
        optionsPanel.SetActive(false);
    }

    public void WindowsButton()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region SCORE
    private void UpdateScoreDisplay()
    {
        if (scoreText == null) return;

        int finalScore = CoinUI.Instance != null ? CoinUI.Instance.GetCoinCount() : 0;
        scoreText.text = $"SCORE: {finalScore}";
    }
    #endregion

    #region TRACKER METHODS
    public void ShowTracker()
    {
        trackersPanel.SetActive(true);
    }
    private void UpdateTrackers()
    {
        if (TowerTracker.Instance == null || !trackersPanel.activeSelf) return;

        playerSlider.value = TowerTracker.Instance.GetPlayerNormalizedHeight();
        lavaSlider.value = TowerTracker.Instance.GetLavaNormalizedHeight();

        float meters = TowerTracker.Instance.GetCurrentMeters();
        heightText.text = $"{meters:F0}m";
    }
    #endregion

    #region SINGLETON HANDLER
    public override void Instantiate()
    {
    }
    #endregion
}