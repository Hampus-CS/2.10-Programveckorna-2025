using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject playerSoldierPrefab; // Prefab named PlayerSoldier
    public GameObject enemySoldierPrefab;  // Prefab named EnemySoldier
    public Transform friendlySpawnPoint;  // Spawn point for FriendlyTroop
    public Transform hostileSpawnPoint;   // Spawn point for HostileTroop

    public float spawnInterval = 3f; // Time between spawns
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnSoldiers();
            spawnTimer = 0;
        }
    }

    private void SpawnSoldiers()
    {
        GameObject playerSoldier = Instantiate(playerSoldierPrefab, friendlySpawnPoint.position, Quaternion.identity);
        playerSoldier.tag = "FriendlyTroop";
        playerSoldier.GetComponent<BaseSoldier>().SetPlayerStatus(true);

        GameObject enemySoldier = Instantiate(enemySoldierPrefab, hostileSpawnPoint.position, Quaternion.identity);
        enemySoldier.tag = "HostileTroop";
        enemySoldier.GetComponent<BaseSoldier>().SetPlayerStatus(false);
    }
}