using UnityEngine;

public class Settings : MonoBehaviour
{
    public SaveHandler saveHandler;

    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float miscVolume = 1f;

    private int resolutionWidth = 1920;
    private int resolutionHeight = 1080;
    private bool isFullscreen = true;

    private void Start()
    {
        LoadSettings();
        ApplySettings();
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
        ApplySettings();
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

    public void SetMiscVolume(float volume)
    {
        miscVolume = Mathf.Clamp(volume, 0f, 1f);
        // Skicka värdet till ljudeffekthanteraren (om du har en separat ljudkomponent)
    }

    public void SaveSettings()
    {
        GameState currentState = new GameState
        {
            ResolutionWidth = resolutionWidth,
            ResolutionHeight = resolutionHeight,
            IsFullscreen = isFullscreen,
            MasterVolume = masterVolume,
            MusicVolume = musicVolume,
            MiscVolume = miscVolume
        };
        saveHandler.Save(currentState);
    }

    private void LoadSettings()
    {
        GameState loadedState = saveHandler.Load();
        if (loadedState != null)
        {
            resolutionWidth = loadedState.ResolutionWidth;
            resolutionHeight = loadedState.ResolutionHeight;
            isFullscreen = loadedState.IsFullscreen;
            masterVolume = loadedState.MasterVolume;
            musicVolume = loadedState.MusicVolume;
            miscVolume = loadedState.MiscVolume;
        }
    }

    private void ApplySettings()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, isFullscreen);
        AudioListener.volume = masterVolume;
        Debug.Log($"Settings Applied: {resolutionWidth}x{resolutionHeight}, Fullscreen: {isFullscreen}, MasterVolume: {masterVolume}");
    }
}
