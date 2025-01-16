using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class Settings : MonoBehaviour
{
    public SaveHandler saveHandler;

    [SerializeField]
    private TMP_Dropdown resDrop;

    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float miscVolume = 1f;

    private int resolutionWidth = 1920;
    private int resolutionHeight = 1080;
    private bool isFullscreen = true;

    private void Start()
    {
        //LoadSettings();
        //ApplySettings();
        PopulateResolutionDropdown();
    }

    public class ResolutionOptions
    {
        public int width;
        public int height;
        public ResolutionOptions(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return $"{width} x {height}";
        }
    }

    private List<ResolutionOptions> availableResolutions = new()
    {
        new ResolutionOptions(640, 480),
        new ResolutionOptions(1280, 720),
        new ResolutionOptions(1920, 1080),
        new ResolutionOptions(2560, 1440)
    };

    private void PopulateResolutionDropdown()
    {
        resDrop.ClearOptions();

        List<string> options = new();

        foreach (var res in availableResolutions)
        {
            options.Add(res.ToString());
        }

        resDrop.AddOptions(options);
        resDrop.onValueChanged.AddListener(OnResolutionChanged);

        int defaultIndex = availableResolutions.FindIndex(res => res.width == resolutionWidth && res.height == resolutionHeight);
        resDrop.value = defaultIndex >= 0 ? defaultIndex : 2;
        resDrop.RefreshShownValue();
    }

    private void OnResolutionChanged(int index)
    {
        if (index >= 0 && index < availableResolutions.Count)
        {
            ResolutionOptions selectedResolution = availableResolutions[index];
            SetResolution(selectedResolution.width, selectedResolution.height);
            //SaveSettings();
        }
    }

    public void SetResolution(int width, int height)
    {
        resolutionWidth = width;
        resolutionHeight = height;
        ApplySettings();
    }

    public void SetFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp(volume, 0f, 1f);
        AudioListener.volume = masterVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0f, 1f);
        // Skicka värdet till musikhanteraren (om du har en separat musikkomponent)
    }

    public void SaveSettings(SaveHandler saveHandler)
    {
        SaveHandler.GameState gameState = new()
        {
            ResolutionWidth = resolutionWidth,
            ResolutionHeight = resolutionHeight,
            IsFullscreen = isFullscreen,
            MasterVolume = masterVolume,
            MusicVolume = musicVolume,
            MiscVolume = miscVolume
        };
        saveHandler.Save(gameState);
    }

    public void ApplySettings()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, isFullscreen);
        AudioListener.volume = masterVolume;
        Debug.Log($"Settings Applied: {resolutionWidth}x{resolutionHeight}, Fullscreen: {isFullscreen}, MasterVolume: {masterVolume}");
    }

    public void LoadSettings(SaveHandler saveHandler)
    {
        SaveHandler.GameState gameState = saveHandler.Load();
        if (gameState == null) return;

        resolutionWidth = gameState.ResolutionWidth;
        resolutionHeight = gameState.ResolutionHeight;
        isFullscreen = gameState.IsFullscreen;
        masterVolume = gameState.MasterVolume;
        musicVolume = gameState.MusicVolume;
        miscVolume = gameState.MiscVolume;

        ApplySettings();
    }

    public void SetMiscVolume(float volume)
    {
        miscVolume = Mathf.Clamp(volume, 0f, 1f);
    }
}

/// <summary>
/// Key Features:
/// 
///     Resolution Management:
///         - Allows players to select and apply different screen resolutions.
///         - Supports fullscreen mode and dropdown population with options.
/// 
///     Volume Controls:
///         - Handles master, music, and miscellaneous audio volumes.
///         - Provides public methods for volume adjustments.
/// 
///     Save and Load Integration:
///         - Saves and loads player preferences via SaveHandler.
/// 
///     Debugging Tools:
///         - Outputs applied settings to the console for easy debugging.
/// </summary>

// How to Use
// 1. Attach this script to a GameObject in your settings menu.
// 2. Assign TMP_Dropdown and SaveHandler references in the Inspector.
// 3. Call SaveSettings() and LoadSettings() to persist and retrieve preferences.
