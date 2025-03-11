using UnityEngine;

public class RestGoal : GoapGoal
{
    [SerializeField] bool _onDistance = false;
    public override bool IsVallid(WorldState currentWorldState)
    {
        return _isVallid;
    }

    public override float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {
        float bonus = currentWorldState._worldStateValues2[EWorldState.Stamina] == WorldStateValue.LowOnStamina ? 0.6f : 0f;
        switch (menatlity)
        {
            case CharacterMentality.Agresive:
                return 0.35f + bonus;

            case CharacterMentality.Defensive:
                return 0.5f + bonus;

            case CharacterMentality.Technical:
                return 0.4f + bonus + (_onDistance? 0.15f : 0f);
                
            default:
                return base.GoalScore(menatlity, currentWorldState);
        }
    }

}
