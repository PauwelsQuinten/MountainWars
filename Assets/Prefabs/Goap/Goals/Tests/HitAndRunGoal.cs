using System.Collections;
using UnityEngine;

public class HitAndRunGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return _isVallid && currentWorldState._worldStateValues2[EWorldState.HasTarget] == WorldStateValue.InPosesion ; 
    }

    public override float GoalScore(CharacterMentality menatlity, WorldState currentWorldState)
    {

        switch (menatlity)
        {
            case CharacterMentality.Agresive:
                return 0.9f;

            case CharacterMentality.Defensive:
                return 0.25f;
     
            case CharacterMentality.Technical:
                return 0.75f;
     
            default:
                return base.GoalScore(menatlity, currentWorldState);
        }
    }
        


}
