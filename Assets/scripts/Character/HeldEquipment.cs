using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HeldEquipment : MonoBehaviour
{
    [SerializeField] private Equipment _weaponPrefab;
    [SerializeField] private Equipment _shieldPrefab;
    [SerializeField] private Equipment _armorPrefab;
    private Dictionary<EquipmentType, Equipment> _fullEquipment = new Dictionary<EquipmentType, Equipment>();


    private void Start()
    {
        _fullEquipment.Add(EquipmentType.Weapon, null);
        if (_weaponPrefab != null)
        {
            var weapon = Instantiate(_weaponPrefab, gameObject.transform);
            _fullEquipment[EquipmentType.Weapon] = weapon;
        }
        
        _fullEquipment.Add(EquipmentType.Shield, null);
        if (_shieldPrefab != null)
        {
            var shield = Instantiate(_shieldPrefab, gameObject.transform);
            _fullEquipment[EquipmentType.Shield] = shield;
        }

        _fullEquipment.Add(EquipmentType.Armor, null);
        if (_armorPrefab != null)
        {
            var armor = Instantiate(_armorPrefab, gameObject.transform);
            _fullEquipment[EquipmentType.Armor] = armor;
        }

       
        
    }

    public bool EquipmentEnduresHit(EquipmentType equipmentType, float damage)
    {
        if (_fullEquipment[equipmentType] != null &&  _fullEquipment[equipmentType].DecreaseDurability(damage))
        {
            _fullEquipment[equipmentType] = null;
            return false;
        }
        return true;
    }

    public void PickupEquipment(Equipment equipment)
    {
        _fullEquipment[equipment.GetEquipmentType()] = equipment;
    }

    public bool HoldsEquipment(EquipmentType type)
    {
        return _fullEquipment[type] != null;
    }

    public GameObject GetEquipment(EquipmentType type)
    {
        return _fullEquipment[type].GetEquipment();
    }

}
