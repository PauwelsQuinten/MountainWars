using UnityEngine;

public class DefendeGoal : GoapGoal
{
    public override bool IsVallid(WorldState currentWorldState)
    {
        return currentWorldState._worldStateValues2[EWorldState.HasTarget] == WorldStateValue.InPosesion ; 
    }
}
