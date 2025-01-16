using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject playerSoldierPrefab; // Prefab for player soldiers
    public GameObject enemySoldierPrefab; // Prefab for enemy soldiers
    public Transform playerSpawnPoint;    // Spawn point for player soldiers
    public Transform enemySpawnPoint;    // Spawn point for enemy soldiers
    public float playerSpawnInterval = 5f; // Time between player spawns
    public float enemySpawnInterval = 8f; // Time between enemy spawns

    private float playerSpawnTimer;
    private float enemySpawnTimer;

    [Header("Line Assignments")]
    public LineManager playerLineManager; // The first line for player soldiers
    public LineManager enemyLineManager; // The first line for enemy soldiers

    [Header("Soldier Caps")]
    public int maxPlayerSoldiers = 30; // Max player soldiers allowed on the field
    public int maxEnemySoldiers = 30;  // Max enemy soldiers allowed on the field
    private int currentPlayerSoldiers = 0; // Tracks number of player soldiers
    private int currentEnemySoldiers = 0;  // Tracks number of enemy soldiers

    private void Update()
    {
        HandleAutoSpawning();
    }

    private void HandleAutoSpawning()
    {
        // Handle player soldier spawning
        playerSpawnTimer += Time.deltaTime;
        if (playerSpawnTimer >= playerSpawnInterval && currentPlayerSoldiers < maxPlayerSoldiers)
        {
            SpawnSoldier(true);
            playerSpawnTimer = 0f;
        }

        // Handle enemy soldier spawning
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= enemySpawnInterval && currentEnemySoldiers < maxEnemySoldiers)
        {
            SpawnSoldier(false);
            enemySpawnTimer = 0f;
        }
    }

    public void SpawnSoldier(bool isPlayer)
    {
        GameObject soldierPrefab = isPlayer ? playerSoldierPrefab : enemySoldierPrefab;

        if (soldierPrefab == null)
        {
            Debug.LogError($"Spawner: {(isPlayer ? "Player" : "Enemy")} Soldier Prefab is not assigned!");
            return;
        }

        if (soldierPrefab.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
        {
            Debug.LogError($"Spawner: {(isPlayer ? "Player" : "Enemy")} Soldier Prefab is missing a NavMeshAgent!");
            return;
        }

        Transform spawnPoint = isPlayer ? playerSpawnPoint : enemySpawnPoint;
        LineManager initialLine = isPlayer ? playerLineManager : enemyLineManager;

        if (initialLine == null)
        {
            Debug.LogError($"Spawner: Initial line for {(isPlayer ? "Player" : "Enemy")} Soldiers is not set!");
            return;
        }

        GameObject soldier = Instantiate(soldierPrefab, spawnPoint.position, Quaternion.identity);
        var baseSoldier = soldier.GetComponent<BaseSoldier>();

        if (baseSoldier != null)
        {
            baseSoldier.IsPlayer = isPlayer;
            baseSoldier.SetTargetLine(initialLine);
        }

        if (isPlayer)
            currentPlayerSoldiers++;
        else
            currentEnemySoldiers++;

        Debug.Log($"{soldier.name} spawned at {spawnPoint.position}");
    }

    public void SoldierDied(bool isPlayer)
    {
        // Decrement the soldier count when a soldier dies
        if (isPlayer)
            currentPlayerSoldiers--;
        else
            currentEnemySoldiers--;
    }

}

/// <summary>
/// Key Features
///     
///     Dynamic Spawning:
///         - Automatically spawns soldiers for both sides at configurable intervals.
///         - Allows manual spawning via the SpawnSoldier(bool isPlayer) method.
///
///     Initial Line Assignment:
///         - Automatically assigns spawned soldiers to their respective starting lines (playerLineManager or enemyLineManager).
/// 
///     Customizable Settings:
///         - Separate spawn intervals for players and enemies.
///         - Easily adjustable spawn points and soldier prefabs.
///     
///     Debugging:
///         - Outputs logs for each soldier spawned, including their spawn location.
/// </summary>

// How to Use
// 1. Attach the Spawner.cs script to a GameObject in your scene.
// 2. Assign the following in the Unity Inspector:
//      - playerSoldierPrefab and enemySoldierPrefab (your soldier prefabs).
//      - playerSpawnPoint and enemySpawnPoint (empty GameObjects marking spawn locations).
//      - playerLineManager and enemyLineManager (the first lines for each side).
// 3. Adjust playerSpawnInterval and enemySpawnInterval to balance the game.