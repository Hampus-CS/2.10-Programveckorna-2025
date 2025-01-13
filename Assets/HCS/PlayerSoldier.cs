using System.Collections.Generic;
using UnityEngine;

public class PlayerSoldier : BaseSoldier
{
    protected override bool IsTargetLine(Line line)
    {
        return line.CurrentState == Line.LineState.PlayerOwned || line.CurrentState == Line.LineState.Contested;
    }

    protected override void EngageLine()
    {
        if (currentTargetLine.CurrentState == Line.LineState.Contested)
        {
            // Attackera fiendens soldater i linjen
            AttackEnemies(currentTargetLine.EnemySoldiers);
        }
        else if (currentTargetLine.CurrentState == Line.LineState.PlayerOwned)
        {
            // Om linjen är ägd av spelaren, gå vidare till nästa linje
            FindNextLine();
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