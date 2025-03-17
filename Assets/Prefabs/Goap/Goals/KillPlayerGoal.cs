using UnityEngine;

public class KillPlayerGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return _isVallid && currentWorldState._worldStateValues2[EWorldState.HasTarget] == WorldStateValue.InPosesion;
    }
    
    public override float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {
        return 0.7f;
    }

    public override bool InteruptGoal(WorldState currentWorldState)
    {
        //return currentWorldState.IsBleeding || currentWorldState.Stamina < 0.3f || currentWorldState._isPlayerToAggressive || !currentWorldState.IsBlockInCorrectDirection();
        if ( currentWorldState.IsBleeding || currentWorldState.Stamina < 0.3f || currentWorldState._isPlayerToAggressive || !currentWorldState.IsBlockInCorrectDirection())
            return true;
        else
            return false;
    }
}
