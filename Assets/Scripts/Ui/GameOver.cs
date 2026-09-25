using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public GameObject youDied;
    public GameObject youWin;

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {


    }

    public void Fail()
    {

        youDied.SetActive(true);

    }

    public void Success()
    {

        TimerUI.Instance.StopTimer();
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

}
