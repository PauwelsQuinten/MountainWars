using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Size
{
    None, Small, Medium, Large
}

public enum OpeningDirection
{
    Left, Center, Right, Full
}

public struct TargetOpenings
{
    public Size Size;
    public OpeningDirection Direction;
    public TargetOpenings(Size size, OpeningDirection direction)
    {
        Size = size; Direction = direction; 
    }
}

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

    TargetWeaponDistance,
    TargetShieldDistance,
    TargetWeaponPosesion,
    TargetShieldPosesion,
    TargetBehaviour,
    TargetStamina,
    TargetOpening,
    HasTarget,
    TargetDistance,

    WeaponDistance,
    ShieldDistance,
    WeaponPosesion,
    ShieldPosesion,
    Stamina,
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
    OnLeft,

    EquipmentUp,
    EquipmentHalfUp,
    EquipmentDown,

    FullStamina,
    MidStamina,
    LowOnStamina,

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
    [SerializeField] private WorldStateValue _targetWeaponMovement = WorldStateValue.DontCare; 
    [SerializeField] private WorldStateValue _targetShieldMovement = WorldStateValue.DontCare; 
    [SerializeField] private WorldStateValue _targetShieldOrientation = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetWeaponOrientation = WorldStateValue.DontCare;
    [SerializeField] private float _targetSwingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _targetWeaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetShieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetBehaviour = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _hasTarget = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _targetStamina = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _Targetopening = WorldStateValue.DontCare;

    //Self
    //private GameObject _weapon;
    //private GameObject _shield;
    private HeldEquipment _npcEquipment;
    private Equipment _foundEquipment;
    [SerializeField] private WorldStateValue _WeaponDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _ShieldDistance = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _weaponMovement = WorldStateValue.DontCare; 
    [SerializeField] private WorldStateValue _shieldMovement = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _shieldOrientation = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _weaponOrientation = WorldStateValue.DontCare;
    [SerializeField] private float _swingSpeed = DEFAULT_VALUE;
    [SerializeField] private WorldStateValue _weaponPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _shieldPossesion = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _behaviour = WorldStateValue.DontCare;
    [SerializeField] private WorldStateValue _stamina = WorldStateValue.DontCare;


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
        if (_targetWeaponMovement != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetWeaponMovement] = _targetWeaponMovement;
        if (_targetShieldMovement != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetShieldMovement] = _targetShieldMovement;
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
        if (_targetStamina != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetStamina] = _targetStamina;
        if (_Targetopening != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.TargetOpening] = _Targetopening;

        if (_WeaponDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.WeaponDistance] = _WeaponDistance;
        if (_ShieldDistance != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.ShieldDistance] = _ShieldDistance;
        if (_weaponMovement != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.WeaponMovement] = _weaponMovement;
        if (_shieldMovement != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.ShieldMovement] = _shieldMovement;
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
        if (_stamina != WorldStateValue.DontCare || _shouldUpdate)
            _worldStateValues2[EWorldState.Stamina] = _stamina;


    }
    public void UpdateWorldState()
    {
        if (!_shouldUpdate)
            return;

        //NPC UPDATE WITHOUT FOUND TARGET
        if (!_target)
        {
            CalculateNpcStamina();

            //WEAPON
            _orientation = GetComponent<WalkAnimate>().GetOrientation();
            if (_npcEquipment.GetEquipment(EquipmentType.Weapon))
            {
                _worldStateValues2[EWorldState.WeaponDistance] = WorldStateValue.OutOfRange;

                CalculateEquipmentMovement(EWorldState.WeaponMovement, EquipmentType.Weapon, false);

                if (_worldStateValues2[EWorldState.WeaponMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.WeaponMovement] == WorldStateValue.EquipmentUp)
                {
                    CalculateOrientation(EquipmentType.Weapon, EWorldState.WeaponOrientation, false);
                }
                else
                    _worldStateValues2[EWorldState.WeaponOrientation] = WorldStateValue.DontCare;
            }
            //if weapon is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Weapon)
                _worldStateValues2[EWorldState.WeaponDistance] =
                    Vector2.Distance(_foundEquipment.transform.position, transform.position) > 0.2f ? WorldStateValue.OutOfRange : WorldStateValue.InRange;


            //SHIELD
            if (_npcEquipment.GetEquipment(EquipmentType.Shield))
            {
                _worldStateValues2[EWorldState.ShieldDistance] = WorldStateValue.OutOfRange;

                CalculateEquipmentMovement(EWorldState.ShieldMovement, EquipmentType.Shield, false);

                if (_worldStateValues2[EWorldState.ShieldMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.WeaponMovement] == WorldStateValue.EquipmentUp)

                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.ShieldOrientation, false);
                }
                else
                    _worldStateValues2[EWorldState.ShieldOrientation] = WorldStateValue.DontCare;
            }
            //if shield is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Shield)
                _worldStateValues2[EWorldState.ShieldDistance] =
                    Vector2.Distance(_foundEquipment.transform.position, transform.position) > 0.2f ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
        }
        //------------------------------------------------------------------------------------
        //TARGET FOUND
        //-----------------------------------------------------------------------------------
        else
        {
            //TARGET UPDATE
            CalculateTargetStamina();
            LookForOpening();

            //Target
            float distance = Vector3.Distance(_target.transform.position, transform.position);
            if (distance <= _weaponRange)
                _worldStateValues2[EWorldState.TargetDistance] = WorldStateValue.InRange;
            else if (distance > _weaponRange * 3f)
                _worldStateValues2[EWorldState.TargetDistance] = WorldStateValue.FarAway;
            else
                _worldStateValues2[EWorldState.TargetDistance] = WorldStateValue.OutOfRange;
            
            _targetOrientation = _target.GetComponent<WalkAnimate>().GetOrientation();

            //WEAPON
            if (_targetEquipment.GetEquipment(EquipmentType.Weapon) != null)
            {
                _worldStateValues2[EWorldState.TargetWeaponDistance] = (Vector3.Distance(_targetEquipment.GetEquipment(EquipmentType.Weapon).transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                CalculateEquipmentMovement(EWorldState.TargetWeaponMovement, EquipmentType.Weapon, true);
                if (_worldStateValues2[EWorldState.TargetWeaponMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.TargetWeaponMovement] == WorldStateValue.EquipmentUp)
                {
                    CalculateOrientation(EquipmentType.Weapon, EWorldState.TargetWeaponOrientation, true);
                    _worldStateValues[EWorldState.TargetSwingSpeed] = _target.GetComponent<AimingInput2>().GetSwingSpeed();
                    _targetSwingSpeed = _worldStateValues[EWorldState.TargetSwingSpeed];
                }
                else
                {
                    _worldStateValues2[EWorldState.TargetWeaponOrientation] = WorldStateValue.DontCare;
                    _worldStateValues[EWorldState.TargetSwingSpeed] = 0f;
                    _targetSwingSpeed = _worldStateValues[EWorldState.TargetSwingSpeed];
                }

            }

            //SHIELD
            if (_targetEquipment.GetEquipment(EquipmentType.Shield) != null)
            {
                _worldStateValues2[EWorldState.TargetShieldDistance] = (Vector3.Distance(_targetEquipment.GetEquipment(EquipmentType.Shield).transform.position, transform.position) > _weaponRange)? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                CalculateEquipmentMovement(EWorldState.TargetShieldMovement, EquipmentType.Shield, true);
                if (_worldStateValues2[EWorldState.TargetShieldMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.TargetShieldMovement] == WorldStateValue.EquipmentUp)
                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.TargetShieldOrientation, true);
                }
                else
                    _worldStateValues2[EWorldState.TargetShieldOrientation] = WorldStateValue.DontCare;
            }
            //--------------------------------------------------------------------------------------------------------------


            //NPC UPDATE
            CalculateNpcStamina();


            //WEAPON
            _orientation = GetComponent<WalkAnimate>().GetOrientation();
            if (_npcEquipment.GetEquipment(EquipmentType.Weapon))
            {
                _worldStateValues2[EWorldState.WeaponDistance] = 
                    (Vector3.Distance(_npcEquipment.GetEquipment(EquipmentType.Weapon).transform.position, _target.transform.position) > _weaponRange) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;
                CalculateEquipmentMovement(EWorldState.WeaponMovement, EquipmentType.Weapon, false);

                if (_worldStateValues2[EWorldState.WeaponMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.WeaponMovement] == WorldStateValue.EquipmentUp)
                {

                    CalculateOrientation(EquipmentType.Weapon, EWorldState.WeaponOrientation, false);
                    
                }
                else
                    _worldStateValues2[EWorldState.WeaponOrientation] = WorldStateValue.DontCare;

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
                CalculateEquipmentMovement(EWorldState.ShieldMovement, EquipmentType.Shield, false);

                if (_worldStateValues2[EWorldState.ShieldMovement] == WorldStateValue.EquipmentHalfUp || _worldStateValues2[EWorldState.ShieldMovement] == WorldStateValue.EquipmentUp)
                {
                    CalculateOrientation(EquipmentType.Shield, EWorldState.ShieldOrientation, false);

                }
                else
                    _worldStateValues2[EWorldState.ShieldOrientation] = WorldStateValue.DontCare;
            }
            //if shield is unequiped but found one
            else if (_foundEquipment && _foundEquipment.GetEquipmentType() == EquipmentType.Shield)
                _worldStateValues2[EWorldState.ShieldDistance] =
                    (Vector3.Distance(_foundEquipment.transform.position, transform.position) > 0.2f) ? WorldStateValue.OutOfRange : WorldStateValue.InRange;

        }

    }

    private void LookForOpening()
    {
        WorldStateValue value = WorldStateValue.DontCare;
        //Look for large openings

        //Look for mid openings

        //Look for low openings

        _worldStateValues2[EWorldState.TargetOpening] = value;
    }


    private void CalculateNpcStamina()
    {
        float stamina = GetComponent<StaminaManager>().GetStamina();
        if (stamina > 0.75f)
            _worldStateValues2[EWorldState.Stamina] = WorldStateValue.FullStamina;
        else if (stamina > 0.4f)
            _worldStateValues2[EWorldState.Stamina] = WorldStateValue.MidStamina;
        else
            _worldStateValues2[EWorldState.Stamina] = WorldStateValue.LowOnStamina;
    }

    private void CalculateTargetStamina()
    {
        float stamina = _target.GetComponent<StaminaManager>().GetStamina();
        if (stamina > 0.75f)
            _worldStateValues2[EWorldState.TargetStamina] = WorldStateValue.FullStamina;
        else if (stamina > 0.4f)
            _worldStateValues2[EWorldState.TargetStamina] = WorldStateValue.MidStamina;
        else
            _worldStateValues2[EWorldState.TargetStamina] = WorldStateValue.LowOnStamina;
    }


    private void CalculateEquipmentMovement(EWorldState listValue, EquipmentType type, bool target)
    {
        float diff = target?
           Vector2.Distance(_target.transform.position, _targetEquipment.GetEquipment(type).transform.position)
           : Vector2.Distance(transform.position, _npcEquipment.GetEquipment(type).transform.position);

        float maxMovement = type == EquipmentType.Weapon ? _weaponMaxMovement : _shieldMaxMovement;
        if (diff >= maxMovement * 0.95f)
            _worldStateValues2[listValue] = WorldStateValue.EquipmentUp;
        else if (diff >= maxMovement * 0.5f)
            _worldStateValues2[listValue] = WorldStateValue.EquipmentHalfUp;
        else
            _worldStateValues2[listValue] = WorldStateValue.EquipmentDown;
    }

    private void CalculateOrientation(EquipmentType type, EWorldState listKey, bool target)
    {
        GameObject checkedEquipment = null;
        float oriantation = 0f;
        Vector2 diff = Vector2.zero;

        if (target)
        {
            oriantation = _targetOrientation;
            if (_targetEquipment.GetEquipment(type))
                checkedEquipment = _targetEquipment.GetEquipment(type);
            else
                checkedEquipment = _targetEquipment.GetReserveEquipment(type);

            diff = checkedEquipment.transform.position - _target.transform.position;
        }
        else
        {
            oriantation = _Orientation;
            if (_npcEquipment.GetEquipment(type))
                checkedEquipment = _npcEquipment.GetEquipment(type);
            else
                checkedEquipment = _npcEquipment.GetReserveEquipment(type);

            diff = checkedEquipment.transform.position - transform.position;
        }
        
        float angle = Mathf.Atan2(diff.y, diff.x) - oriantation;
        if (angle < -Mathf.PI)
            angle += Mathf.PI * 2f;
        if (angle > Mathf.PI)
            angle -= Mathf.PI * 2f;

        if (angle < 0.3f && angle > -0.3f)
            _worldStateValues2[listKey] = WorldStateValue.OnCenter;
        else if (angle < 0  && angle > -Mathf.PI)
            _worldStateValues2[listKey] = WorldStateValue.OnRight;
        else if (angle > 0 && angle < Mathf.PI)
            _worldStateValues2[listKey] = WorldStateValue.OnLeft;
        else
            _worldStateValues2[listKey] = WorldStateValue.DontCare;

        _targetWeaponOrientation = _worldStateValues2[EWorldState.TargetWeaponOrientation]; //Only for to be  shown in Worldstate in editor, no further purpose
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
            _weaponRange = _weapon ? _weapon.GetComponent<Equipment>().GetAttackRange() + transform.localScale.x : transform.localScale.x * 1.5f;

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
            _targetWeaponRange = _weapon ? _weapon.GetComponent<Equipment>().GetAttackRange() + transform.localScale.x : transform.localScale.x * 1.5f;

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
        _worldStateValues[EWorldState.SwingSpeed] = speed;
        _swingSpeed = _worldStateValues[EWorldState.SwingSpeed];
    }

    public void FoundEquipment(Equipment equipment)
    {
        _foundEquipment = equipment;
    }

    #endregion public Setters
}
