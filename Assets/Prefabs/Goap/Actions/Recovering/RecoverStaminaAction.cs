using UnityEngine;

public class RecoverStaminaAction : GoapAction
{
    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
                
         //_aiController.AttackGuardMode(false, false);
         //ActionCompleted();
         //return;
       
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        switch (currentWorldState._worldStateValues2[EWorldState.Stamina])
        {
            case WorldStateValue.Full:
                Cost = 1f;
                break;
            case WorldStateValue.Mid:
                Cost = 0.8f;
                break;
            case WorldStateValue.Low:
                Cost = 0.6f;
                break;
            case WorldStateValue.Zero:
                Cost = 0.2f;
                break;
        }
        return true;
    }

    public override bool IsCompleted(WorldState current, WorldState activeActionDesiredState)
    {
        if (current.Stamina > 0.6f)
            ActionCompleted();
        return base.IsCompleted(current, activeActionDesiredState);
    }
    public override bool IsInterupted(WorldState currentWorldState)
    {
        return AboutToBeHit(currentWorldState) || FamiliarAttack(currentWorldState);

    }
}
