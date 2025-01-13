using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private float attackTimer;

    private Transform targetLine; // Target position to move toward

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        FindTargetLine(); // Find the initial target line
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // Move to the assigned slot
        if (targetLine != null && agent != null)
        {
            agent.SetDestination(targetLine.position);

            // Check if soldier has reached the slot
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Attack nearby enemies when at the destination
                if (attackTimer >= attackCooldown)
                {
                    AttackNearbyEnemies();
                    attackTimer = 0;
                }
            }
        }

        // Fallback: Attack nearby enemies even if not at the target slot
        if (attackTimer >= attackCooldown)
        {
            AttackNearbyEnemies();
            attackTimer = 0;
        }
    }

    private void FindTargetLine()
    {
        // Locate the nearest line based on ownership
        Line[] lines = Object.FindObjectsByType<Line>(FindObjectsSortMode.None);
        foreach (var line in lines)
        {
            if (IsEnemyLine(line))
            {
                targetLine = line.transform; // Set the target position to the line
                break;
            }
        }
    }

    private bool IsEnemyLine(Line line)
    {
        // Define logic for identifying enemy-controlled or contested lines
        if (gameObject.CompareTag("FriendlyTroop") &&
            (line.CurrentState == Line.LineState.Contested || line.CurrentState == Line.LineState.EnemyOwned))
        {
            return true;
        }
        else if (gameObject.CompareTag("HostileTroop") &&
                 (line.CurrentState == Line.LineState.Contested || line.CurrentState == Line.LineState.PlayerOwned))
        {
            return true;
        }
        return false;
    }

    private void AttackNearbyEnemies()
    {
        // Find all colliders in attack range
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            // Check if it's an enemy
            if (IsEnemy(hit.gameObject))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    return; // Attack one enemy at a time
                }
            }
        }
    }

    private bool IsEnemy(GameObject target)
    {
        // Check if the target belongs to the opposing faction
        if (gameObject.CompareTag("FriendlyTroop") && target.CompareTag("HostileTroop"))
        {
            return true;
        }
        else if (gameObject.CompareTag("HostileTroop") && target.CompareTag("FriendlyTroop"))
        {
            return true;
        }
        return false;
    }

}
