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
            var enemies = IsPlayer ? currentTargetLine.EnemySoldiers : currentTargetLine.PlayerSoldiers;
            StartCoroutine(AttackEnemies(enemies)); // Call shared method from BaseSoldier
        }
        else if (currentTargetLine.CurrentState == Line.LineState.EnemyOwned)
        {
            FindNextLine(); // Move forward if the line is owned
        }
    }

}