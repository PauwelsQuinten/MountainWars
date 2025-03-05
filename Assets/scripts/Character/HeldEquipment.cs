using System.Collections.Generic;
using UnityEngine;

public class HeldEquipment : MonoBehaviour
{
    [SerializeField] private Equipment _weaponPrefab;
    [SerializeField] private Equipment _shieldPrefab;
    [SerializeField] private Equipment _armorPrefab;
    [SerializeField] private Equipment _fistPrefab;
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

         _fullEquipment.Add(EquipmentType.Fist, null);
        if (_fistPrefab != null)
        {
            var fist = Instantiate(_fistPrefab, gameObject.transform);
            fist.transform.localScale = Vector3.zero;
            _fullEquipment[EquipmentType.Fist] = fist;
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
        if(_fullEquipment[type])
            return _fullEquipment[type].GetEquipment();
        else 
            return null;
    }

    public bool HoldSwordAndShield()
    {
        return _fullEquipment[EquipmentType.Weapon] && _fullEquipment[EquipmentType.Shield];
    }

    public void PickupNewEquipment(Equipment equipment)
    {
        _foundEquipment = equipment;
    }

    public void SetLookForPickup(bool drop = false)
    {
        if (_foundEquipment == null && !drop)
            return;

        var equipment = _foundEquipment;
        EquipmentType targetedtype = drop? EquipmentType.Weapon : _foundEquipment.GetEquipmentType();
        if (_fullEquipment[targetedtype] != null)
        {
            _foundEquipment = _fullEquipment[targetedtype];
            _fullEquipment[targetedtype].transform.parent = null;
            _fullEquipment[targetedtype].GetComponent<SphereCollider>().enabled = true;
            _fullEquipment[targetedtype].GetComponent<SphereCollider>().isTrigger = true;

        }
        else
            _foundEquipment = null;
        
        if (equipment == null)
        {
            _fullEquipment[EquipmentType.Weapon] = equipment;
            return;
        }

        equipment.transform.parent = transform;
        equipment.transform.localPosition = Vector3.zero;
        equipment.GetComponent<SphereCollider>().enabled = false;
        equipment.GetComponent<SphereCollider>().isTrigger = false;
        _fullEquipment[equipment.GetEquipmentType()] = equipment;

        switch(equipment.GetEquipmentType())
        {
            case EquipmentType.Weapon:
                GetComponent<AimingInput2>().NewSword();
                break;
            case EquipmentType.Shield:
                GetComponent<Blocking>().NewShield();
                break;
        }
        
    }


    public void DropSword()
    {
        if (_fullEquipment[EquipmentType.Weapon] != null)
        {
            SetLookForPickup(true);
        }
    }

    public Vector3 GetEquipmentLocation(EquipmentType equipment)
    {
        return _fullEquipment[equipment].transform.position;
    }


}
