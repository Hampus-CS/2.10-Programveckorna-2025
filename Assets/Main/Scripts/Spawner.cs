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

        Debug.Assert(playerSoldierPrefabs.Length > 0, "Player soldier prefabs are missing!");
        Debug.Assert(enemySoldierPrefabs.Length > 0, "Enemy soldier prefabs are missing!");
        Debug.Assert(playerSpawnPoint != null, "Player spawn point is not assigned!");
        Debug.Assert(enemySpawnPoint != null, "Enemy spawn point is not assigned!");
        Debug.Assert(playerLineManager != null, "Player line manager is not assigned!");
        Debug.Assert(enemyLineManager != null, "Enemy line manager is not assigned!");

        ValidatePrefabs(playerSoldierPrefabs, "Player");
        ValidatePrefabs(enemySoldierPrefabs, "Enemy");

    }

    private void ValidatePrefabs(GameObject[] prefabs, string type)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab == null || !prefab.GetComponent<BaseSoldier>())
            {
                Debug.LogError($"Invalid {type} prefab: Missing BaseSoldier component. Prefab: {prefab?.name}");
            }
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
            if (currentPlayerSoldiers >= maxPlayerSoldiers && currentEnemySoldiers >= maxEnemySoldiers)
            {
                Debug.Log("Maximum soldiers reached. Stopping AutoSpawn.");
                yield break;
            }

            if (currentPlayerSoldiers < maxPlayerSoldiers)
            {
                Debug.LogWarning("Attempting to spawn a player soldier...");
                SpawnSoldier(true);
            }

            if (currentEnemySoldiers < maxEnemySoldiers)
            {
                Debug.LogWarning("Attempting to spawn an enemy soldier...");
                SpawnSoldier(false);
            }

            yield return new WaitForSeconds(1f);
        }
    }


    public void SpawnSoldier(bool isPlayer)
    {
        try
        {
            if (isPlayer && currentPlayerSoldiers >= maxPlayerSoldiers) return;
            if (!isPlayer && currentEnemySoldiers >= maxEnemySoldiers) return;

            GameObject[] soldierPrefabs = isPlayer ? playerSoldierPrefabs : enemySoldierPrefabs;
            Transform spawnPoint = isPlayer ? playerSpawnPoint : enemySpawnPoint;
            LineManager initialLine = isPlayer ? playerLineManager : enemyLineManager;

            if (soldierPrefabs == null || soldierPrefabs.Length == 0 || spawnPoint == null || initialLine == null)
            {
                Debug.LogError("Spawner setup is incomplete!");
                return;
            }

            GameObject soldier = Instantiate(soldierPrefabs[Random.Range(0, soldierPrefabs.Length)], spawnPoint.position, Quaternion.identity);

            if (soldier.TryGetComponent<BaseSoldier>(out var baseSoldier))
            {
                baseSoldier.IsPlayer = isPlayer;
                baseSoldier.SetTargetLine(initialLine);

                if (isPlayer) currentPlayerSoldiers++;
                else currentEnemySoldiers++;
            }
            else
            {
                Debug.LogError("Spawned prefab lacks BaseSoldier component!");
                Destroy(soldier); // Clean up invalid prefab
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error spawning soldier: {ex.Message}");
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
