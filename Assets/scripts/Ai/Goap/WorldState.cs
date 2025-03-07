using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum EWorldState
{
    TargetWeaponMovement,
    TargetShieldMovement,
    TargetShieldOrientation,
    TargetWeaponOrientation,
    TargetSwingSpeed,

    WeaponMovement,
    ShieldMovement,
    ShieldOrientation,
    WeaponOrientation,
    SwingSpeed,
    //list1 -2 line----
    TargetWeaponDistance,
    TargetShieldDistance,
    TargetWeaponPosesion,
    TargetShieldPosesion,
    TargetBehaviour,
    HasTarget,
    TargetDistance,

    WeaponDistance,
    ShieldDistance,
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
    DontCare,//--DEFAULT : Set to this value if you want to ignore the worldstate--

    InPosesion,
    NotInPosesion,

    NeedsLower,
    NeedsHigher,

    Idle,
    Rotating,
    Attacking,
    Parried,
    Blocked,

    InRange,
    OutOfRange,
    FarAway,

    OnRight,
    OnCenter,
    OnLeft
}

public class WorldState : MonoBehaviour
{
    //only the currentWorldState in the planner should update
    [SerializeField] private bool _shouldUpdate=false;
    [SerializeField] private List<EWorldState> _lowtoHighPriority = new List<EWorldState>();
    public StateType _worldStateType = StateType.Desired;
    private const float DEFAULT_VALUE = 9000;
    //Target
    private HeldEquipment _targetEquipment;
    private GameObject _target;
    //private GameObject _targetWeapon;
    //private GameObject _targetShield;
    [SerializeField] private WorldStateValue _targetWeaponDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetShieldDistance = WorldStateValue.DontCare;
    [SerializeField] private float _targetWeaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _targetShieldMovement = DEFAULT_VALUE; 
    [SerializeField] private WorldStateValue _targetShieldOrientation = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetWeaponOrientation = WorldStateValue.DontCare;
    [SerializeField] private float _targetSwingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _targetWeaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetShieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetBehaviour = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _hasTarget = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetDistance = WorldStateValue.DontCare;

    //Self
    //private GameObject _weapon;
    //private GameObject _shield;
    private HeldEquipment _npcEquipment;
    private Equipment _foundEquipment;
    [SerializeField] private WorldStateValue _WeaponDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _ShieldDistance = WorldStateValue.DontCare;
    [SerializeField] private float _weaponMovement = DEFAULT_VALUE; 
    [SerializeField] private float _shieldMovement = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _shieldOrientation = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _weaponOrientation = WorldStateValue.DontCare;
    [SerializeField] private float _swingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _weaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _shieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _behaviour = WorldStateValue.DontCare;

    //Helper state 
    private float _targetOrientation = 0.0f;
    private float _orientation = 0.0f;
    private float _targetWeaponRange = 0.0f;
    private float _weaponRange = 0.0f;
    private float _Orientation = 0.0f;
    private float _weaponMaxMovement = 0.04f;
    private float _shieldMaxMovement = 1f;

    public Dictionary<EWorldState,float> _worldStateValues = new Dictionary<EWorldState, float>();
    public Dictionary<EWorldState,WorldStateValue> _worldStateValues2 = new Dictionary<EWorldState, WorldStateValue>();


    void Start()
    {
        UpdateHeldEquipment();
        UpdateTargetHeldEquipment();

        //When using a priority list for your desiredState
        if (_lowtoHighPriority.Count > 0)
        {
            foreach (var item in _lowtoHighPriority)
            {
                if ((int)item >= 10)//List 2
                {
                    _worldStateValues2.Add(item, WorldStateValue.DontCare);
                }
                else //list 1
                {
                    _worldStateValues.Add(item, DEFAULT_VALUE);
                }
            }
        }


        if (_targetWeaponDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetWeaponDistance] = _targetWeaponDistance;
        if (_targetShieldDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetShieldDistance] = _targetShieldDistance;
        if (_targetWeaponMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.TargetWeaponMovement] = _targetWeaponMovement;
        if (_targetShieldMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.TargetShieldMovement] = _targetShieldMovement;
        if (_targetShieldOrientation != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetShieldOrientation] = _targetShieldOrientation;
        if (_targetWeaponOrientation != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetWeaponOrientation] = _targetWeaponOrientation;
        if (_targetSwingSpeed != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.TargetSwingSpeed] = _targetSwingSpeed;
        if (_targetWeaponPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetWeaponPosesion] = _targetWeaponPossesion;
        if (_targetShieldPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetShieldPosesion] = _targetShieldPossesion;
        if (_targetBehaviour != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetBehaviour] = _targetBehaviour;
        if (_hasTarget != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.HasTarget] = _hasTarget;
        if (_targetDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetDistance] = _targetDistance;

        if (_WeaponDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.WeaponDistance] = _WeaponDistance;
        if (_ShieldDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.ShieldDistance] = _ShieldDistance;
        if (_weaponMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.WeaponMovement] = _weaponMovement;
        if (_shieldMovement != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.ShieldMovement] = _shieldMovement;
        if (_shieldOrientation != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.ShieldOrientation] = _shieldOrientation;
        if (_weaponOrientation != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.WeaponOrientation] = _weaponOrientation;
        if (_swingSpeed != DEFAULT_VALUE || _shouldUpdate)
            _worldStateValues[EWorldState.SwingSpeed] = _swingSpeed;
        if (_weaponPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.WeaponPosesion] = _weaponPossesion;
        if (_shieldPossesion != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.ShieldPosesion] = _shieldPossesion;
        if (_behaviour != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.Behaviour] = _behaviour;

    }
    public void UpdateWorldState()
    {
        if (!_shouldUpdate)
            return;

        //NPC UPDATE WITHOUT FOUND TARGET
        if (!_target)
        {
            //WEAPON
            _orientation = GetComponent<WalkAnimate>().GetOrientation();
            if (_npcEquipment.GetEquipment(EquipmentType.Weapon))
            {
                _worldStateValues2[EWorldState.WeaponDistance] = WorldStateValue.OutOfRange;
                _worldStateValues[EWorldState.WeaponMovement] =
                    Vector2.Distance(transform.position, _npcEquipment.GetEquipment(EquipmentType.Weapon).transform.position);
                if (_worldStateValues[EWorldState.WeaponMovement] >= _weaponMaxMovement * 0.5f)
                {
                    CalculateOrientation(EquipmentType.Weapon, EWorldState.WeaponOrientation, false);
                }
            }
            //if weapon is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Weapon)
                _worldStateValues2[EWorldState.WeaponDistance] =
                    Vector2.Distance(_foundEquipment.transform.position, transform.position) > 0.2f? WorldStateValue.OutOfRange : WorldStateValue.InRange;


            //SHIELD
            if (_npcEquipment.GetEquipment(EquipmentType.Shield))
            {
                _worldStateValues2[EWorldState.ShieldDistance] = WorldStateValue.OutOfRange;
                _worldStateValues[EWorldState.ShieldMovement] =
                    Vector2.Distance(transform.position, _npcEquipment.GetEquipment(EquipmentType.Shield).transform.position);
                if (_worldStateValues[EWorldState.ShieldMovement] >= _shieldMaxMovement * 0.5f)
                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.ShieldOrientation, false);
                }
            }
            //if shield is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Shield)
                _worldStateValues2[EWorldState.ShieldDistance] = 
                    Vector2.Distance(_foundEquipment.transform.position, transform.position) > 0.2f? WorldStateValue.OutOfRange : WorldStateValue.InRange;
        }
        else
        {
           //TARGET UPDATE
            //Target
            _worldStateValues2[EWorldState.TargetDistance] = (Vector3.Distance(_target.transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
            _targetOrientation = _target.GetComponent<WalkAnimate>().GetOrientation();

            //WEAPON
            if (_targetEquipment.GetEquipment(EquipmentType.Weapon) != null)
            {
                _worldStateValues2[EWorldState.TargetWeaponDistance] = (Vector3.Distance(_targetEquipment.GetEquipment(EquipmentType.Weapon).transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                _worldStateValues[EWorldState.TargetWeaponMovement] = Vector2.Distance(_target.transform.position, _targetEquipment.GetEquipment(EquipmentType.Weapon).transform.position);
                if (_worldStateValues[EWorldState.TargetWeaponMovement] >= _weaponMaxMovement * 0.5f)
                {
                   CalculateOrientation(EquipmentType.Weapon, EWorldState.TargetWeaponOrientation, true);

                }
            }

            //SHIELD
            if (_targetEquipment.GetEquipment(EquipmentType.Shield) != null)
            {
                _worldStateValues2[EWorldState.TargetShieldDistance] = (Vector3.Distance(_targetEquipment.GetEquipment(EquipmentType.Shield).transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                _worldStateValues[EWorldState.TargetShieldMovement] = Vector2.Distance(_target.transform.position, _targetEquipment.GetEquipment(EquipmentType.Shield).transform.position);
                if (_worldStateValues[EWorldState.TargetShieldMovement] >= _shieldMaxMovement * 0.5f)
                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.TargetShieldOrientation, true);
                }
            }
            //--------------------------------------------------------------------------------------------------------------


            //NPC UPDATE
            //WEAPON
            _orientation = GetComponent<WalkAnimate>().GetOrientation();
            if (_npcEquipment.GetEquipment(EquipmentType.Weapon))
            {
                _worldStateValues2[EWorldState.WeaponDistance] = 
                    (Vector3.Distance(_npcEquipment.GetEquipment(EquipmentType.Weapon).transform.position, _target.transform.position) > _weaponRange) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                _worldStateValues[EWorldState.WeaponMovement] = 
                    Vector2.Distance(transform.position, _npcEquipment.GetEquipment(EquipmentType.Weapon).transform.position);
                if (_worldStateValues[EWorldState.WeaponMovement] >= _weaponMaxMovement * 0.5f)
                {
                 
                    CalculateOrientation(EquipmentType.Weapon, EWorldState.WeaponOrientation, false);
                }
            }
            //if weapon is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Weapon) 
                _worldStateValues2[EWorldState.WeaponDistance] = 
                    (Vector3.Distance(_foundEquipment.transform.position, transform.position) > 0.2f) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;


            //SHIELD
            if (_npcEquipment.GetEquipment(EquipmentType.Shield))
            {
                _worldStateValues2[EWorldState.ShieldDistance] =
                    (Vector3.Distance(_npcEquipment.GetEquipment(EquipmentType.Shield).transform.position, _target.transform.position) > _weaponRange) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                _worldStateValues[EWorldState.ShieldMovement] =
                    Vector2.Distance(transform.position, _npcEquipment.GetEquipment(EquipmentType.Shield).transform.position);
                if (_worldStateValues[EWorldState.ShieldMovement] >= _shieldMaxMovement * 0.5f)
                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.ShieldOrientation, false);

                }
            }
            //if shield is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Shield)
                _worldStateValues2[EWorldState.ShieldDistance] =
                    (Vector3.Distance(_foundEquipment.transform.position, transform.position) > 0.2f) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;

        }

    }

    private void CalculateOrientation(EquipmentType type, EWorldState listKey, bool target)
    {
        var dif = target?
            _targetEquipment.GetEquipment(type).transform.position - transform.position : _npcEquipment.GetEquipment(type).transform.position - transform.position;
        float angle = Mathf.Atan2(dif.y, dif.x) - _targetOrientation;
        if (Mathf.Abs(angle) < Mathf.PI * 0.5f)
            _worldStateValues2[listKey] = WorldStateValue.OnRight;
        else
            _worldStateValues2[listKey] = WorldStateValue.OnLeft;
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

    //WorldState Setters from actions or GoapPlanner
    #region public Setter/Getters

    public void UpdateHeldEquipment()
    {
        _npcEquipment = GetComponent<HeldEquipment>();
        if (_npcEquipment != null && _shouldUpdate)
        {
            var _weapon = _npcEquipment.GetEquipment(EquipmentType.Weapon);
            _weaponPossesion = _weapon ? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
            _worldStateValues2[EWorldState.WeaponPosesion] = _weaponPossesion;
            _weaponRange = _weapon ? _weapon.GetComponent<Equipment>().GetAttackRange() + transform.localScale.x *0.5f : 0f;

            var _shield = _npcEquipment.GetEquipment(EquipmentType.Shield);
            _shieldPossesion = _shield ? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
            _worldStateValues2[EWorldState.ShieldPosesion] = _shieldPossesion;

            _weaponMaxMovement = GetComponent<AimingInput2>().radius;
            _shieldMaxMovement = GetComponent<Blocking>().Radius;
        }
    }
    
    public void UpdateTargetHeldEquipment()
    {
        if (_target == null)
            return;

        _targetEquipment = _target.GetComponent<HeldEquipment>();
        if (_targetEquipment != null && _shouldUpdate)
        {
            var _weapon = _targetEquipment.GetEquipment(EquipmentType.Weapon);
            _targetWeaponPossesion = _weapon ? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
            _worldStateValues2[EWorldState.TargetWeaponPosesion] = _targetWeaponPossesion;
            _targetWeaponRange = _weapon ? _weapon.GetComponent<Equipment>().GetAttackRange() : 0f;

            var _shield = _targetEquipment.GetEquipment(EquipmentType.Shield);
            _targetShieldPossesion = _shield ? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;
            _worldStateValues2[EWorldState.TargetShieldPosesion] = _targetShieldPossesion;
        }
    }


    public void SetTargetValues(GameObject target)
    {
        _target = target;
        _worldStateValues2[EWorldState.HasTarget] = _target? WorldStateValue.InPosesion : WorldStateValue.NotInPosesion;

        UpdateTargetHeldEquipment();
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

    public void FoundEquipment(Equipment equipment)
    {
        _foundEquipment = equipment;
    }

    #endregion public Setters
}
