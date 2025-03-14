using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class TurnBlockAction : GoapAction
{
    AIController _aiController;
    public override void StartAction(WorldState currentWorldState)
    {
        //base.StartAction(currentWorldState);
        _isActivated = true;
        _aiController = currentWorldState.GetOwner().GetComponent<AIController>();
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        float orientAngle = currentWorldState.GetOwner().GetComponent<WalkAnimate>().GetOrientation();
        var blockAngle = 0f;
        var targetWeaponOrientation = currentWorldState.TargetCurrentAttack;

        switch (targetWeaponOrientation)
        {
            case AttackType.UpperSlashRight:
            case AttackType.DownSlashRight:
            case AttackType.HorizontalSlashRight:
                blockAngle = -Mathf.PI * 0.25f + orientAngle;
                break;
            case AttackType.UpperSlashLeft:
            case AttackType.DownSlashLeft:
            case AttackType.HorizontalSlashLeft:
                blockAngle = Mathf.PI * 0.25f + orientAngle;
                break;
            case AttackType.Stab:
                blockAngle = 0f + orientAngle;
                break;
            case AttackType.Feint:
                return;
            case AttackType.None:
            
                return;
            default:
                break;
        }

        Vector2 blockVec = new Vector2(Mathf.Cos(blockAngle), Mathf.Sin(blockAngle));

        _aiController.AttackGuardMode(false, true);
        _aiController.AimAction_performed(blockVec, FightStyle.Shield);
        _aiController.AttackGuardMode(true, true);
        ActionCompleted();
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (!currentWorldState.IsBlockInCorrectDirection())
            Cost = 0.9f;
        else
            Cost = 0.5f;
        return true;
    }

    public override bool IsCompleted(WorldState current, WorldState activeActionDesiredState)
    {
        return (!_isActivated);
        
    }
    public override bool IsInterupted(WorldState currentWorldState)
    {
        return false;
    }
}
