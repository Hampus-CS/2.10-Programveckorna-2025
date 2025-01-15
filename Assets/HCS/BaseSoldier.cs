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

    private Transform currentTargetSlot;

    public bool IsPlayer { get; private set; }

    protected float attackRange = 2f;
    protected float attackDamage = 20f;
    protected float attackCooldown = 1.5f;
    protected float attackTimer;

    private float lineSwitchCooldown = 3f;
    private float lineSwitchTimer = 0f;

    private float reevaluationCooldown = 2f; // Time before re-evaluating a line or slot
    private float reevaluationTimer = 0f;    // Tracks time until reevaluation is allowed

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();

        // Sätt ett temporärt fast mål för att testa om agenten rör sig
        // agent.SetDestination(new Vector3(5, 0, 5));
    }

    protected virtual void Update()
    {
        if (!agent.isOnNavMesh) return;

        attackTimer += Time.deltaTime;

        if (currentTargetLine != null && attackTimer >= attackCooldown)
        {
            EngageLine(); // Calls the subclass's EngageLine, triggering AttackEnemies
            attackTimer = 0f;
        }

        if (currentTargetSlot != null)
        {
            var slotScript = currentTargetSlot.GetComponent<Slot>();
            if (slotScript != null && slotScript.OccupyingSoldier == gameObject)
            {
                return; // Stay in slot and engage
            }

            if (slotScript == null || slotScript.OccupyingSoldier != gameObject)
            {
                FindNewTargetSlotOrLine();
            }
        }
        else
        {
            FindNewTargetSlotOrLine();
        }
    }

    private void PatrolCurrentLine()
    {
        // Example: Random movement within the current line's bounds
        Vector3 patrolPoint = currentTargetLine.transform.position + Random.insideUnitSphere * 2f;
        patrolPoint.y = transform.position.y; // Maintain current height
        agent.SetDestination(patrolPoint);
    }

    private void OnSlotOccupied()
    {
        FindNewTargetSlotOrLine(); // Redirect the soldier if their target slot is taken.
    }

    protected void MoveToSlot(Transform slot)
    {
        if (slot == null) return;

        Slot slotScript = slot.GetComponent<Slot>();
        if (slotScript != null && slotScript.IsFree())
        {
            currentTargetSlot = slot;
            agent.SetDestination(slot.position);
            Debug.Log($"{gameObject.name} moving to slot at {slot.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} attempted to move to an invalid or occupied slot at {slot?.position}");
            FindNewTargetSlotOrLine(); // Redirect to another slot
        }
    }

    private IEnumerator ClaimSlotWhenArrived(Transform slot)
    {
        // Wait until the soldier is close enough to the slot
        while (Vector3.Distance(transform.position, slot.position) > 0.1f)
        {
            yield return null; // Wait for the next frame
        }

        // Once the soldier arrives, attempt to claim the slot
        Slot slotScript = slot.GetComponent<Slot>();
        if (slotScript != null && slotScript.IsFree())
        {
            slotScript.AssignSoldier(gameObject);
            transform.SetParent(slot); // Set the soldier as a child of the slot
            Debug.Log($"{gameObject.name} has claimed the slot at {slot.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not claim the slot at {slot.position} because it's no longer free.");
            FindNewTargetSlotOrLine(); // Redirect to a new target
        }
    }


    protected void FindNextLine()
    {
        if (lineSwitchTimer > 0f) return; // Prevent line switching during cooldown

        Line[] lines = Object.FindObjectsByType<Line>(FindObjectsSortMode.None);
        var sortedLines = lines.OrderBy(line => Vector3.Distance(transform.position, line.transform.position)).ToArray();

        foreach (var line in sortedLines)
        {
            if (IsTargetLine(line) && line != currentTargetLine && line.HasFreeCapacity())
            {
                currentTargetLine = line;
                MoveToLine(line);
                lineSwitchTimer = lineSwitchCooldown; // Set cooldown
                return;
            }
        }

        // If no valid line is found, log once and stay idle temporarily
        Debug.LogWarning($"{gameObject.name} could not find a valid target line! Staying idle.");
        StartCoroutine(IdleAndReevaluate());
    }

    private IEnumerator IdleAndReevaluate()
    {
        yield return new WaitForSeconds(5f); // Idle for a few seconds
        FindNextLine(); // Reattempt line finding
    }

    public void FindNewTargetSlotOrLine()
    {
        if (currentTargetLine != null)
        {
            Transform newSlot = currentTargetLine.GetFreeSlot();
            if (newSlot != null)
            {
                Debug.Log($"{gameObject.name} redirecting to a new slot at {newSlot.position}");
                MoveToSlot(newSlot);
                return;
            }
            else
            {
                Debug.Log($"{gameObject.name} could not find a free slot in the current line.");
            }
        }

        // If no free slots, look for a new line
        FindNextLine();
    }

    protected void MoveToLine(Line line)
    {
        Transform slot = line.GetFreeSlot();
        if (slot != null)
        {
            Debug.Log($"{gameObject.name} moving to slot at {slot.position}");
            agent.SetDestination(slot.position);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not find a free slot in line {line.name}, searching next line.");
            FindNextLine(); // Hitta nästa linje om ingen slot är ledig
        }
    }

    public Line FindClosestEnemyLine(Line playerLine)
    {
        Line[] lines = FindObjectsOfType<Line>();
        return lines
            .OrderBy(line => Vector3.Distance(playerLine.transform.position, line.transform.position))
            .FirstOrDefault(line => line.CurrentState == Line.LineState.EnemyOwned || line.CurrentState == Line.LineState.Contested);
    }

    public Line FindClosestPlayerLine(Line enemyLine)
    {
        Line[] lines = FindObjectsOfType<Line>();
        return lines
            .OrderBy(line => Vector3.Distance(enemyLine.transform.position, line.transform.position))
            .FirstOrDefault(line => line.CurrentState == Line.LineState.PlayerOwned);
    }

    private IEnumerator SetParentWhenArrived(Transform slot)
    {
        while (Vector3.Distance(transform.position, slot.position) > 0.1f)
        {
            yield return null; // Vänta tills soldaten når slotten
        }

        Slot slotScript = slot.GetComponent<Slot>();
        if (slotScript != null && slotScript.IsFree())
        {
            slotScript.AssignSoldier(gameObject); // Registrera soldaten i slotten
            transform.SetParent(slot); // Gör soldaten till barn av slotten
            Debug.Log($"{gameObject.name} has claimed the slot at {slot.position}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not claim the slot at {slot.position} because it's no longer free.");
            FindNextLine(); // Om slotten är upptagen, försök hitta nästa linje
        }
    }

    public void AssignTargetLine(Line targetLine)
    {
        currentTargetLine = targetLine;
        MoveToLine(targetLine);
    }

    public bool IsEnemy(GameObject target)
    {
        var targetHealth = target.GetComponent<Health>();
        if (targetHealth == null) return false;

        return (IsPlayer && !targetHealth.IsPlayer) || (!IsPlayer && targetHealth.IsPlayer);
    }

    public void SetPlayerStatus(bool isPlayer)
    {
        IsPlayer = isPlayer;
    }

    protected abstract bool IsTargetLine(Line line);
    public abstract void EngageLine();
}
