using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance; 
    public GameObject gameOver;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitBututton;

    private void Awake()
    {
        Instance = this;
    }

    public void GameOverSceen()
    {

        gameOver.SetActive(true);

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
        retryButton.onClick.AddListener(() =>
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("");
        });
    }

}
