using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Coroutine _goFlyCoroutine;

    private void Awake()
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
            if (equipmentType == EquipmentType.Weapon) GetComponent<AimingInput2>().enabled = false;
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

    public GameObject GetReserveEquipment(EquipmentType type)
    {
       
       if( type == EquipmentType.Weapon)
           return _fullEquipment[EquipmentType.Shield].GetEquipment();
       else if( type == EquipmentType.Shield)
           return _fullEquipment[EquipmentType.Weapon].GetEquipment();
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

    public void SetLookForPickup(bool drop = false, int direction = 1, bool goFly = false)
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

            if (goFly)
            {
                float orientation = GetComponent<WalkAnimate>().Orientation * Mathf.Rad2Deg;
                Transform t = Instantiate(transform);
                t.rotation = Quaternion.Euler(new Vector3(0,0,t.rotation.z + (orientation - 90)));

                if (direction == 1) _goFlyCoroutine = StartCoroutine(LaunchSword(_fullEquipment[targetedtype].transform.position, t.right * 6, 2, 1, _fullEquipment[targetedtype].transform));
                else if (direction == -1) _goFlyCoroutine = StartCoroutine(LaunchSword(_fullEquipment[targetedtype].transform.position, -t.right * 6, 2, 1, _fullEquipment[targetedtype].transform));
                else if (direction == 0) _goFlyCoroutine = StartCoroutine(LaunchSword(_fullEquipment[targetedtype].transform.position, -t.up * 6, 2, 1, _fullEquipment[targetedtype].transform));

                Destroy(t.gameObject);
            }
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


    public void DropSword(int direction = 1, bool goFly = false)
    {
        if (_fullEquipment[EquipmentType.Weapon] != null)
        {
            SetLookForPickup(true, direction, goFly);
        }
    }

    public Vector3 GetEquipmentLocation(EquipmentType equipment)
    {
        return _fullEquipment[equipment].transform.position;
    }

    IEnumerator LaunchSword(Vector3 startPos, Vector3 direction, float distance, float speed, Transform obj)
    {
        while (Vector3.Distance(startPos, obj.position) <= distance)
        {
            obj.position += speed * Time.deltaTime * direction;
            yield return null;
        }

        Vector3 resetZ = obj.position;
        resetZ.z = 0;
        obj.position = resetZ;
    }
}
