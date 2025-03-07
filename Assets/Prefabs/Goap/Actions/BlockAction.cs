using System.Collections.Generic;
using UnityEngine;

public class BlockAction : GoapAction
{

    public override void UpdateAction(WorldState currentWorldState)
    {
        GameObject owner = currentWorldState.GetOwner();
        Blocking blockComp = owner.GetComponent<Blocking>();

        float orientAngle = owner.GetComponent<WalkAnimate>().GetOrientation();
        WorldStateValue targetWeaponOrientation = currentWorldState._worldStateValues2[EWorldState.TargetWeaponOrientation];
        Debug.Log($"{targetWeaponOrientation}");
        float blockAngle = 0f;
        switch(targetWeaponOrientation)
        {
            case WorldStateValue.OnRight:
                blockAngle = Mathf.PI * 0.25f + orientAngle;
                break;
            case WorldStateValue.OnLeft: 
                blockAngle = -Mathf.PI *0.25f + orientAngle;
                break;
            case WorldStateValue.OnCenter:
                blockAngle = 0f + orientAngle;
                break;
            case WorldStateValue.DontCare:
                //Debug.Log($"zero");
                //blockComp.SetInputDirection(Vector2.zero);
                return;
            default:
                break;
        }

        Vector3 blockVec = new Vector2(Mathf.Cos(blockAngle), Mathf.Sin(blockAngle));

        blockComp.SetInputDirection(blockVec);

    }

    public override bool IsVallid(WorldState currentWorldState) 
    {
        return currentWorldState.GetOwner() != null;
        //return currentWorldState._worldStateValues2[EWorldState.ShieldPosesion] == WorldStateValue.InPosesion;
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        if (base.IsCompleted(currentWorldState, activeActionDesiredState))
        {
            GameObject owner = currentWorldState.GetOwner();
            Blocking blockComp = owner.GetComponent<Blocking>();
            blockComp.SetInputDirection(Vector2.zero);
            return true;
        }
        return false;

    }

}
