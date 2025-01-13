using UnityEngine;
using UnityEngine.AI;

public class EnemySoldier : MonoBehaviour
{
    private NavMeshAgent agent;
    private Line currentTargetLine;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindNextLine();
    }

    private void Update()
    {
        // Check if soldier reached the line or needs a new target
        if (currentTargetLine != null && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            EngageLine();
        }
    }

    private void FindNextLine()
    {
        Debug.Log($"{gameObject.name} is searching for the next line.");

        Line[] lines = Object.FindObjectsByType<Line>(FindObjectsSortMode.None);

        foreach (var line in lines)
        {
            Debug.Log($"{gameObject.name} checking line: {line.name}, State: {line.CurrentState}");

            if (line.CurrentState == Line.LineState.PlayerOwned || line.CurrentState == Line.LineState.Contested)
            {
                currentTargetLine = line;
                Debug.Log($"{gameObject.name} selected target line: {line.name}");
                MoveToLine(line);
                return;
            }
        }

        Debug.Log($"{gameObject.name} could not find a valid target line.");
    }

    private void MoveToLine(Line line)
    {
        Transform slot = line.GetFreeSlot();
        if (slot != null)
        {
            // Assign the slot as the target
            agent.SetDestination(slot.position);
            currentTargetLine = line;

            // Register the soldier to the line
            line.RegisterSoldier(gameObject, false); // false = Enemy soldier
        }
    }

    private void EngageLine()
    {
        if (currentTargetLine != null && currentTargetLine.CurrentState == Line.LineState.Contested)
        {
            // Logic to fight player soldiers
            AttackEnemies();
        }
        else
        {
            // Push forward if line is fully controlled by the enemy
            FindNextLine();
        }
    }

    private void AttackEnemies()
    {
        // Example: Deal damage to a player soldier in the line
        if (currentTargetLine.PlayerSoldiers.Count > 0)
        {
            GameObject target = currentTargetLine.PlayerSoldiers[0];
            Destroy(target); // Replace with actual combat logic
            currentTargetLine.PlayerSoldiers.Remove(target);
        }
    }
}
