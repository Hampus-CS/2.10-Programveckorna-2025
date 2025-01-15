using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public enum LineState { Neutral, PlayerOwned, EnemyOwned, Contested }
    public LineState CurrentState = LineState.Neutral;

    public Transform[] Slots; // Slot positions
    public List<GameObject> PlayerSoldiers = new List<GameObject>();
    public List<GameObject> EnemySoldiers = new List<GameObject>();

    [Header("Debug Settings")]
    public bool ShowGizmos = true; // Toggle Gizmos visibility in the Inspector

    private void Awake()
    {
        EvaluateOwnership();
    }

    public void EvaluateOwnership()
    {
        int playerCount = PlayerSoldiers.Count;
        int enemyCount = EnemySoldiers.Count;

        if (playerCount > enemyCount)
            CurrentState = LineState.PlayerOwned;
        else if (enemyCount > playerCount)
            CurrentState = LineState.EnemyOwned;
        else if (playerCount > 0 || enemyCount > 0)
            CurrentState = LineState.Contested;
        else
            CurrentState = LineState.Neutral;

        Debug.Log($"Ownership of {name} updated to {CurrentState}");
    }

    public bool IsContested()
    {
        return PlayerSoldiers.Count > 0 && EnemySoldiers.Count > 0;
    }

    public Transform GetFreeSlot()
    {
        foreach (var slot in Slots)
        {
            if (slot.GetComponent<Slot>().IsFree())
                return slot;
        }
        return null; // No free slots available
    }

    public void AddSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
            PlayerSoldiers.Add(soldier);
        else
            EnemySoldiers.Add(soldier);

        EvaluateOwnership();
    }

    public void RemoveSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
            PlayerSoldiers.Remove(soldier);
        else
            EnemySoldiers.Remove(soldier);

        EvaluateOwnership();
    }

    public void CommandSoldiersToStorm(Line targetLine)
    {
        List<GameObject> soldiers = CurrentState == LineState.PlayerOwned ? PlayerSoldiers : EnemySoldiers;

        foreach (var soldierObj in soldiers)
        {
            if (soldierObj == null) continue;

            var soldier = soldierObj.GetComponent<BaseSoldier>();
            if (soldier != null)
            {
                soldier.SetTargetLine(targetLine);
            }
        }

        Debug.Log($"{name} soldiers storming {targetLine.name}");
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos) return;

        // Draw the line as a rectangle
        Gizmos.color = CurrentState switch
        {
            LineState.PlayerOwned => Color.green,
            LineState.EnemyOwned => Color.red,
            LineState.Contested => Color.yellow,
            _ => Color.gray,
        };

        Gizmos.DrawWireCube(transform.position, new Vector3(10, 1, 1)); // Adjust dimensions as needed

        // Draw slots as spheres
        Gizmos.color = Color.blue;
        foreach (var slot in Slots)
        {
            if (slot != null)
                Gizmos.DrawWireSphere(slot.position, 0.5f);
        }
    }
}
