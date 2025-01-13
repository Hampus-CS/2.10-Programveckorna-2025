using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemySoldier : BaseSoldier
{
    protected override bool IsTargetLine(Line line)
    {
        return line.CurrentState == Line.LineState.EnemyOwned || line.CurrentState == Line.LineState.Contested || line.CurrentState == Line.LineState.Neutral;
    }

    protected override void EngageLine()
    {
        if (currentTargetLine.CurrentState == Line.LineState.Contested)
        {
            // Attackera spelarens soldater i linjen
            AttackEnemies(currentTargetLine.PlayerSoldiers);
        }
        else if (currentTargetLine.CurrentState == Line.LineState.EnemyOwned)
        {
            // Om linjen är ägd av fienden, gå vidare till nästa linje
            if (currentTargetLine.HasMinimumSoldiers(5, IsPlayer))
            {
                FindNextLine();
            }
        }
    }

    private void AttackEnemies(List<GameObject> enemies)
    {
        if (enemies.Count > 0)
        {
            GameObject target = enemies[0];
            target.GetComponent<Health>()?.TakeDamage(20f);
        }
    }
}