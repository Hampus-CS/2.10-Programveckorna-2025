public class EnemySoldier : BaseSoldier
{
    protected override bool IsValidLine(Line line) => line.CurrentState != Line.LineState.PlayerOwned;

    protected override void EngageLine()
    {
        if (CurrentTargetLine.IsContested())
        {
            // Engage logic
        }
        else if (CurrentTargetLine.CurrentState == Line.LineState.EnemyOwned)
        {
            FindNextLine();
        }
    }
}
