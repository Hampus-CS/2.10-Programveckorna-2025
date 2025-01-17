using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    /*public enum LineState { Neutral, PlayerOwned, EnemyOwned, Contested }

    [Header("Line Settings")]
    public Transform[] slots; // Slots belonging to this line
    public LineState CurrentState = LineState.Neutral;

    [Header("Debug Settings")]
    public bool ShowDebugGizmos = true; // Toggle Gizmo display
    public Color PlayerOwnedColor = Color.green;
    public Color EnemyOwnedColor = Color.red;
    public Color ContestedColor = Color.yellow;
    public Color NeutralColor = Color.gray;

    private List<GameObject> playerSoldiers = new List<GameObject>();
    private List<GameObject> enemySoldiers = new List<GameObject>();

    // Public API for line queries
    public bool IsPlayerControlled => CurrentState == LineState.PlayerOwned;
    public bool IsEnemyControlled => CurrentState == LineState.EnemyOwned;
    public bool IsContested => CurrentState == LineState.Contested;

    private LineState previousState;

    private void Update()
    {
        if (CurrentState != previousState)
        {
            previousState = CurrentState;
        }

        EvaluateOwnership();
    }


    public void AddSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
            playerSoldiers.Add(soldier);
        else
            enemySoldiers.Add(soldier);

        EvaluateOwnership();
    }

    public void RemoveSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
            playerSoldiers.Remove(soldier);
        else
            enemySoldiers.Remove(soldier);

        EvaluateOwnership();
    }

    private void EvaluateOwnership()
    {
        int playerCount = playerSoldiers.Count;
        int enemyCount = enemySoldiers.Count;

        if (playerCount > enemyCount)
        {
            CurrentState = LineState.PlayerOwned;
        }
        else if (enemyCount > playerCount)
        {
            CurrentState = LineState.EnemyOwned;
        }
        else if (playerCount > 0 || enemyCount > 0)
        {
            CurrentState = LineState.Contested;
        }
        else
        {
            CurrentState = LineState.Neutral;
        }
    }

    public Transform GetFreeSlot()
    {
        foreach (var slot in slots)
        {
            var slotScript = slot.GetComponent<SlotManager>();
            if (slotScript != null && slotScript.IsFree())
            {
                return slot;
            }
        }
        Debug.LogWarning($"No free slots in line {name}.");
        return null;
    }


    public void CommandSoldiersToStorm(LineManager targetLine)
    {
        List<GameObject> soldiers = IsPlayerControlled ? playerSoldiers : enemySoldiers;

        foreach (var soldierObj in soldiers)
        {
            if (soldierObj == null) continue;

            var soldier = soldierObj.GetComponent<BaseSoldier>();
            if (soldier != null)
            {
                soldier.SetTargetLine(targetLine);
            }
        }
        Debug.Log($"{name}: Soldiers storming {targetLine.name}");
    }

    private void OnDrawGizmos()
    {
        if (!ShowDebugGizmos) return;

        // Set Gizmo color based on ownership state
        Gizmos.color = CurrentState switch
        {
            LineState.PlayerOwned => PlayerOwnedColor,
            LineState.EnemyOwned => EnemyOwnedColor,
            LineState.Contested => ContestedColor,
            _ => NeutralColor
        };

        // Draw the line as a wire rectangle
        Gizmos.DrawWireCube(transform.position, new Vector3(10, 1, 1)); // Adjust dimensions as needed

        // Draw slots as smaller spheres
        Gizmos.color = Color.blue;
        foreach (var slot in slots)
        {
            if (slot != null)
                Gizmos.DrawWireSphere(slot.position, 0.5f);
        }
    }*/
}


/// <summary>
/// Key Features
/// 
///     Dynamic Ownership:
///         - Tracks soldiers in the line and determines ownership dynamically.
///         - Ownership can toggle between Neutral, PlayerOwned, EnemyOwned, and Contested.
/// 
///     Storming System:
///         - Soldiers can be commanded to storm a target line via CommandSoldiersToStorm().
/// 
///     Debugging Gizmos:
///         - Visual feedback for ownership and slot positions.
///         - Adjustable colors for ownership states.
/// 
///     Slot Management:
///         - Provides an interface to check and use free slots.
/// </summary>

// How to Use
// Integrate this LineManager.cs into your scene. Add it to all Line GameObjects.