using UnityEngine;

public class DefendeGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return _isVallid && currentWorldState._worldStateValues2[EWorldState.HasTarget] == WorldStateValue.InPosesion ; 
    }

    public override float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {
        if (currentWorldState._worldStateValues[EWorldState.TargetSwingSpeed] > 50f
            && currentWorldState._worldStateValues2[EWorldState.TargetDistance] == WorldStateValue.OutOfRange)
            return 0.8f;
        return 0.5f;
    }

}
