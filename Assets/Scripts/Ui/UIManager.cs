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
    [SerializeField] private GameObject GameOverPanel;
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
    private float timeTillFail; //TODO: Need to confirm
                                //TODO: Need warning changing color/size/animation?

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

    public bool IsPaused {  get; private set; }
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

        UpdateTrackers();
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
        GameOverPanel.SetActive(false);

        pauseAction.Disable();
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }
    public void RestartButton()
    {
        ResetTimer();
        ResetGameState();

        GameOverPanel.SetActive(false);
        InGameUIPanel.SetActive(true);
        trackersPanel.SetActive(false);

        pauseAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
    }

    public void StartGame()
    {
        ResetTimer();
        ResetGameState();

        mainMenuPanel.SetActive(false);
        GameOverPanel.SetActive(false);
        InGameUIPanel.SetActive(true);

        pauseAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
    }

    private void ResetGameState()
    {
        IsPaused = false;
        IsGameOver = false;
        Time.timeScale = 1;

        pausePanel.SetActive(false);
        youDiedPanel.SetActive(false);
        youWinPanel.SetActive(false);
    }

    private void GameOverSceen(bool won)
    {
        IsGameOver = true;
        isTimerRunning = false;

        GameOverPanel.SetActive(true);
        youDiedPanel.SetActive(!won);
        youWinPanel.SetActive(won);
    }
    #endregion

    #region FAIL METHODS
    private void TriggerTimeUp()
    {
        OnTimeUp?.Invoke();
        Fail();
    }
    public void Fail()
    {
        if (IsGameOver) return;
        GameOverSceen(false);
    }
    public void Success()
    {
        if (IsGameOver) return;
        GameOverSceen(true);
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
    public void CreditsButton()
    {
        creditsPanel.SetActive(true);
    }
    public void CloseCreditsButton()
    {
        creditsPanel.SetActive(false);
    }
    public void ExitButton()
    {
        //Need to look up
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
