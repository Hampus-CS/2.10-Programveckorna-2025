using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeatherSystem : MonoBehaviour
{
    private List<string> weatherConditions = new List<string> { "Sunny", "Rainy", "Snowing", "Foggy" };

    public TMP_Text currentWeatherText;
    public TMP_Text forecastText;

    public GameObject rainEffect;  
    public GameObject snowEffect;   
    public GameObject fogEffect;   
    public Light sunLight;          

    private string currentWeather;

    void Start()
    {
        ChangeWeather();
        InvokeRepeating("ChangeWeather", 120f, 120f); //2min
    }

    void ChangeWeather()
    {
        string[] weatherConditions = { "Sunny", "Rainy", "Snowing", "Foggy" };
        currentWeather = weatherConditions[Random.Range(0, weatherConditions.Length)];
        UpdateWeatherEffects();
    }

    void UpdateWeatherEffects()
    {
        rainEffect.SetActive(false);
        snowEffect.SetActive(false);
        fogEffect.SetActive(false);
        sunLight.enabled = false;

        switch (currentWeather)
        {
            case "Sunny":
                sunLight.enabled = true;
                break;
            case "Rainy":
                rainEffect.SetActive(true);
                break;
            case "Snowing":
                snowEffect.SetActive(true);
                break;
            case "Foggy":
                fogEffect.SetActive(true);
                break;
        }

        currentWeatherText.text = $"Current Weather: {currentWeather}";
    }
}