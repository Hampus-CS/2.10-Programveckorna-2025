using System.IO;
using UnityEngine;

// Handles saving and loading the game state to/from a JSON file.
public class SaveHandler : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
    }

    // Saves the entire game state.
    public void Save(GameState gameState)
    {
        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game state saved successfully.");
    }

    // Loads the entire game state.
    public GameState Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameState state = JsonUtility.FromJson<GameState>(json);
            Debug.Log("Game state loaded successfully.");
            return state;
        }

        Debug.LogWarning("Save file not found. Returning default state.");
        return null;
    }
}

// Represents the entire game state for saving/loading.
[System.Serializable]
public class GameState
{

    public string GameDate; // Example of additional data
    public int PlayerLevel; // Example of game progress
    public int Score; // Example of player score
    public int ResolutionWidth = 1920; // Default resolution width
    public int ResolutionHeight = 1080; // Default resolution height
    public bool IsFullscreen = true;
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;
    public float MiscVolume = 1f;

}