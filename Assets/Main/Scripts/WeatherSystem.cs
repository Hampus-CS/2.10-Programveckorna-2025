using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    private List<string> weatherConditions = new List<string> { "Sunny", "Rainy", "Snowing", "Foggy" };

    public GameObject rainEffect;
    public GameObject snowEffect;
    public GameObject fogEffect;
    public Light sunLight;

    public AudioSource rainAudioSource;
    public AudioSource windAudioSource;

    private string currentWeather;

    void Start()
    {
        // Ensure all effects are turned off initially
        DisableAllEffects();
        ChangeWeather();
        InvokeRepeating(nameof(ChangeWeather), 120f, 120f);
    }

    void ChangeWeather()
    {
        currentWeather = weatherConditions[Random.Range(0, weatherConditions.Count)];
        UpdateWeatherEffects();
    }

    void UpdateWeatherEffects()
    {
        // Disable all weather effects
        DisableAllEffects();

        switch (currentWeather)
        {
            case "Sunny":
                if (sunLight != null) sunLight.enabled = true;
                PlayWindSound();
                break;
            case "Rainy":
                if (rainEffect != null) rainEffect.SetActive(true);
                PlayRainSound();
                break;
            case "Snowing":
                if (snowEffect != null) snowEffect.SetActive(true);
                PlayWindSound();
                break;
            case "Foggy":
                if (fogEffect != null) fogEffect.SetActive(true);
                PlayWindSound();
                break;
        }
    }

    void DisableAllEffects()
    {
        if (rainEffect != null) rainEffect.SetActive(false);
        if (snowEffect != null) snowEffect.SetActive(false);
        if (fogEffect != null) fogEffect.SetActive(false);
        if (sunLight != null) sunLight.enabled = false;

        if (rainAudioSource != null) rainAudioSource.Stop();
        if (windAudioSource != null) windAudioSource.Stop();
    }

    void PlayRainSound()
    {
        if (rainAudioSource != null && !rainAudioSource.isPlaying)
        {
            rainAudioSource.Play();
        }
    }

    void PlayWindSound()
    {
        if (windAudioSource != null && !windAudioSource.isPlaying)
        {
            windAudioSource.Play();
        }
    }
}