using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PauseUI : MonoBehaviour
{
    public GameObject pausePanel;
    private Button resumeButton;


    private void Awake()
    {
        resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();

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
        resumeButton.onClick.AddListener(() =>
        {
            pausePanel.SetActive(false);
            Time.timeScale = 0;
        });
    }

    public void MainMenuButton()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("");
    }


}
