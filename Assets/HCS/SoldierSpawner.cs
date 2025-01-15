using System.Diagnostics;
using System.Linq;
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

    private float enemyAttackInterval = 10f; // Time between enemy attacks
    private float enemyAttackTimer = 0f;

    BaseSoldier baseSoldier;

    private void Update()
    {
        playerSpawnTimer += Time.deltaTime;
        enemySpawnTimer += Time.deltaTime;
        enemyAttackTimer += Time.deltaTime;

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

        if (enemyAttackTimer >= enemyAttackInterval)
        {
            PeriodicEnemyStorm();
            enemyAttackTimer = 0;
        }
    }

    public void PeriodicEnemyStorm()
    {
        Line[] lines = FindObjectsOfType<Line>();
        Line lastEnemyLine = lines.FirstOrDefault(line => line.CurrentState == Line.LineState.EnemyOwned);

        // If no enemy-owned line exists, fallback to neutral lines
        if (lastEnemyLine == null)
        {
            lastEnemyLine = lines.FirstOrDefault(line => line.CurrentState == Line.LineState.Neutral);
            if (lastEnemyLine == null)
            {
                Debug.LogWarning("No enemy-owned or neutral line found. Skipping periodic enemy storm.");
                return;
            }
        }

        // Find the nearest player-owned or neutral line
        Line nearestPlayerLine = lines
            .OrderBy(line => Vector3.Distance(lastEnemyLine.transform.position, line.transform.position))
            .FirstOrDefault(line => line.CurrentState == Line.LineState.PlayerOwned || line.CurrentState == Line.LineState.Neutral);

        if (nearestPlayerLine != null)
        {
            lastEnemyLine.CommandSoldiersToStorm(nearestPlayerLine);
            Debug.Log($"Enemy soldiers commanded to storm line: {nearestPlayerLine.name}");
        }
        else
        {
            Debug.LogWarning("No valid target line found for enemy storm.");
        }
    }

    public void CommandAllSoldiersToStorm(Line targetLine, bool isPlayer)
    {
        Line[] lines = FindObjectsOfType<Line>();
        Line lastLine = isPlayer
            ? lines.LastOrDefault(line => line.CurrentState == Line.LineState.PlayerOwned)
            : lines.FirstOrDefault(line => line.CurrentState == Line.LineState.EnemyOwned);

        if (lastLine != null)
        {
            lastLine.CommandSoldiersToStorm(targetLine);
        }
    }

    public void PlayerCommandStorm()
    {
        Line[] lines = FindObjectsOfType<Line>();
        Line lastPlayerLine = lines.LastOrDefault(line => line.CurrentState == Line.LineState.PlayerOwned);

        // If no player-owned line exists, fallback to neutral lines
        if (lastPlayerLine == null)
        {
            lastPlayerLine = lines.FirstOrDefault(line => line.CurrentState == Line.LineState.Neutral);
            if (lastPlayerLine == null)
            {
                Debug.LogWarning("No player-owned or neutral line found. Skipping player storm.");
                return;
            }
        }

        // Find the nearest enemy-owned or neutral line
        Line nearestEnemyLine = lines
            .OrderBy(line => Vector3.Distance(lastPlayerLine.transform.position, line.transform.position))
            .FirstOrDefault(line => line.CurrentState == Line.LineState.EnemyOwned || line.CurrentState == Line.LineState.Neutral);

        if (nearestEnemyLine != null)
        {
            lastPlayerLine.CommandSoldiersToStorm(nearestEnemyLine);
            Debug.Log($"Player soldiers commanded to storm line: {nearestEnemyLine.name}");
        }
        else
        {
            Debug.LogWarning("No valid target line found for player storm.");
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