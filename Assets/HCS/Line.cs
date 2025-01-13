using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Line : MonoBehaviour
{
    public int TotalSlots = 10;
    public Transform[] Slots;
    public List<GameObject> PlayerSoldiers = new List<GameObject>();
    public List<GameObject> EnemySoldiers = new List<GameObject>();

    public enum LineState { PlayerOwned, EnemyOwned, Contested, Neutral }
    public LineState CurrentState;

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

    public void RemoveSoldier(GameObject soldier, bool isPlayer)
    {
        if (isPlayer)
        {
            PlayerSoldiers.Remove(soldier);
        }
        else
        {
            EnemySoldiers.Remove(soldier);
        }
        UpdateLineState(); // Uppdatera endast vid ändringar
    }

    private void UpdateLineState()
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
    }

    public bool HasFreeCapacity()
    {
        int occupiedSlots = PlayerSoldiers.Count + EnemySoldiers.Count;
        return occupiedSlots < Slots.Length;
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
        Debug.LogWarning("No free slot found!");
        return null;
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

    private void Update()
    {
        UpdateLineState();
    }
}