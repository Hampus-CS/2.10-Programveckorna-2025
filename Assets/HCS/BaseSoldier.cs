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

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();

        // Sätt ett temporärt fast mål för att testa om agenten rör sig
        // agent.SetDestination(new Vector3(5, 0, 5));
    }

    protected virtual void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not on a NavMesh!");
            return;
        }

        attackTimer += Time.deltaTime;

        // Attack enemies if cooldown is reached
        if (attackTimer >= attackCooldown)
        {
            EngageLine();
            attackTimer = 0;
        }

        if (currentTargetSlot != null)
        {
            Slot slotScript = currentTargetSlot.GetComponent<Slot>();
            if (slotScript != null && slotScript.OccupyingSoldier != gameObject)
            {
                Debug.LogWarning($"{gameObject.name} lost its slot or was pushed out. Redirecting.");
                FindNewTargetSlotOrLine();
            }
        }

        // Ensure soldiers stay in their assigned slots unless conditions for moving are met
        if (currentTargetSlot != null)
        {
            Slot slotScript = currentTargetSlot.GetComponent<Slot>();
            if (slotScript != null && slotScript.OccupyingSoldier == gameObject)
            {
                // Stay at slot if line is not ready for progression
                if (!currentTargetLine.IsSufficientlyFilled(IsPlayer, 5)) // Example: 5 soldiers required
                {
                    agent.SetDestination(currentTargetSlot.transform.position);
                    return;
                }
            }
        }

        // Continue to next line if all conditions are met
        if (currentTargetLine.IsSufficientlyFilled(IsPlayer, 5))
        {
            FindNextLine();
        }
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

    public void FindNewTargetSlotOrLine()
    {
        // Försök hitta en ledig slot på samma linje
        if (currentTargetLine != null)
        {
            Transform newSlot = currentTargetLine.GetFreeSlot();
            if (newSlot != null)
            {
                Debug.Log($"{gameObject.name} redirecting to a new slot at {newSlot.position}");
                MoveToSlot(newSlot);
                return;
            }
        }

        // Om ingen ledig slot finns, gå till nästa linje
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

    private void MoveToSlot(Transform slot)
    {
        currentTargetSlot = slot; // Uppdatera målslotten
        agent.SetDestination(slot.position); // Ställ in destinationen
        Debug.Log($"{gameObject.name} is now moving to new slot at {slot.position}");
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

    protected bool IsEnemy(GameObject target)
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
    protected abstract void EngageLine();
}
