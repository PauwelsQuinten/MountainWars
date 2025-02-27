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
    private bool _readyToPickup = false;
    private Equipment _foundEquipment;

    private void Start()
    {
        _fullEquipment.Add(EquipmentType.Weapon, null);
        if (_weaponPrefab != null)
        {
            var weapon = Instantiate(_weaponPrefab, gameObject.transform);
            weapon.GetComponent<SphereCollider>().enabled = false;
            _fullEquipment[EquipmentType.Weapon] = weapon;
        }
        
        _fullEquipment.Add(EquipmentType.Shield, null);
        if (_shieldPrefab != null)
        {
            var shield = Instantiate(_shieldPrefab, gameObject.transform);
            shield.GetComponent<SphereCollider>().enabled = false;
            _fullEquipment[EquipmentType.Shield] = shield;
        }

        _fullEquipment.Add(EquipmentType.Armor, null);
        if (_armorPrefab != null)
        {
            var armor = Instantiate(_armorPrefab, gameObject.transform);
            armor.GetComponent<SphereCollider>().enabled = false;
            _fullEquipment[EquipmentType.Armor] = armor;
        }

       
        
    }

    public bool EquipmentEnduresHit(EquipmentType equipmentType, float damage)
    {
        if (_fullEquipment[equipmentType] != null &&  _fullEquipment[equipmentType].DecreaseDurability(damage))
        {
            Destroy(_fullEquipment[equipmentType].gameObject);
            _fullEquipment[equipmentType] = null;
            return false;
        }
        return true;
    }

    public bool HoldsEquipment(EquipmentType type)
    {
        return _fullEquipment[type] != null;
    }

    public GameObject GetEquipment(EquipmentType type)
    {
        return _fullEquipment[type].GetEquipment();
    }

    public bool HoldSwordAndShield()
    {
        return _fullEquipment[EquipmentType.Weapon] && _fullEquipment[EquipmentType.Shield];
    }

    public void PickupNewEquipment(Equipment equipment)
    {
        _foundEquipment = equipment;
    }

    public void SetLookForPickup()
    {
        if (_foundEquipment == null)
            return;

        var equipment = _foundEquipment;
        if (_fullEquipment[_foundEquipment.GetEquipmentType()] != null)
        {
            _foundEquipment = _fullEquipment[_foundEquipment.GetEquipmentType()];
            _fullEquipment[_foundEquipment.GetEquipmentType()].transform.parent = null;
        }
        else
            _foundEquipment = null;
        
        equipment.transform.parent = transform;
        equipment.transform.localPosition = Vector3.zero;
        _fullEquipment[equipment.GetEquipmentType()] = equipment;
        GetComponent<Blocking>().NewShield();


    }

}
