using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSoldier : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Line CurrentTargetLine { get; private set; }
    protected Transform currentTargetSlot;

    public bool IsPlayer { get; private set; }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();
    }

    private void Update()
    {
        if (CurrentTargetLine != null)
        {
            EngageLine();
        }
    }

    public void SetPlayerStatus(bool isPlayer)
    {
        IsPlayer = isPlayer;
        gameObject.tag = isPlayer ? "FriendlyTroop" : "HostileTroop";
    }

    public void FindNextLine()
    {
        Line[] lines = FindObjectsOfType<Line>();
        foreach (var line in lines)
        {
            if (IsValidLine(line))
            {
                CurrentTargetLine = line;
                MoveToLine(line);
                return;
            }
        }
    }

    protected abstract bool IsValidLine(Line line);

    private void MoveToLine(Line line)
    {
        currentTargetSlot = line.GetFreeSlot();
        if (currentTargetSlot != null)
        {
            agent.SetDestination(currentTargetSlot.position);
        }
        else
        {
            HoldPosition();
        }
    }

    protected abstract void EngageLine();

    protected void HoldPosition()
    {
        agent.isStopped = true;
    }

    public void SetTargetLine(Line line)
    {
        CurrentTargetLine = line;
        MoveToLine(line);
    }
}
