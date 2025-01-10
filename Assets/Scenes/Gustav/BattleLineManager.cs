using System.Collections.Generic;
using UnityEngine;

public class BattleLineManager : MonoBehaviour
{
    public List<GameObject> lineObjects = new List<GameObject>();
    public List<BattleLine> lines = new List<BattleLine>();

    void Start()
    {
        foreach (var lineObject in lineObjects)
        {
            lines.Add(new BattleLine(lineObject));
        }
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
    private List<CoverScript> waypoints;

    private int playerTroops;
    private int enemyTroops;

    public BattleLine(GameObject lineObject)
    {
        this.lineObject = lineObject;
        InitializeWaypoints();
    }

    private void InitializeWaypoints()
    {
        waypoints = new List<CoverScript>();

        if (lineObject == null)
        {
            Debug.LogError("Line object is null!");
            return;
        }

        foreach (Transform child in lineObject.transform)
        {
            var coverScript = child.GetComponent<CoverScript>();
            if (coverScript != null)
            {
                waypoints.Add(coverScript);
                Debug.Log($"Added waypoint: {child.name}");
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
            Debug.LogWarning($"No waypoints to evaluate for line {lineObject.name}");
            return;
        }

        playerTroops = 0;
        enemyTroops = 0;

        foreach (var waypoint in waypoints)
        {
            if (waypoint.occupiedBy == "Player")
            {
                playerTroops++;
            }
            else if (waypoint.occupiedBy == "Enemy")
            {
                enemyTroops++;
            }
        }

        Debug.Log($"Line {lineObject.name} - Player Troops: {playerTroops}, Enemy Troops: {enemyTroops}");
    }

    public bool IsControlledByPlayer()
    {
        return playerTroops > waypoints.Count / 2;
    }

    public bool IsControlledByEnemy()
    {
        return enemyTroops > waypoints.Count / 2;
    }

    public bool CanAttackNextLine()
    {
        return IsControlledByPlayer() || IsControlledByEnemy();
    }

    public void StartBattle()
    {
        Debug.Log($"Battle started on line {lineObject.name}!");
    }
}