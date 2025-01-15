using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemySoldier : BaseSoldier
{
    protected override bool IsTargetLine(Line line)
    {
        return line.CurrentState == Line.LineState.EnemyOwned || line.CurrentState == Line.LineState.Contested || line.CurrentState == Line.LineState.Neutral;
    }

    public override void EngageLine()
    {
        if (currentTargetLine == null) return;

        if (currentTargetLine.IsContested())
        {
            var enemies = currentTargetLine.PlayerSoldiers;
            AttackEnemies(enemies); // Call EnemySoldier's AttackEnemies
        }
        else if (currentTargetLine.CurrentState == Line.LineState.EnemyOwned)
        {
            FindNextLine(); // Move forward if line is owned
        }
    }


    private void AttackEnemies(List<GameObject> enemies)
    {
        if (enemies == null || enemies.Count == 0) return;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null && enemyHealth.IsAlive)
            {
                Debug.Log($"{gameObject.name} (EnemySoldier) is attacking {enemy.name}");
                enemyHealth.TakeDamage(attackDamage);
                return; // Attack one enemy per frame
            }
        }

        Debug.Log($"{gameObject.name} (EnemySoldier) found no valid enemies on the line.");
    }
}