using System.Collections.Generic;
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

    // Save the game state
    public void Save(GameState gameState)
    {
        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game state saved successfully.");
    }

    // Load the game state
    public GameState Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameState gameState = JsonUtility.FromJson<GameState>(json);
            Debug.Log("Game state loaded successfully.");
            return gameState;
        }

        Debug.LogWarning("Save file not found. Returning default state.");
        return null;
    }

    // Represents the game state
    [System.Serializable]
    public class GameState
    {
        public int Scrap;
        public List<WeaponData> Stockpile = new();
        public int ResolutionWidth;
        public int ResolutionHeight;
        public bool IsFullscreen;
        public float MasterVolume;
        public float MusicVolume;
        public float MiscVolume;
    }

    // Represents weapon data for saving/loading
    [System.Serializable]
    public class WeaponData
    {
        public string Name;
        public int Quantity;
        public int Tier;
    }
}

/// <summary>
/// Key Features:
/// 
///     Game State Persistence:
///         - Saves and loads the game state to/from a JSON file.
///         - Handles key game data such as stockpile, resources, and player settings.
/// 
///     Flexible Serialization:
///         - Supports custom serializable classes for modular and scalable saves.
///         - Ensures stockpile and weapon data are preserved with full fidelity.
/// 
///     Debugging Tools:
///         - Outputs save/load status and errors for easier debugging.
/// 
///     Cross-System Integration:
///         - Works seamlessly with GameManager and Settings for data persistence.
/// </summary>

// How to Use
// 1. Attach this script to a GameObject in your scene.
// 2. Use Save(GameState) to serialize and save data.
// 3. Use Load() to deserialize and retrieve saved data.
// 4. Ensure other systems like GameManager and Settings interact with this script for persistence.