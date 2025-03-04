using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum EWorldState
{
    TargetWeaponDistance,
    TargetShieldDistance,
    TargetWeaponMovement,
    TargetShieldMovement,
    TargetShieldOrientation,
    TargetWeaponOrientation,
    TargetSwingSpeed,
    WeaponDistance,
    ShieldDistance,
    WeaponMovement,
    ShieldMovement,
    ShieldOrientation,
    WeaponOrientation,
    SwingSpeed,


}

public class WorldState : MonoBehaviour
{
    [SerializeField] private bool _shouldUpdate=false;
    private const float DEFAULT_VALUE = float.MaxValue;
    //Target
    private GameObject _target;
    private GameObject _targetWeapon;
    private GameObject _targetShield;
    [SerializeField] private float _targetWeaponDistance = DEFAULT_VALUE;
    [SerializeField] private float _targetShieldDistance = DEFAULT_VALUE;
    [SerializeField] private float _targetWeaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _targetShieldMovement = DEFAULT_VALUE; 
    [SerializeField] private float _targetShieldOrientation = DEFAULT_VALUE;
    [SerializeField] private float _targetWeaponOrientation = DEFAULT_VALUE;
    [SerializeField] private float _targetSwingSpeed = DEFAULT_VALUE;

    //Self
    private GameObject _Weapon;
    private GameObject _Shield;
    [SerializeField] private float _WeaponDistance = DEFAULT_VALUE;
    [SerializeField] private float _ShieldDistance = DEFAULT_VALUE;
    [SerializeField] private float _weaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _shieldMovement  = DEFAULT_VALUE;
    [SerializeField] private float _shieldOrientation = DEFAULT_VALUE;
    [SerializeField] private float _weaponOrientation = DEFAULT_VALUE;
    [SerializeField] private float _swingSpeed = DEFAULT_VALUE;

    //Helper state 
    private float _targetOrientation = 0.0f;
    private float _targetWeaponRange = 0.0f;
    private float _weaponRange = 0.0f;
    private float _Orientation = 0.0f;
    private float _weaponMaxMovement = 0.04f;

    public Dictionary<EWorldState,float> _worldStateValues = new Dictionary<EWorldState, float>();


    void Start()
    {
        if (_targetWeaponDistance != DEFAULT_VALUE  || _shouldUpdate)
            _worldStateValues.Add( EWorldState.TargetWeaponDistance, _targetWeaponDistance);
        if (_targetShieldDistance != DEFAULT_VALUE  || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetShieldDistance, _targetShieldDistance);
        if (_targetWeaponMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetWeaponMovement, _targetWeaponMovement);
        if (_targetShieldMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetShieldMovement, _targetShieldMovement);
        if (_targetShieldOrientation != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetShieldOrientation, _targetShieldOrientation);
        if (_targetWeaponOrientation != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetWeaponOrientation, _targetWeaponOrientation);
        if (_targetSwingSpeed != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.TargetSwingSpeed, _targetSwingSpeed);
        if (_WeaponDistance != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.WeaponDistance, _WeaponDistance);
        if (_ShieldDistance != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.ShieldDistance, _ShieldDistance);
        if (_weaponMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.WeaponMovement, _weaponMovement);
        if (_shieldMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.ShieldMovement, _shieldMovement);
        if (_shieldOrientation != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.ShieldOrientation, _shieldOrientation);
        if (_weaponOrientation != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.WeaponOrientation, _weaponOrientation);
        if (_swingSpeed != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues.Add(EWorldState.SwingSpeed, _swingSpeed);
    }

    public void UpdateWorldState()
    {
        if (_target == null && !_shouldUpdate)
            return;

        _targetWeaponDistance = Vector3.Distance( _targetWeapon.transform.position, transform.position);
        _targetShieldDistance = Vector3.Distance(_targetShield.transform.position, transform.position);
        _weaponMovement = Vector2.Distance(_target.transform.position, _targetWeapon.transform.position);
        _targetOrientation = _target.GetComponent<WalkAnimate>().GetOrientation();
        if ( _weaponMovement >=  _weaponMaxMovement * 0.5f )
        {
            var dif = _targetWeapon.transform.position - _target.transform.position;
            float angle = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
        }

    }

    public void SetTargetValues(GameObject target)
    {
        _target = target;
        _weaponMaxMovement = target.GetComponent<AimingInput2>().radius;
        _targetWeapon = target.GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
        _targetShield = target.GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Shield);
    }

    //Return Dictionary with the enums that differ and bool isDesiredStateBigger  
    public Dictionary<EWorldState, bool> CompareWorldState(WorldState desiredWorldState)
    {
        Dictionary< EWorldState,bool > listOfDifference = new Dictionary<EWorldState, bool>();

        foreach (KeyValuePair<EWorldState, float> worldState in desiredWorldState._worldStateValues)
        {
            float dif = worldState.Value - _worldStateValues[worldState.Key];
            if (Mathf.Abs(dif) > 0.01f)
            {
                listOfDifference.Add(worldState.Key, dif > 0f );
            }
        }

        return listOfDifference;
    }
    
    public void UpddateSwingSpeed(float speed)
    {

    }

}
