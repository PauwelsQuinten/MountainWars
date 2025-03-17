using System.Collections.Generic;
using UnityEngine;

public class GrabEquipment : GoapAction
{
    [SerializeField] private EquipmentType equipmentToCollect;
    List<Equipment> _foundEquipment = new List<Equipment>();
    private Equipment _equipment;
    
    public override bool IsVallid(WorldState currentWorldState)
    {
        //Is called before StartAction, The neccesary equipment if in scene will be set in here.
        return FindEquipmentInArea(equipmentToCollect);
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        var equipComp = currentWorldState.GetOwner().GetComponent<HeldEquipment>();
        equipComp.PickupNewEquipment(_equipment);
        equipComp.SetLookForPickup();
        currentWorldState.UpdateHeldEquipment();//set newequipmentValues
        ActionCompleted();//In IsActionComplete() will be checked if complete or not, so this is not neccesary but only to make sure it is only called once.
    }

    

    private bool FindEquipmentInArea(EquipmentType type)
    {
        Equipment[] foundStuff = GameObject.FindObjectsByType<Equipment>(FindObjectsSortMode.None);
        _foundEquipment = new List<Equipment>(foundStuff);
        return _foundEquipment.Count > 0 && FindEquipmentOfType(equipmentToCollect); 
    }

    private bool FindEquipmentOfType(EquipmentType type)
    {
        foreach (Equipment equipment in _foundEquipment)
        {
            if (equipment.GetEquipmentType() == type && equipment.GetComponent<SphereCollider>().enabled)
            {
                _equipment = equipment;
                return true;
            }
        }
        return false;
    }

}
