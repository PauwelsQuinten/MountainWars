using UnityEngine;

public class RepositionAction : GoapAction
{
    public override void StartAction(WorldState currentWorldState)
    {
        _isActivated = true;
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        ActionCompleted();
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (currentWorldState._isPlayerToAggressive)
            Cost = 0.8f;
        else
            Cost = 0.5f;
        return true;
    }

    public override bool IsCompleted(WorldState current, WorldState activeActionDesiredState)
    {
        return _isActivated;
    }
    public override bool IsInterupted(WorldState currentWorldState)
    {
        return base.IsInterupted(currentWorldState);
    }
}
