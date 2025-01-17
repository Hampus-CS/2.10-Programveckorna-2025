using System.Collections.Generic;
using UnityEngine;


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

public class TroopAi : MonoBehaviour
{
    private BaseSoldier BaseSoldier;
    private TroopPersonalityScript personalityScript;
    private RangeColliderScript rangeColliderScript;
    private Weapon weapon;

    private bool enemyInRange = false;

    private bool isFriendly;

    void Start()
    {
        rangeColliderScript = GetComponentInChildren<RangeColliderScript>();
        BaseSoldier = GetComponent<BaseSoldier>();
        personalityScript = GetComponent<TroopPersonalityScript>();
        weapon = GetComponent<Weapon>();

        isFriendly = personalityScript.isFriendly;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !BaseSoldier.holdPosition)
        {
            BaseSoldier.holdPosition = true;
            Debug.Log("Hold position = flase");
        }
        else if (Input.GetKeyUp(KeyCode.Space) && BaseSoldier.holdPosition)
        {
            BaseSoldier.holdPosition = false;
            Debug.Log("hold position = true");
        }

        if (rangeColliderScript.triggers.Count > 0)
        {
            enemyInRange = true;
            RotateTowardsEnemy();
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

        actions.Add(new PriorityAction(1, MoveForward));

        if (BaseSoldier.holdPosition)
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
        if (personalityScript.stress < 5 && !BaseSoldier.isAtCover)
        {
            BaseSoldier.RunToNearestCover();
        }
        else
        {
            BaseSoldier.RunToBackToCover();
        }
    }

    private void Shoot()
    {
        weapon.Shoot(rangeColliderScript.triggers[0].transform.position);
    }

    private void MoveForward()
    {
        BaseSoldier.MoveForwards();
    }

    private void HoldPosition()
    {
        BaseSoldier.HoldPosition();
    }

    private void RotateTowardsEnemy()
    {
        BaseSoldier.RotateToEnemy(rangeColliderScript.triggers[0].transform.position);
    }
}