using System.Collections.Generic;
using UnityEngine;

public class IdleAction : GoapAction
{
    
    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        base.UpdateAction(currentWorldState);
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        return base.IsCompleted(currentWorldState, activeActionDesiredState);
    }

    public override bool IsInterupted(WorldState currentWorldState)
    {
        return (currentWorldState._worldStateValues[EWorldState.TargetSwingSpeed] > 50f
            && currentWorldState._worldStateValues2[EWorldState.TargetDistance] == WorldStateValue.OutOfRange);
       
    }

}


