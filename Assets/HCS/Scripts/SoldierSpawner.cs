using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    public GameObject playerSoldierPrefab;
    public GameObject enemySoldierPrefab;
    public Transform friendlySpawnPoint;
    public Transform hostileSpawnPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            SpawnPlayerSoldier();

        if (Input.GetKeyDown(KeyCode.E))
            SpawnEnemySoldier();
    }

    public void SpawnPlayerSoldier()
    {
        GameObject soldier = Instantiate(playerSoldierPrefab, friendlySpawnPoint.position, Quaternion.identity);
        var baseSoldier = soldier.GetComponent<BaseSoldier>();
        baseSoldier?.SetPlayerStatus(true);
    }

    public void SpawnEnemySoldier()
    {
        GameObject soldier = Instantiate(enemySoldierPrefab, hostileSpawnPoint.position, Quaternion.identity);
        var baseSoldier = soldier.GetComponent<BaseSoldier>();
        baseSoldier?.SetPlayerStatus(false);
    }
}
