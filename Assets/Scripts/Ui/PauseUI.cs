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


    private void Update()
    {
        if (Keyboard.current[Key.Escape].wasPressedThisFrame) 
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;

        }

        ResumeButton();


    }



    public void ResumeButton()
    {
        Debug.Log("1");
        resumeButton.onClick.AddListener(() =>
        {
            Debug.Log("2");
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        });
    }

    public void RestartButton()
    {

    }

    public void MainMenuButton()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("");
    }

}
