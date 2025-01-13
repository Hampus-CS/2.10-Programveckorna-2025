using System.Collections.Generic;
using UnityEngine;

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
        UpdateLineState(); // Uppdatera endast vid ändringar
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

    public Transform GetFreeSlot()
    {
        foreach (var slot in Slots)
        {
            if (slot.childCount == 0) // Check if the slot is unoccupied
            {
                return slot;
            }
        }
        return null; // No free slot available
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

    private void Update()
    {
        UpdateLineState();
    }
}