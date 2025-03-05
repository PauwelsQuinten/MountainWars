using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    TargetWeaponPosesion,
    TargetShieldPosesion,
    TargetBehaviour,
    HasTarget,
    TargetDistance,

    WeaponDistance,
    ShieldDistance,
    WeaponMovement,
    ShieldMovement,
    ShieldOrientation,
    WeaponOrientation,
    SwingSpeed,
    WeaponPosesion,
    ShieldPosesion,
    Behaviour,

}

public enum StateType
{
    Desired,
    Satisfying
}

public enum WorldStateValue
{
    InPosesion,
    NotInPosesion,

    NeedsLower,
    NeedsHigher,

    DontCare,//--DEFAULT : Set to this value if you want to ignore the worldstate--

    Idle,
    Rotating,
    Attacking,
    Parried,
    Blocked,

    InRange,
    OutOfRange,
    FarAway
}

public class WorldState : MonoBehaviour
{
    //only the currentWorldState in the planner should update
    [SerializeField] private bool _shouldUpdate=false;
    public StateType _worldStateType = StateType.Desired;
    private const float DEFAULT_VALUE = 9000;
    //Target
    private GameObject _target;
    private GameObject _targetWeapon;
    private GameObject _targetShield;
    [SerializeField] private WorldStateValue _targetWeaponDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetShieldDistance = WorldStateValue.DontCare;
    [SerializeField] private float _targetWeaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _targetShieldMovement = DEFAULT_VALUE; 
    [SerializeField] private float _targetShieldOrientation = DEFAULT_VALUE;
    [SerializeField] private float _targetWeaponOrientation = DEFAULT_VALUE;
    [SerializeField] private float _targetSwingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _targetWeaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetShieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetBehaviour = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _hasTarget = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetDistance = WorldStateValue.DontCare;

    //Self
    private GameObject _weapon;
    private GameObject _shield;
    [SerializeField] private WorldStateValue _WeaponDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _ShieldDistance = WorldStateValue.DontCare;
    [SerializeField] private float _weaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _shieldMovement  = DEFAULT_VALUE;
    [SerializeField] private float _shieldOrientation = DEFAULT_VALUE;
    [SerializeField] private float _weaponOrientation = DEFAULT_VALUE;
    [SerializeField] private float _swingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _weaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _shieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _behaviour = WorldStateValue.DontCare;

    //Helper state 
    private float _targetOrientation = 0.0f;
    private float _targetWeaponRange = 0.0f;
    private float _weaponRange = 0.0f;
    private float _Orientation = 0.0f;
    private float _weaponMaxMovement = 0.04f;
    private float _shieldMaxMovement = 1f;

    public Dictionary<EWorldState,float> _worldStateValues = new Dictionary<EWorldState, float>();
    public Dictionary<EWorldState,WorldStateValue> _worldStateValues2 = new Dictionary<EWorldState, WorldStateValue>();


    void Start()
    {
        if (GetComponent<HeldEquipment>() != null && _shouldUpdate)
        {
            _weapon = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
            _weaponPossesion = _weapon? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
            _weaponRange = _weapon? _weapon.GetComponent<Equipment>().GetAttackRange() : 0f;

            _shield = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Shield);
            _shieldPossesion = _shield? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
        }


        if (_targetWeaponDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add( EWorldState.TargetWeaponDistance, _targetWeaponDistance);
        if (_targetShieldDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.TargetShieldDistance, _targetShieldDistance);
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
        if (_targetWeaponPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.TargetWeaponPosesion, _targetWeaponPossesion);
        if (_targetShieldPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.TargetShieldPosesion, _targetShieldPossesion);
        if (_targetBehaviour != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.TargetBehaviour, _targetBehaviour);
        if (_hasTarget != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.HasTarget, _hasTarget);
        if (_targetDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.TargetDistance, _targetDistance);

        if (_WeaponDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.WeaponDistance, _WeaponDistance);
        if (_ShieldDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.ShieldDistance, _ShieldDistance);
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
        if (_weaponPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.WeaponPosesion, _weaponPossesion);
        if (_shieldPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.ShieldPosesion, _shieldPossesion);
        if (_behaviour != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2.Add(EWorldState.Behaviour, _behaviour);

    }

    public void UpdateWorldState()
    {
        if (_target == null || !_shouldUpdate)
            return;

        //TARGET UPDATE
        _worldStateValues2[EWorldState.TargetDistance] = (Vector3.Distance(_target.transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
        _worldStateValues2[EWorldState.TargetWeaponDistance] = (Vector3.Distance(_targetWeapon.transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
        _worldStateValues2[EWorldState.TargetShieldDistance] = (Vector3.Distance(_targetShield.transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
        _worldStateValues[EWorldState.TargetWeaponMovement] = Vector2.Distance(_target.transform.position, _targetWeapon.transform.position);
        _worldStateValues[EWorldState.TargetShieldMovement] = Vector2.Distance(_target.transform.position, _targetShield.transform.position);
        _targetOrientation = _target.GetComponent<WalkAnimate>().GetOrientation();
        if (_worldStateValues[EWorldState.TargetWeaponMovement] >=  _weaponMaxMovement * 0.5f )
        {
            var dif = _targetWeapon.transform.position - _target.transform.position;
            //_worldStateValues[EWorldState.TargetWeaponOrientation] = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
            _worldStateValues[EWorldState.TargetWeaponOrientation] = Mathf.Atan2(dif.y, dif.x);
            
        }
        if (_worldStateValues[EWorldState.TargetShieldMovement] >= _shieldMaxMovement * 0.5f )
        {
            var dif = _targetShield.transform.position - _target.transform.position;
            //_worldStateValues[EWorldState.TargetWeaponOrientation] = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
            _worldStateValues[EWorldState.TargetShieldOrientation] = Mathf.Atan2(dif.y, dif.x);
            
        }

        //NPC UPDATE
        if (_weapon)
        {
            _worldStateValues2[EWorldState.WeaponDistance] = (Vector3.Distance(_weapon.transform.position, _target.transform.position) > _weaponRange) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
            _worldStateValues[EWorldState.WeaponMovement] = Vector2.Distance(transform.position, _weapon.transform.position);
        }

        if (_shield)
        {
            _worldStateValues2[EWorldState.ShieldDistance] = (Vector3.Distance(_shield.transform.position, _target.transform.position) > _weaponRange) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
            _worldStateValues[EWorldState.ShieldMovement] = Vector2.Distance(transform.position, _shield.transform.position);
        }

        _targetOrientation = _target.GetComponent<WalkAnimate>().GetOrientation();
        if (_weapon && _worldStateValues[EWorldState.WeaponMovement] >= _weaponMaxMovement * 0.5f)
        {
            var dif = _weapon.transform.position - transform.position;
            //_worldStateValues[EWorldState.TargetWeaponOrientation] = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
            _worldStateValues[EWorldState.WeaponOrientation] = Mathf.Atan2(dif.y, dif.x);

        }
        if (_shield && _worldStateValues[EWorldState.ShieldMovement] >= _shieldMaxMovement * 0.5f)
        {
            var dif = _shield.transform.position - transform.position;
            //_worldStateValues[EWorldState.TargetWeaponOrientation] = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
            _worldStateValues[EWorldState.ShieldOrientation] = Mathf.Atan2(dif.y, dif.x);

        }



    }

    public void SetTargetValues(GameObject target)
    {
        _target = target;
        _worldStateValues2[EWorldState.HasTarget] = _target? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;

        if (!target)
            return;
        _weaponMaxMovement = target.GetComponent<AimingInput2>().radius;
        _shieldMaxMovement = target.GetComponent<Blocking>().Radius;
       
        _targetWeapon = target.GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
        if (_targetWeapon)
        {
            _targetWeaponPossesion = WorldStateValue.InPosesion;
            _worldStateValues2[EWorldState.TargetWeaponPosesion] = WorldStateValue.InPosesion;
        }

        
        _targetShield = target.GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Shield);
        if (_targetShield)
        {
            _targetShieldPossesion = WorldStateValue.InPosesion;
            _worldStateValues2[EWorldState.TargetShieldPosesion] = WorldStateValue.InPosesion;
        }

       
       
    }

    //Return Dictionary with the enums that differ and bool isDesiredStateBigger  
    public Dictionary<EWorldState, WorldStateValue> CompareWorldState(WorldState desiredWorldState)
    {
        Dictionary< EWorldState, WorldStateValue> listOfDifference = new Dictionary<EWorldState, WorldStateValue>();

        //Check floats
        foreach (KeyValuePair<EWorldState, float> worldState in desiredWorldState._worldStateValues)
        {
            float dif = worldState.Value - _worldStateValues[worldState.Key];
            if (Mathf.Abs(dif) > 0.01f)
            {
                listOfDifference.Add(worldState.Key, (dif < 0f)? WorldStateValue.NeedsLower : WorldStateValue.NeedsHigher );
            }
        }
        //Check bools
        foreach(KeyValuePair<EWorldState, WorldStateValue>worldState in desiredWorldState._worldStateValues2)
        {
            if (worldState.Value - _worldStateValues2[worldState.Key] != 0 && worldState.Value != WorldStateValue.DontCare)
            {
                listOfDifference.Add(worldState.Key, worldState.Value);
            }
        }

        return listOfDifference;
    }

    public GameObject GetTarget() { return _target; }
    public GameObject GetOwner() { return gameObject; }
    
    public void SetActive(bool active)
    {
        _shouldUpdate = active;
    }

    public void UpddateSwingSpeed(float speed)
    {

    }

}
