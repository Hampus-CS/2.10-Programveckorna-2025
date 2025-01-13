using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private float attackTimer;

    private Health health; // Referens till soldatens egen h�lsa

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!health.IsAlive) return; // Om soldaten �r d�d, avbryt logik

        attackTimer += Time.deltaTime;

        // Fallback: Attackera n�r fiender �r inom r�ckh�ll
        if (attackTimer >= attackCooldown)
        {
            AttackNearbyEnemies();
            attackTimer = 0;
        }
    }

    private void AttackNearbyEnemies()
    {
        // Hitta alla colliders inom attackr�ckvidd
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hit in hits)
        {
            // Kontrollera om det �r en fiende
            if (IsEnemy(hit.gameObject))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    return; // Attackera en fiende �t g�ngen
                }
            }
        }
    }

    private bool IsEnemy(GameObject target)
    {
        // Kontrollera om m�let tillh�r motsatt fraktion
        return (health.IsPlayer && target.CompareTag("HostileTroop")) ||
               (!health.IsPlayer && target.CompareTag("FriendlyTroop"));
    }
}