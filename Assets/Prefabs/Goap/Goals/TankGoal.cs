using UnityEngine;

public class TankGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return _isVallid && currentWorldState._worldStateValues2[EWorldState.HasTarget] == WorldStateValue.InPosesion ; 
    }

    public override float GoalScore(CharacterMentality mentality, WorldState currentWorldState)
    {
        switch (mentality)
        {
            case CharacterMentality.Agresive:
                return 0.2f;
            case CharacterMentality.Defensive:
                return 0.8f;
            case CharacterMentality.Technical:
                return 0.2f;
        }
        return 0.2f;
    }
}