using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Shield,
    Armor,
    Fist
}

public class Equipment : MonoBehaviour
{
    [SerializeField] private EquipmentType _equipmentType = EquipmentType.Weapon;
    [SerializeField] private float _maxDurability = 100f;
    [Range(0.5f, 3f)]
    [SerializeField] private float _attack = 3f;
    [Range(1f, 5f)]
    [SerializeField] private float _weight = 2f;//for attck speed and power outage

    private float _currentDurability = 0f;

    void Start()
    {
        _currentDurability = _maxDurability;
    }

    public bool DecreaseDurability(float damage)
    {

        _currentDurability -= damage;
        return _currentDurability < 0f;
    
    }

    public EquipmentType GetEquipmentType()
    { return _equipmentType; }

    public GameObject GetEquipment()
    {
        return gameObject;
    }
    
    public float GetEquipmentstrength()
    {
        return _attack;
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller == null) return;
        other.GetComponent<HeldEquipment>().PickupNewEquipment(this);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterController controller = other.GetComponent<CharacterController>();
        if (controller == null) return;
        other.GetComponent<HeldEquipment>().PickupNewEquipment(null);
    }
}

//Add toAImInput2
//
//  public void NewSword()
//    {
//        if (_sword.GetComponent<Equipment>().GetEquipmentType() == EquipmentType.Fist)
//            _sword.transform.localScale = Vector3.zero;
//        _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
//        radius = 0.4f;
//    }
//    public void SwordBroke()
//    {
//        if (GetComponent<HeldEquipment>().HoldsEquipment(EquipmentType.Weapon))
//            return;
//        if (GetComponent<HeldEquipment>().HoldsEquipment(EquipmentType.Shield))
//        {
//            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Shield);
//            radius = 1.0f;
//        }
//        else
//        {
//            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Fist);
//            _sword.transform.localScale = Vector3.one;
//            radius = 1.5f;
//        }
//    }
//
