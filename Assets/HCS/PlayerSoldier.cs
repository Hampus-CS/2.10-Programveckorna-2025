using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerSoldier : BaseSoldier
{
    protected override bool IsTargetLine(Line line)
    {
        return line.CurrentState == Line.LineState.PlayerOwned || line.CurrentState == Line.LineState.Contested || line.CurrentState == Line.LineState.Neutral;
    }

    protected override void EngageLine()
    {
        if (currentTargetLine == null) return;

        // Ensure soldier doesn't leave slot prematurely
        if (!currentTargetLine.IsSufficientlyFilled(IsPlayer, 5)) // Example: 5 player soldiers required
        {
            Debug.Log($"{gameObject.name} (PlayerSoldier) is holding position on line {currentTargetLine.name}");
            return;
        }

        // Attack enemy soldiers on the line
        List<GameObject> enemies = currentTargetLine.EnemySoldiers;
        if (enemies.Count > 0)
        {
            Debug.Log($"{gameObject.name} (PlayerSoldier) is attacking enemy soldiers on line {currentTargetLine.name}");
            AttackEnemies(enemies);
        }
        else
        {
            Debug.Log($"{gameObject.name} (PlayerSoldier) found no enemies to attack. Advancing to the next line.");
            FindNextLine();
        }
    }

    private void AttackEnemies(List<GameObject> enemies)
    {
        // Försök attackera fiender på linjen först
        if (enemies != null && enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    Debug.Log($"{gameObject.name} (PlayerSoldier) is attacking {enemy.name}");
                    enemyHealth.TakeDamage(attackDamage);
                    return; // Attackera en fiende åt gången
                }
            }
        }

        // Om inga fiender på linjen, sök efter fiender i närheten
        Debug.Log($"{gameObject.name} (PlayerSoldier) found no valid enemies on the line. Searching nearby.");
        Collider[] hits = Physics.OverlapSphere(transform.position, base.attackRange);

        foreach (var hit in hits)
        {
            if (IsEnemy(hit.gameObject)) // Kontrollera om det är en fiende
            {
                Health enemyHealth = hit.GetComponent<Health>();
                if (enemyHealth != null && enemyHealth.IsAlive)
                {
                    Debug.Log($"{gameObject.name} (PlayerSoldier) is attacking nearby {hit.gameObject.name}");
                    enemyHealth.TakeDamage(base.attackDamage);
                    return;
                }
            }
        }

        Debug.Log($"{gameObject.name} (PlayerSoldier) found no valid enemies to attack.");
    }
}