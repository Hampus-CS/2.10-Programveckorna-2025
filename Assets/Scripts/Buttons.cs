using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEditor.Rendering;
using System.Collections.Generic;

public class Buttons : MonoBehaviour
{
    public List<GameObject> menus = new();

    private void Start()
    {
        Time.timeScale = 0;

        menus[2].SetActive(true);
        menus[0].SetActive(false);
        menus[1].SetActive(false);
        menus[3].SetActive(false);

        /*
         0 = def
         1 = pause
         2 = start
         3 = settings
        */
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menus[1].activeSelf == false)
            {
                Pause();
            }
            else if (menus[1].activeSelf == true)
            {
                Resume();
            }
        }
    }

    public void Play()
    {
        menus[2].SetActive(false);
        menus[0].SetActive(true);

        Time.timeScale = 1;
        print("TimeScale " + Time.timeScale);
    }

    public void Settings()
    {
        menus[2].SetActive(false);
        menus[3].SetActive(true);
    }

    public void Done()
    {
        menus[3].SetActive(false);
        menus[2].SetActive(true);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        print("TimeScale " + Time.timeScale);

        menus[0].SetActive(false);
        menus[1].SetActive(true);
    }

    public void Resume()
    {
        menus[1].SetActive(false);
        menus[0].SetActive(true);

        Time.timeScale = 1;
        print("TimeScale " + Time.timeScale);
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