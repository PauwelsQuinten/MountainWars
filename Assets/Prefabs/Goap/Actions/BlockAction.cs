using System.Collections.Generic;
using UnityEngine;

public class BlockAction : GoapAction
{

    public override void UpdateAction(WorldState currentWorldState)
    {
        var owner = currentWorldState.GetOwner();
        Blocking blockComp = owner.GetComponent<Blocking>();
        float targetWeaponOrientation = currentWorldState._worldStateValues[EWorldState.TargetWeaponOrientation];
        Vector3 incomingVec = new Vector3(Mathf.Cos(targetWeaponOrientation), Mathf.Sin(targetWeaponOrientation), 0f);
        float reflectionNrm = owner.GetComponent<WalkAnimate>().Orientation;
        Vector3 reflectionVec = new Vector3(Mathf.Cos(reflectionNrm), Mathf.Sin(reflectionNrm), 0f);
        Vector3 blockAngle = Vector3.Reflect(incomingVec, reflectionVec);
        blockComp.SetInputDirection(blockAngle);
    }

    public override bool IsVallid(WorldState currentWorldState) 
    {
        return currentWorldState.GetOwner() != null;
        //return currentWorldState._worldStateValues2[EWorldState.ShieldPosesion] == WorldStateValue.InPosesion;
    }

    public override bool IsCompleted(WorldState currentWorldState, Dictionary<EWorldState, WorldStateValue> comparedWorldStat)
    {
        if (SatisfyingWorldState._worldStateValues.Count !=0)
        {
            foreach (KeyValuePair<EWorldState, float> updatingState in SatisfyingWorldState._worldStateValues)
            {
                if (Mathf.Abs(updatingState.Value - currentWorldState._worldStateValues[updatingState.Key]) >= 0.1f)
                    return false;
            }
        }
        if (SatisfyingWorldState._worldStateValues2.Count != 0)
        {
            foreach (KeyValuePair<EWorldState, WorldStateValue> updatingState in SatisfyingWorldState._worldStateValues2)
            {
                if (updatingState.Value - currentWorldState._worldStateValues2[updatingState.Key] != 0)
                    return false;
            }
        }

        Debug.Log("finish block");
        return true;
    }

}
