using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleLineManager : MonoBehaviour
{
    public List<BattleLine> lines = new List<BattleLine>(); // List of all lines on the battlefield

    void Start()
    {
        GameObject[] lineObjects = GameObject.FindGameObjectsWithTag("Line");
        Debug.Log($"Found {lineObjects.Length} objects with the 'Line' tag.");

        if (lineObjects.Length == 0)
        {
            Debug.LogWarning("No Line objects found in the scene! Ensure the Line tag is assigned.");
        }

        foreach (var lineObject in lineObjects)
        {
            if (lineObject != null)
            {
                Debug.Log($"Adding line object: {lineObject.name}");
                lines.Add(new BattleLine(lineObject));
            }
            else
            {
                Debug.LogError("A line object was null. Check scene hierarchy.");
            }
        }

        Debug.Log(lines.Count > 0
            ? $"Successfully added {lines.Count} lines to the BattleLineManager."
            : "No lines were added to the BattleLineManager.");
    }

    void Update()
    {
        foreach (var line in lines)
        {
            line.EvaluateOwnership();
        }

        for (int i = 0; i < lines.Count - 1; i++)
        {
            var currentLine = lines[i];
            var nextLine = lines[i + 1];

            if (currentLine.IsControlledByPlayer() && currentLine.CanAttackNextLine())
            {
                nextLine.StartBattle();
            }

            if (currentLine.IsControlledByEnemy() && currentLine.CanAttackNextLine())
            {
                nextLine.StartBattle();
            }
        }
    }
}

[System.Serializable]
public class BattleLine
{
    public GameObject lineObject;
    private List<CoverScript> waypoints = new List<CoverScript>(); // Ensure non-static instance

    // Define playerTroops and enemyTroops as instance variables
    private int playerTroops = 0;
    private int enemyTroops = 0;

    private Queue<int> playerTroopHistory = new Queue<int>();
    private Queue<int> enemyTroopHistory = new Queue<int>();
    private int historySize = 10; // Number of frames to average


    public BattleLine(GameObject lineObject)
    {
        this.lineObject = lineObject;
        InitializeWaypoints();
    }

    private void InitializeWaypoints()
    {
        if (lineObject == null)
        {
            Debug.LogError("Line object is null! Ensure all line objects are assigned correctly.");
            return;
        }

        Debug.Log($"Initializing waypoints for line {lineObject.name}");

        foreach (Transform child in lineObject.transform)
        {
            var coverScript = child.GetComponent<CoverScript>();
            if (coverScript != null)
            {
                waypoints.Add(coverScript);
                Debug.Log($"Added waypoint: {child.name} to line {lineObject.name}");
            }
            else
            {
                Debug.LogWarning($"Child {child.name} of {lineObject.name} does not have a CoverScript!");
            }
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError($"No waypoints found for line {lineObject.name}. Ensure children have CoverScript.");
        }
    }

    public void EvaluateOwnership()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning($"No waypoints to evaluate for line {lineObject?.name ?? "null"}");
            return;
        }

        int currentPlayerTroops = 0;
        int currentEnemyTroops = 0;

        foreach (var waypoint in waypoints)
        {
            if (waypoint.occupiedBy == "Player")
            {
                currentPlayerTroops++;
            }
            else if (waypoint.occupiedBy == "Enemy")
            {
                currentEnemyTroops++;
            }
        }

        // Add to history
        playerTroopHistory.Enqueue(currentPlayerTroops);
        enemyTroopHistory.Enqueue(currentEnemyTroops);

        if (playerTroopHistory.Count > historySize)
        {
            playerTroopHistory.Dequeue();
            enemyTroopHistory.Dequeue();
        }

        // Average troop counts
        playerTroops = (int)(playerTroopHistory.Average());
        enemyTroops = (int)(enemyTroopHistory.Average());

        Debug.Log($"Line {lineObject.name} - Player Troops: {playerTroops}, Enemy Troops: {enemyTroops}");
    }

    public bool IsControlledByPlayer()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning($"Cannot determine control for line {lineObject?.name ?? "null"} - No waypoints available.");
            return false;
        }

        return playerTroops > waypoints.Count / 2;
    }

    public bool IsControlledByEnemy()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning($"Cannot determine control for line {lineObject?.name ?? "null"} - No waypoints available.");
            return false;
        }

        return enemyTroops > waypoints.Count / 2;
    }

    public bool CanAttackNextLine()
    {
        return IsControlledByPlayer() || IsControlledByEnemy();
    }

    public void StartBattle()
    {
        Debug.Log($"Battle started on line {lineObject.name}!");
        // Add any additional logic for starting the battle on this line
    }
}