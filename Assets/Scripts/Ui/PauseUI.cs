using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PauseUI : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainmenuButton;


    //public void Awake()
    //{
    //    resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();

    //}

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeButton);
        mainmenuButton.onClick.AddListener(MainMenuButton);
    }

    private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame) 
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;

        }

    }



    public void ResumeButton()
    {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
    }

    public void RestartButton()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("");
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
