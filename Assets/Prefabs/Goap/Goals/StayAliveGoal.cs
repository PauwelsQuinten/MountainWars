using UnityEngine;

public class StayAliveGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return base.IsVallid(currentWorldState);
    }

    public override float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {
        if (currentWorldState.IsBleeding)
            return 1f;
        if (currentWorldState.Stamina < 0.3f)
            return 0.8f;
        if (currentWorldState._isPlayerToAggressive)
            return 0.8f;
        if (!currentWorldState.IsBlockInCorrectDirection())
            return 0.9f;

        return 0.5f;
    }
    public override bool InteruptGoal(WorldState currentWorldState)
    {
        return base.InteruptGoal(currentWorldState);
    }
}
