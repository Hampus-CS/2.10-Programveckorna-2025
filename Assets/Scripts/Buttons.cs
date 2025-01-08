using Unity.VisualScripting;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject defaultView;
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
        defaultView.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        defaultView.SetActive(true);
        pauseMenu.SetActive(false);
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
