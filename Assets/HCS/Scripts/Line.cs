using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Line : MonoBehaviour
{
    public int TotalSlots = 5;
    public Transform[] Slots;
    public List<GameObject> PlayerSoldiers = new List<GameObject>();
    public List<GameObject> EnemySoldiers = new List<GameObject>();

    public enum LineState { PlayerOwned, EnemyOwned, Contested, Neutral }
    public LineState CurrentState;

    private void Awake()
    {
        if (Slots.Length != TotalSlots)
        {
            Debug.LogWarning($"Mismatch between TotalSlots ({TotalSlots}) and actual slot count ({Slots.Length}) on line {name}. Adjusting TotalSlots.");
            TotalSlots = Slots.Length;
        }

        // Find spawn points
        Transform friendlySpawn = GameObject.Find("FriendlySpawnPoint")?.transform;
        Transform hostileSpawn = GameObject.Find("HostileSpawnPoint")?.transform;

        if (friendlySpawn == null || hostileSpawn == null)
        {
            Debug.LogError("Spawn points not found. Ensure they are named 'FriendlySpawnPoint' and 'HostileSpawnPoint'.");
            return;
        }

        // Assign initial ownership based on proximity to spawn points
        float playerDistance = Vector3.Distance(transform.position, friendlySpawn.position);
        float enemyDistance = Vector3.Distance(transform.position, hostileSpawn.position);

        if (playerDistance < enemyDistance)
        {
            CurrentState = LineState.PlayerOwned;
        }
        else if (enemyDistance < playerDistance)
        {
            CurrentState = LineState.EnemyOwned;
        }
        else
        {
            CurrentState = LineState.Neutral;
        }

        Debug.Log($"Line {name} initialized with state: {CurrentState}");
    }


    public void RegisterSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
        {
            PlayerSoldiers.Add(soldier);
        }
        else
        {
            EnemySoldiers.Add(soldier);
        }

        Debug.Log($"Soldier {soldier.name} registered on line {name}. Player soldiers: {PlayerSoldiers.Count}, Enemy soldiers: {EnemySoldiers.Count}");
        UpdateLineState(); // Uppdatera linjens status

        // Kontrollera om linjen nu är contested
        if (PlayerSoldiers.Count > 0 && EnemySoldiers.Count > 0)
        {
            Debug.Log($"Line {name} is now contested.");
        }
    }

    private void CleanupNullReferences()
    {
        PlayerSoldiers.RemoveAll(soldier => soldier == null);
        EnemySoldiers.RemoveAll(soldier => soldier == null);
    }

    public void RemoveSoldier(GameObject soldier, bool isPlayer)
    {
        if (soldier == null) return;

        if (isPlayer)
        {
            PlayerSoldiers.Remove(soldier);
        }
        else
        {
            EnemySoldiers.Remove(soldier);
        }

        Debug.Log($"Soldier {soldier.name} removed from line {name}. Player soldiers: {PlayerSoldiers.Count}, Enemy soldiers: {EnemySoldiers.Count}");
        UpdateLineState();
    }

    public void CommandSoldiersToStorm(Line targetLine)
    {
        if (CurrentState == LineState.PlayerOwned || CurrentState == LineState.Contested)
        {
            for (int i = PlayerSoldiers.Count - 1; i >= 0; i--) // Iterate backward to handle removal
            {
                var soldierObj = PlayerSoldiers[i];

                if (soldierObj == null) // Check if the soldier has been destroyed
                {
                    PlayerSoldiers.RemoveAt(i); // Remove destroyed references
                    continue;
                }

                var soldier = soldierObj.GetComponent<PlayerSoldier>();
                if (soldier != null)
                {
                    soldier.AssignTargetLine(targetLine);
                }
            }

            Debug.Log($"Commanded soldiers on line {name} to storm {targetLine.name}.");
        }
    }

    private void UpdateLineState()
    {
        CleanupNullReferences();

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

        Debug.Log($"Line {name} updated state to {CurrentState}. Player count: {playerCount}, Enemy count: {enemyCount}");
    }

    public bool HasEmptySlots()
    {
        return Slots.Any(slotTransform =>
        {
            var slot = slotTransform.GetComponent<Slot>();
            return slot != null && slot.OccupyingSoldier == null;
        });
    }


    public bool HasFreeCapacity()
    {
        bool hasCapacity = Slots.Any(slotTransform =>
        {
            var slot = slotTransform.GetComponent<Slot>();
            return slot != null && slot.IsFree();
        });

        if (!hasCapacity)
        {
            Debug.Log($"Line {name} has no free capacity.");
        }

        return hasCapacity;
    }

    public bool IsSufficientlyFilled(bool isPlayer, int requiredCount)
    {
        return isPlayer ? PlayerSoldiers.Count >= requiredCount : EnemySoldiers.Count >= requiredCount;
    }

    public Transform GetFreeSlot()
    {
        foreach (var slotTransform in Slots)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.IsFree())
            {
                Debug.Log($"Free slot found at {slotTransform.position}");
                return slotTransform;
            }
        }

        // Only log the warning when absolutely necessary
        Debug.LogWarning($"No free slot found in line {name}!");
        return null;
    }

    private void OnDrawGizmos()
    {
        // Draw line ownership state
        Gizmos.color = CurrentState switch
        {
            LineState.PlayerOwned => Color.green,
            LineState.EnemyOwned => Color.red,
            LineState.Contested => Color.yellow,
            _ => Color.gray, // Neutral
        };

        // Draw a rectangle to represent the line
        Gizmos.DrawWireCube(transform.position, new Vector3(25, 1, 1)); // Adjust the size to match your line dimensions

        // Draw slots within the line
        foreach (var slotTransform in Slots)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null)
            {
                Gizmos.color = slot.IsFree() ? Color.green : Color.red;
                Gizmos.DrawWireSphere(slotTransform.position, 0.5f);
            }
        }
    }

    public bool CanAdvance(bool isPlayer)
    {
        if (isPlayer)
        {
            return CurrentState == LineState.PlayerOwned || CurrentState == LineState.Contested;
        }
        else
        {
            return CurrentState == LineState.EnemyOwned || CurrentState == LineState.Contested;
        }
    }

    public bool CanPushForward(bool isPlayer)
    {
        if (isPlayer)
        {
            return CurrentState == LineState.PlayerOwned || CurrentState == LineState.Contested;
        }
        else
        {
            return CurrentState == LineState.EnemyOwned || CurrentState == LineState.Contested;
        }
    }

    public bool HasMinimumSoldiers(int minimumSoldiers, bool isPlayer)
    {
        if (isPlayer)
        {
            return PlayerSoldiers.Count >= minimumSoldiers;
        }
        else
        {
            return EnemySoldiers.Count >= minimumSoldiers;
        }
    }

    public void DeregisterSlot(Slot slot)
    {
        if (slot != null && slot.OccupyingSoldier != null)
        {
            Debug.Log($"{slot.OccupyingSoldier.name} left slot at {slot.transform.position}");
            slot.OccupyingSoldier = null;
        }
    }

    public bool IsContested()
    {
        int playerCount = PlayerSoldiers.Count;
        int enemyCount = EnemySoldiers.Count;

        // Linjen är contested om båda sidor har minst en soldat
        return playerCount > 0 && enemyCount > 0;
    }

    private void Update()
    {
        UpdateLineState();
    }
}