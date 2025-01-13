using UnityEngine;
using UnityEngine.AI;

public abstract class BaseSoldier : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Line currentTargetLine;

    public bool IsPlayer { get; private set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();
    }

    protected virtual void Update()
    {
        if (currentTargetLine != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            EngageLine();
        }
    }

    protected void FindNextLine()
    {
        Line[] lines = Object.FindObjectsByType<Line>(FindObjectsSortMode.None);

        foreach (var line in lines)
        {
            if (IsTargetLine(line) && line != currentTargetLine)
            {
                currentTargetLine = line;
                MoveToLine(line);
                break;
            }
        }
    }

    protected void MoveToLine(Line line)
    {
        Transform slot = line.GetFreeSlot();
        if (slot != null)
        {
            // Sätt destinationen för soldaten
            agent.SetDestination(slot.position);

            // Registrera soldaten i linjen
            line.RegisterSoldier(gameObject, IsPlayer);

            // Uppdatera Health-komponenten
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.CurrentLine = line;
                health.IsPlayer = IsPlayer;
            }
        }
    }

    public void SetPlayerStatus(bool isPlayer)
    {
        IsPlayer = isPlayer;
    }

    protected abstract bool IsTargetLine(Line line);
    protected abstract void EngageLine();
}
