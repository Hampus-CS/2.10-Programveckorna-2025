using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject playerSoldierPrefab; // Prefab named PlayerSoldier
    public GameObject enemySoldierPrefab;  // Prefab named EnemySoldier
    public Transform friendlySpawnPoint;  // Spawn point for FriendlyTroop
    public Transform hostileSpawnPoint;   // Spawn point for HostileTroop

    public int maxSoldiers = 45; // Maximalt antal soldater som kan existera samtidigt
    private int currentSoldiers = 0; // Håller reda på nuvarande antal soldater

    public float playerSpawnInterval = 5f;
    public float enemySpawnInterval = 5f;

    private float playerSpawnTimer;
    private float enemySpawnTimer;

    private void Update()
    {
        playerSpawnTimer += Time.deltaTime;
        enemySpawnTimer += Time.deltaTime;

        if (playerSpawnTimer >= playerSpawnInterval)
        {
            SpawnPlayerSoldier();
            playerSpawnTimer = 0;
        }

        if (enemySpawnTimer >= enemySpawnInterval)
        {
            SpawnEnemySoldier();
            enemySpawnTimer = 0;
        }
    }

    private void SpawnPlayerSoldier()
    {
        if (currentSoldiers >= maxSoldiers)
        {
            Debug.LogWarning("Maximum soldier limit reached. No player soldier will be spawned.");
            return;
        }

        GameObject playerSoldier = Instantiate(playerSoldierPrefab, friendlySpawnPoint.position, Quaternion.identity);
        playerSoldier.tag = "FriendlyTroop";
        playerSoldier.GetComponent<BaseSoldier>().SetPlayerStatus(true);
        currentSoldiers++;
        //Debug.Log($"Player soldier spawned at {friendlySpawnPoint.position}. Current soldiers: {currentSoldiers}");
    }

    private void SpawnEnemySoldier()
    {
        if (currentSoldiers >= maxSoldiers)
        {
            Debug.LogWarning("Maximum soldier limit reached. No enemy soldier will be spawned.");
            return;
        }

        GameObject enemySoldier = Instantiate(enemySoldierPrefab, hostileSpawnPoint.position, Quaternion.identity);
        enemySoldier.tag = "HostileTroop";
        enemySoldier.GetComponent<BaseSoldier>().SetPlayerStatus(false);
        currentSoldiers++;
        //Debug.Log($"Enemy soldier spawned at {hostileSpawnPoint.position}. Current soldiers: {currentSoldiers}");
    }

    public void DecreaseSoldierCount()
    {
        currentSoldiers = Mathf.Max(0, currentSoldiers - 1);
        Debug.Log($"Soldier removed. Current soldiers: {currentSoldiers}");
    }

}