using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private float attackTimer;

    private Health health; // Referens till soldatens egen hälsa

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!health.IsAlive) return; // Om soldaten är död, avbryt logik

        attackTimer += Time.deltaTime;

        // Fallback: Attackera när fiender är inom räckhåll
        if (attackTimer >= attackCooldown)
        {
            AttackNearbyEnemies();
            attackTimer = 0;
        }
    }

    private void AttackNearbyEnemies()
    {
        // Hitta alla colliders inom attackräckvidd
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            // Kontrollera om det är en fiende
            if (IsEnemy(hit.gameObject))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    return; // Attackera en fiende åt gången
                }
            }
        }
    }

    private bool IsEnemy(GameObject target)
    {
        // Kontrollera om målet tillhör motsatt fraktion
        return (health.IsPlayer && target.CompareTag("HostileTroop")) ||
               (!health.IsPlayer && target.CompareTag("FriendlyTroop"));
    }
}