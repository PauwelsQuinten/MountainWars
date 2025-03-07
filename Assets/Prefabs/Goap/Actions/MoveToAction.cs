using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum ObjectTarget
{
    Player,
    Weapon,
    Shield,
    Forward
}


public class MoveToAction : GoapAction
{
    [SerializeField] private ObjectTarget _MoveTo = ObjectTarget.Player;
    [SerializeField] private float _moveSpeed = 2f;
    GameObject npc;
    private CharacterMovement npcComp;
    private List<Equipment> _foundEquipment = new List<Equipment>();
    private Equipment _foundSpecificEquipment;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);

        npc = currentWorldState.GetOwner();
        npcComp = npc.GetComponent<CharacterMovement>();

        switch(_MoveTo)
        {
            case ObjectTarget.Player:
                break;
            case ObjectTarget.Weapon:
                _foundSpecificEquipment = FindEquipmentOfType(EquipmentType.Weapon);
                currentWorldState.FoundEquipment(_foundSpecificEquipment);
                break;
            case ObjectTarget.Shield:
                _foundSpecificEquipment = FindEquipmentOfType(EquipmentType.Shield);
                currentWorldState.FoundEquipment(_foundSpecificEquipment);
                break;
            case ObjectTarget.Forward:
                break;
        }
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        Vector3 targetDir = Vector3.zero;
        Vector3 npcPos = currentWorldState.transform.position;
        Vector3 targetPos = Vector3.zero;

        switch (_MoveTo)
        {
            case ObjectTarget.Player:
                var target =  currentWorldState.GetTarget();
                if (target)
                    targetPos = target.transform.position;
               
                targetDir = targetPos - npcPos;
                targetDir.Normalize();
                break;

            case ObjectTarget.Weapon:
                if (_foundSpecificEquipment)
                    targetPos = _foundSpecificEquipment.transform.position;

                targetDir = targetPos - npcPos;
                targetDir.Normalize();
                break;
                
            case ObjectTarget.Shield:
                if (_foundSpecificEquipment)
                    targetPos = _foundSpecificEquipment.transform.position;

                targetDir = targetPos - npcPos;
                targetDir.Normalize();
                break;
                
            case ObjectTarget.Forward:
                float angleRad = npc.GetComponent<WalkAnimate>().GetOrientation();
                targetDir = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);
                break;

                
        }

        npcComp.SetInputDirection(targetDir);
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        if(base.IsCompleted(currentWorldState, activeActionDesiredState))
        {
            npcComp.SetInputDirection(Vector2.zero);
            Debug.Log("walk finished");
            return true;
        }
        return false;
        
    }

    private Equipment FindEquipmentOfType(EquipmentType type)
    {
        Equipment[] foundStuff = GameObject.FindObjectsByType<Equipment>(FindObjectsSortMode.None);
        _foundEquipment = new List<Equipment>(foundStuff);

        foreach (Equipment equipment in foundStuff)
        {
            if(equipment.GetEquipmentType() == type && equipment.GetComponent<SphereCollider>().enabled)
                return equipment;
        }
        return null;

    }


}
