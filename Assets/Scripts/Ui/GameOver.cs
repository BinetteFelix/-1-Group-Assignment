using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public GameObject youDied;
    public GameObject youWin;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button f_exitBututton;
    [SerializeField] private Button w_exitBututton;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RetryButton();
        ExitButton();

    }

    public void Fail()
    {

        youDied.SetActive(true);

    }

    public void Success()
    {

        youWin.SetActive(true);
        UpdateScoreDisplay();

    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null) return;

        int finalScore = CoinUI.Instance != null ? CoinUI.Instance.GetCoinCount() : 0;
        scoreText.text = $"SCORE: {finalScore}";

        //TODO: Need to consider time as well?
    }

    public void RetryButton()
    {
        retryButton.onClick.AddListener(() =>
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("");
        });
    }

    public void ExitButton()
    {
        f_exitBututton.onClick.AddListener(ExitToMenu);
        w_exitBututton.onClick.AddListener(ExitToMenu);
    }

    private void ExitToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
