using System.Collections.Generic;
using UnityEngine;

public class GrabShield : GoapAction
{
    List<Equipment> _foundEquipment = new List<Equipment>();
    
    public override bool IsVallid(WorldState currentWorldState)
    {
        return FindEquipmentInArea();
        //return currentWorldState._worldStateValues2[EWorldState.ShieldPosesion] == WorldStateValue.InPosesion;
    }


    private bool FindEquipmentInArea()
    {
        Equipment[] foundStuff = GameObject.FindObjectsByType<Equipment>(FindObjectsSortMode.None);
        _foundEquipment = new List<Equipment>(foundStuff);
        return foundStuff.Length > 0; 
    }

}
