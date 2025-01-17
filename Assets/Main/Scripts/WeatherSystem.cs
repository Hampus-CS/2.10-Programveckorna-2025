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

    public AudioSource rainAudioSource; 
    public AudioSource windAudioSource;  

    private string currentWeather;

    void Start()
    {
        ChangeWeather();
        InvokeRepeating("ChangeWeather", 120f, 120f); 
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

        rainAudioSource.Stop();
        windAudioSource.Stop();

        switch (currentWeather)
        {
            case "Sunny":
                sunLight.enabled = true;
                PlayWindSound();
                break;
            case "Rainy":
                rainEffect.SetActive(true);
                PlayRainSound();
                break;
            case "Snowing":
                snowEffect.SetActive(true);
                PlayWindSound();
                break;
            case "Foggy":
                fogEffect.SetActive(true);
                PlayWindSound();
                break;
        }
        currentWeatherText.text = $"Current Weather: {currentWeather}";
    }

    void PlayRainSound()
    {
        if (!rainAudioSource.isPlaying)
        {
            rainAudioSource.Play();
        }
    }

    void PlayWindSound()
    {
        if (!windAudioSource.isPlaying)
        {
            windAudioSource.Play();
        }
    }
}
