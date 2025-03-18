using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;



public class TurnBlockAction : GoapAction
{
    AIController _aiController;
    private const float _baseCost = 0.4f;
    private const float _HighCost = 0.7f;

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
            case AttackType.None:
                ActionCompleted();
            
                return;
               
            default:
                break;
        }

        Vector2 blockVec = new Vector2(Mathf.Cos(blockAngle), Mathf.Sin(blockAngle));

        _aiController.AttackGuardMode(false, true);
        _aiController.AimAction_performed(blockVec, FightStyle.Shield);
        _aiController.AttackGuardMode(true, true);
        ActionCompleted();
        //Debug.Log($"block turned");
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (!currentWorldState.IsBlockInCorrectDirection())
            Cost = _baseCost;
        else
            Cost = _HighCost;
        //Debug.Log($"block cost = {Cost}");
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
