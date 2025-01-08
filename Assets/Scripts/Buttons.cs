using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Buttons : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject defaultView;
    public TMPro.TextMeshProUGUI time;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf == false)
            {
                pause();
            }
            else if (pauseMenu.activeSelf == true)
            {
                Resume();
            }

        }
    }

    public void pause()
    {
        Time.timeScale = 0;
        print("Timescale 0");

        defaultView.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        defaultView.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        print("Timescale 1");
    }

    public void Quit()
    {
        Application.Quit();
        print("Quit");
    }

    public void Save()
    {
        print("Save");
    }
}
