using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject playerSoldierPrefab; // Prefab named PlayerSoldier
    public GameObject enemySoldierPrefab;  // Prefab named EnemySoldier
    public Transform friendlySpawnPoint;  // Spawn point for FriendlyTroop
    public Transform hostileSpawnPoint;   // Spawn point for HostileTroop

    public int maxSoldiers = 50; // Maximalt antal soldater som kan existera samtidigt
    private int currentSoldiers = 0; // Håller reda på nuvarande antal soldater

    public float spawnInterval = 5f; // Time between spawns
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && currentSoldiers < maxSoldiers)
        {
            SpawnSoldiers();
            spawnTimer = 0;
        }
    }

    private void SpawnSoldiers()
    {
        Line[] lines = FindObjectsOfType<Line>();
        foreach (var line in lines)
        {
            if (!line.HasFreeCapacity())
            {
                Debug.LogWarning($"Line {line.name} is full. No more soldiers will be spawned.");
                continue;
            }

            // Spawna en spelarsoldat
            GameObject playerSoldier = Instantiate(playerSoldierPrefab, friendlySpawnPoint.position, Quaternion.identity);
            playerSoldier.tag = "FriendlyTroop";
            playerSoldier.GetComponent<BaseSoldier>().SetPlayerStatus(true);
            currentSoldiers++;

            // Spawna en fiendesoldat
            GameObject enemySoldier = Instantiate(enemySoldierPrefab, hostileSpawnPoint.position, Quaternion.identity);
            enemySoldier.tag = "HostileTroop";
            enemySoldier.GetComponent<BaseSoldier>().SetPlayerStatus(false);
            currentSoldiers++;
        }
    }

    public void DecreaseSoldierCount()
    {
        currentSoldiers = Mathf.Max(0, currentSoldiers - 1);
        Debug.Log($"Soldier removed. Current soldiers: {currentSoldiers}");
    }

}