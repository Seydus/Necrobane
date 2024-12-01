using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.UI;
using UnityEngine.UI;
public class ChangeSceene : MonoBehaviour
{
    public Button[] buttons;
    public GameObject pausePanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool open = true;

            if (Time.timeScale == 1 && open) 
            {
                pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0; 
                open = false;
            }

            if (Time.timeScale == 0 && open)
            {
                pausePanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                open = false;
            }
        }
    }

    public void ChangeScene(string SceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneName);
    }

    public void Resume()
    {
        bool open = true;

        if (Time.timeScale == 1 && open)
        {
            pausePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            open = false;
        }

        if (Time.timeScale == 0 && open)
        {
            pausePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            open = false;
        }
    }

    public void OpenOptions(GameObject options)
    {
        bool open = true;

        if(options.active == true && open)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = true;
            }
            options.SetActive(false);
            open = false;
        }

        if (options.active == false && open)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = false;
            }
            options.SetActive(true);
            open = false;
        }
    }

    public void Leave()
    {
        Application.Quit();
    }
}
