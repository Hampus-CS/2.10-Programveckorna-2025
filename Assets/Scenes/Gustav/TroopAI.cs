using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public struct PriorityAction
{
    public int priority; // Higher numbers mean higher priority
    public System.Action action;

    public PriorityAction(int priority, System.Action action)
    {
        this.priority = priority;
        this.action = action;
    }
}

public class TroopAI : MonoBehaviour
{
    private TroopNavigation troopNavigation;
    private TroopPersonalityScript personalityScript;
    private RangeColliderScript rangeColliderScript;
    private Weapon weapon;

    private bool enemyInRange = false;

    void Start()
    {
        weapon = GetComponent<Weapon>();
        rangeColliderScript = FindAnyObjectByType<RangeColliderScript>();
        troopNavigation = GetComponent<TroopNavigation>();
        personalityScript = GetComponent<TroopPersonalityScript>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !troopNavigation.holdPosition)
        {
            troopNavigation.holdPosition = true;
            Debug.Log("Hold position = flase");
        }
        else if (Input.GetKeyUp(KeyCode.Space) && troopNavigation.holdPosition)
        {
            troopNavigation.holdPosition = false;
            Debug.Log("hold position = true");
        }

        if (rangeColliderScript.triggers.Count > 0)
        {
            enemyInRange = true;
        }
        else
        {
            enemyInRange = false;
        }

        List<PriorityAction> actions = GetAvailableActions();
        ExecuteHighestPriorityAction(actions);
    }

    private List<PriorityAction> GetAvailableActions()
    {
        List<PriorityAction> actions = new List<PriorityAction>();

        if (personalityScript.stress > 5)
        {
            actions.Add(new PriorityAction(3, SeekCover));
        }

        if (enemyInRange)
        {
            actions.Add(new PriorityAction(2, Shoot));
        }

        if (!troopNavigation.holdPosition)
        {
            actions.Add(new PriorityAction(1, MoveForward));
        }

        if (troopNavigation.holdPosition)
        {
            actions.Add(new PriorityAction(4, HoldPosition));
        }

        return actions;
    }

    private void ExecuteHighestPriorityAction(List<PriorityAction> actions)
    {
        if (actions.Count == 0) return;

        actions.Sort((a, b) => b.priority.CompareTo(a.priority));

        actions[0].action.Invoke();
    }

    private void SeekCover()
    {
        if (personalityScript.stress < 5 && !troopNavigation.isAtCover)
        {
            troopNavigation.RunToNearestCover();
        }
        else
        {
            troopNavigation.RunToBackToCover();
        }
    }

    private void Shoot()
    {
        troopNavigation.RotateToEnemy(rangeColliderScript.triggers[0]);
        weapon.Shoot();
    }

    private void MoveForward()
    {
        troopNavigation.MoveForwards();
    }

    private void HoldPosition()
    {
        troopNavigation.HoldPosition();
    }
}