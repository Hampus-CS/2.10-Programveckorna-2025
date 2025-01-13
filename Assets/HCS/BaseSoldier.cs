using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

public abstract class BaseSoldier : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Line currentTargetLine;

    public Line CurrentTargetLine => currentTargetLine;

    public bool IsPlayer { get; private set; }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();

        // Sätt ett temporärt fast mål för att testa om agenten rör sig
        //agent.SetDestination(new Vector3(5, 0, 5));
    }

    protected virtual void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not on a NavMesh!");
            return;
        }

        // Temporärt test för manuell rörelse
        if (Input.GetKeyDown(KeyCode.T)) // Tryck 'T' för att aktivera manuell rörelse
        {
            Debug.Log("Manual movement triggered!");

            // Hämta alla soldater i scenen och flytta dem manuellt
            var soldiers = FindObjectsOfType<BaseSoldier>();
            foreach (var soldier in soldiers)
            {
                soldier.MoveForwardTest();
            }
        }

        if (currentTargetLine != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            EngageLine();
        }
    }

    // Temporärt test för manuell rörelse
    public void MoveForwardTest()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not on a NavMesh!");
            return;
        }

        // Flytta manuellt framåt
        Vector3 forwardPosition = transform.position + transform.forward * 5f;
        agent.SetDestination(forwardPosition);

        Debug.Log($"{gameObject.name} manually moving to {forwardPosition}");
    }

    protected void FindNextLine()
    {
        Line[] lines = Object.FindObjectsByType<Line>(FindObjectsSortMode.None);

        // Sortera linjer baserat på avstånd
        var sortedLines = lines.OrderBy(line => Vector3.Distance(transform.position, line.transform.position)).ToArray();

        foreach (var line in sortedLines)
        {
            Debug.Log($"{gameObject.name} checking line: {line.name}, State: {line.CurrentState}");

            if (IsTargetLine(line) && line != currentTargetLine && line.HasFreeCapacity())
            {
                currentTargetLine = line;
                Debug.Log($"{gameObject.name} moving to line: {line.name}");
                MoveToLine(line);
                return;
            }
        }

        Debug.LogWarning($"{gameObject.name} could not find a valid target line!");
    }

    protected void MoveToLine(Line line)
    {
        if (!line.HasFreeCapacity())
        {
            Debug.LogWarning($"{gameObject.name} cannot move to line {line.name} because it's full.");
            return;
        }

        Transform slot = line.GetFreeSlot();
        if (slot != null)
        {
            Debug.Log($"{gameObject.name} moving to slot at {slot.position}");
            agent.SetDestination(slot.position);

            line.RegisterSoldier(gameObject, IsPlayer);

            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.CurrentLine = line;
                health.IsPlayer = IsPlayer;
            }

            StartCoroutine(SetParentWhenArrived(slot));
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not find a free slot in line {line.name}");
        }
    }

    private IEnumerator SetParentWhenArrived(Transform slot)
    {
        while (Vector3.Distance(transform.position, slot.position) > 0.1f)
        {
            yield return null; // Vänta tills soldaten når slotten
        }

        // Kontrollera om slotten fortfarande är ledig
        Slot slotScript = slot.GetComponent<Slot>();
        if (slotScript != null && slotScript.IsFree())
        {
            slotScript.AssignSoldier(gameObject); // Registrera soldaten i slotten
            transform.SetParent(slot); // Sätt som barn till slotten
            Debug.Log($"{gameObject.name} has claimed the slot at {slot.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not claim the slot at {slot.position} because it's no longer free.");
        }
    }

    public void SetPlayerStatus(bool isPlayer)
    {
        IsPlayer = isPlayer;
    }

    protected abstract bool IsTargetLine(Line line);
    protected abstract void EngageLine();
}
