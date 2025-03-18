using UnityEngine;

public class MakeOpeningAction : GoapAction
{
    private AIController _aiController;
    private WalkAnimate _animator;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        _aiController = currentWorldState.GetOwner().GetComponent<AIController>();
        _animator = currentWorldState.GetOwner().GetComponent<WalkAnimate>();
    }
    public override void UpdateAction(WorldState currentWorldState)
    {
        if (_aiController.IsHoldingGuard())
        {
            ActionCompleted();
            return;
        }

        //Set shield up in center
        float orientation = _animator.GetOrientation();
        Vector2 direction = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));

        _aiController.AimAction_performed(direction, FightStyle.Shield);
        _aiController.AttackGuardMode(true, true);

        ActionCompleted();
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        return base.IsVallid(currentWorldState);
    }

    public override bool IsCompleted(WorldState current, WorldState activeActionDesiredState)
    {
        return base.IsCompleted(current, activeActionDesiredState);
    }

    public override bool IsInterupted(WorldState currentWorldState)
    {
        return base.IsInterupted(currentWorldState);
    }

}
