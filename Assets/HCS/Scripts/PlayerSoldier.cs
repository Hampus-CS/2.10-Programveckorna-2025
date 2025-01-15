public class PlayerSoldier : BaseSoldier
{
    protected override bool IsValidLine(Line line) => line.CurrentState != Line.LineState.EnemyOwned;

    protected override void EngageLine()
    {
        if (CurrentTargetLine.IsContested())
        {
            // Engage logic
        }
        else if (CurrentTargetLine.CurrentState == Line.LineState.PlayerOwned)
        {
            FindNextLine();
        }
    }
}
