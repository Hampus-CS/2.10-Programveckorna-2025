using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private static Spawner instance; // Singleton instance for the Spawner

    [Header("Spawning Settings")]
    public GameObject[] playerSoldierPrefabs;
    public GameObject[] enemySoldierPrefabs;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public float playerSpawnInterval = 5f;
    public float enemySpawnInterval = 8f;

    [Header("Line Assignments")]
    public LineManager playerLineManager;
    public LineManager enemyLineManager;

    [Header("Soldier Caps")]
    public int maxPlayerSoldiers = 30;
    public int maxEnemySoldiers = 30;
    private int currentPlayerSoldiers = 0;
    private int currentEnemySoldiers = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Debug.LogWarning("Starting AutoSpawn Coroutine...");
        StartCoroutine(AutoSpawn());
    }

    private void Update()
    {
        Debug.LogWarning($"Player: {currentPlayerSoldiers}, Enemy: {currentEnemySoldiers}");
    }

    private IEnumerator AutoSpawn()
    {
        while (true)
        {
            Debug.LogWarning($"AutoSpawn running... Player: {currentPlayerSoldiers}/{maxPlayerSoldiers}, Enemy: {currentEnemySoldiers}/{maxEnemySoldiers}");

            // Player spawn logic
            if (currentPlayerSoldiers < maxPlayerSoldiers)
            {
                Debug.LogWarning("Attempting to spawn a player soldier...");
                SpawnSoldier(true);
            }

            // Enemy spawn logic
            if (currentEnemySoldiers < maxEnemySoldiers)
            {
                Debug.LogWarning("Attempting to spawn an enemy soldier...");
                SpawnSoldier(false);
            }

            // Wait before checking again
            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnSoldier(bool isPlayer)
    {
        // Check if soldier limit has been reached
        if (isPlayer && currentPlayerSoldiers >= maxPlayerSoldiers)
        {
            Debug.LogWarning("Player soldier limit reached. No spawn.");
            return;
        }
        else if (!isPlayer && currentEnemySoldiers >= maxEnemySoldiers)
        {
            Debug.LogWarning("Enemy soldier limit reached. No spawn.");
            return;
        }

        // Get the appropriate prefab and spawn point
        GameObject[] soldierPrefabs = isPlayer ? playerSoldierPrefabs : enemySoldierPrefabs;
        Transform spawnPoint = isPlayer ? playerSpawnPoint : enemySpawnPoint;
        LineManager initialLine = isPlayer ? playerLineManager : enemyLineManager;

        if (soldierPrefabs == null || soldierPrefabs.Length == 0)
        {
            Debug.LogWarning("No soldier prefabs assigned!");
            return;
        }

        if (spawnPoint == null || initialLine == null)
        {
            Debug.LogWarning("Spawner setup is incomplete!");
            return;
        }

        // Spawn a random soldier
        int randomIndex = Random.Range(0, soldierPrefabs.Length);
        GameObject soldier = Instantiate(soldierPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        // Assign properties to the spawned soldier
        var baseSoldier = soldier.GetComponent<BaseSoldier>();
        if (baseSoldier != null)
        {
            baseSoldier.IsPlayer = isPlayer;
            baseSoldier.SetTargetLine(initialLine);
        }

        // Update the soldier count
        if (isPlayer)
        {
            currentPlayerSoldiers++;
            Debug.LogWarning($"Player soldier spawned. Current count: {currentPlayerSoldiers}/{maxPlayerSoldiers}");
        }
        else
        {
            currentEnemySoldiers++;
            Debug.LogWarning($"Enemy soldier spawned. Current count: {currentEnemySoldiers}/{maxEnemySoldiers}");
        }
    }

    public void SoldierDied(bool isPlayer)
    {
        // Decrement the soldier count when a soldier dies
        if (isPlayer)
        {
            currentPlayerSoldiers = Mathf.Max(0, currentPlayerSoldiers - 1);
        }
        else
        {
            currentEnemySoldiers = Mathf.Max(0, currentEnemySoldiers - 1);
        }

        Debug.LogWarning($"Soldier died. Updated counts - Players: {currentPlayerSoldiers}, Enemies: {currentEnemySoldiers}");
    }
}
