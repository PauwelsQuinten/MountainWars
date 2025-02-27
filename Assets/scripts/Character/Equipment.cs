using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Shield,
    Armor
}

public class Equipment : MonoBehaviour
{
    [SerializeField] private EquipmentType _equipmentType = EquipmentType.Weapon;
    [SerializeField] private float _maxDurability = 100f;
    [Range(1f, 5f)]
    [SerializeField] private float _weight = 2f;//for attck speed and power outage

    private float _currentDurability = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
   
}
