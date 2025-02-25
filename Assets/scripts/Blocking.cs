using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Blocking : MonoBehaviour
{
    [SerializeField] private GameObject _shield;
    [SerializeField] private GameObject _attacker;
    [SerializeField] private InputActionReference _useShieldAction;
    [SerializeField] private InputActionReference _blockInputAction;
    [SerializeField] private TextMeshPro _txtBlockPower;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _powerReducer = 0.1f;
    [SerializeField] private float _maxTimeHoldBlock = 0.15f;
    [SerializeField] private float _parryAngle = 1.75f;
    [SerializeField] private bool _acceptBothParryDierctions = true;
    [SerializeField] private bool _acceptAllHeightsToParry = true;

    [SerializeField] private GameObject _parryZone;

    private Vector2 _previousDirection;
    private Vector2 _blockInputDirection;
    private const float MIN_DIFF_BETWEEN_INPUT = 0.00125f;
    private const float MIN_BLOCK_POWER = 1f;
    private const float MAX_BLOCK_POWER = 20f;
    private const int MIN_AMOUNT_OF_STRIKES_COUNTERED = 3;
    private float _blockPower = 0.0f;
    private float _accumulatedTime = 0.0f;
    private float _currentBlockingTime = 0.0f;
    private float _currentBrokenTime = 0.0f;
    private BlockState _blockState = BlockState.Idle;
    private List<bool> _parriedBlows = new List<bool>();

    private ParryChanceState _currentParryChance = ParryChanceState.None;
    private float _currentParryAngle = 0.0f;
    private float _startParryAngle = 0.0f;
    private Vector2 _startParryVector;
    private int _parryDirection = 1;
    WalkAnimate _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<WalkAnimate>();
    }
    void Update()
    {
        //BlockPrototype1();
        BlockPrototype2();

    }

    private void BlockPrototype1()
    {
        _blockInputDirection = _blockInputAction.action.ReadValue<Vector2>();
        float distance = _blockInputDirection.sqrMagnitude;

        if (!_useShieldAction.action.IsPressed()/* || distance < 0.1f*/)
        {
            ResetValues();
            return;
        }

        string blockPower = $"BlockPower: {_blockPower:F2}";
        _txtBlockPower.text = blockPower;


        switch (_blockState)
        {
            case BlockState.Idle:
                if (DetectAnalogMovement())
                {
                    _blockState = BlockState.MovingShield;
                }
                break;

            case BlockState.MovingShield:
                if (DetectAnalogMovement())
                {
                    _accumulatedTime += Time.deltaTime;

                    if (distance >= 0.9f)
                    {
                        _blockPower = distance / _accumulatedTime;
                        _blockPower = (_blockPower > MAX_BLOCK_POWER) ? MAX_BLOCK_POWER : _blockPower;
                        _blockState = BlockState.HoldBlock;
                        _accumulatedTime = 0.0f;
                    }
                }
                break;

            case BlockState.HoldBlock:
                _currentBlockingTime += Time.deltaTime;
                if (_currentBlockingTime > _maxTimeHoldBlock)
                {
                    _blockState = BlockState.WeakeningBlock;
                    _currentBlockingTime = 0.0f;
                }

                break;

            case BlockState.WeakeningBlock:
                _blockPower -= (_blockPower <= MIN_BLOCK_POWER) ? 0.0f : Time.deltaTime * _powerReducer;

                if (/*!_useShieldAction.action.IsPressed() ||*/ distance < 0.1f)
                {
                    ResetValues();
                    return;
                }

                break;


        }

        _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
    }

    private void BlockPrototype2()
    {
        if (_blockInputAction)
            _blockInputDirection = _blockInputAction.action.ReadValue<Vector2>();
        float distance = _blockInputDirection.sqrMagnitude;

        if (!_useShieldAction || !_useShieldAction.action.IsPressed())
        {
            _shield.transform.localScale = Vector3.zero; 
            ResetValues();
            return;
        }
            _shield.transform.localScale = Vector3.one; 

        switch (_blockState)
        {
            case BlockState.Idle:
                if (DetectAnalogMovement())
                {
                    _blockState = BlockState.MovingShield;
                    _accumulatedTime = 0.0f;
                }
                break;

            case BlockState.MovingShield:
                _accumulatedTime += Time.deltaTime;
                _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);

                if (!ParryOnZone())
                {
                    _animator.GetHit();
                    _blockState = BlockState.Broken;
                }


                if (!DetectAnalogMovement())
                {
                    if (ReturnOnIdle(distance))
                        return;
                    _blockState = BlockState.HoldBlock;
                    _blockPower = distance / _accumulatedTime;
                    _blockPower =(_blockPower > MAX_BLOCK_POWER)? MAX_BLOCK_POWER : _blockPower;
                    _currentBlockingTime = 0.0f;
                    _txtBlockPower.text = $"BlockPower : {_blockPower}";
                    _parriedBlows.Clear();

                }
                break;

            case BlockState.HoldBlock:
                _currentBlockingTime += Time.deltaTime;
                if (_currentBlockingTime > _maxTimeHoldBlock)
                {
                    _blockState = BlockState.WeakeningBlock;
                }
                break;

            case BlockState.WeakeningBlock:
                if (ReturnOnIdle(distance))
                    return;

                _blockPower -= Time.deltaTime * _powerReducer;
                _blockPower = (_blockPower < MIN_BLOCK_POWER) ? MIN_BLOCK_POWER : _blockPower;
                _txtBlockPower.text = $"BlockPower : {_blockPower}";
                _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
                break;

            case BlockState.Broken:
                _currentBrokenTime += Time.deltaTime;
                if (_currentBrokenTime >= 1.0f)
                {
                    ResetValues();
                    _blockState = BlockState.Idle;
                    _currentBrokenTime = 0.0f;
                }
                break;

        }
    }

    public bool SuccesfullHit(Vector2 hitzone, float radius)
    {
        //if (OverlapCircles(hitzone, radius))
        //{
        //    transform.position += Vector3.right;
        //}

        switch (_blockState)
        {
            case BlockState.Broken:
                break;
            case BlockState.Idle:
                if (OverlapCircles(hitzone, radius, false))
                {
                    OnHit();
                }
                break;

            case BlockState.HoldBlock:
            case BlockState.WeakeningBlock:
                //if (OverlapCircles(hitzone, radius, true) && _blockPower > MIN_BLOCK_POWER)
                if (CheckForHit(hitzone, radius, true) && _blockPower > MIN_BLOCK_POWER)
                {
                    return false;
                }
                //if (OverlapCircles(hitzone, radius, false))
                if (CheckForHit(hitzone, radius, false))
                {
                    OnHit();
                }
                break;

            case BlockState.MovingShield:
                //if (OverlapCircles(hitzone, radius, true))
                if (CheckForHit(hitzone, radius, true))
                {
                    _parriedBlows.Add(true);
                }
                else if (OverlapCircles(hitzone, radius, false))
                {
                    OnHit();
                }
                else
                {
                    _parriedBlows.Clear();
                }

                break;
        }
        TryParry();

        return true;
    }

    private void OnHit()
    {
        gameObject.transform.position = -Vector3.right;
        _blockState = BlockState.Broken;
        _parriedBlows.Clear();
    }

    private void TryParry()
    {
        int count = 0;
        if (_parriedBlows.Count == 0) return;//------------------------------------------
        foreach (bool value in _parriedBlows)
        {
            if (value)
            {
                count++;
            }
        }
        if (count == _parriedBlows.Count && count >= MIN_AMOUNT_OF_STRIKES_COUNTERED)
        {
            //Parry succes
            AttackTimer AIComp = _attacker.gameObject.GetComponent<AttackTimer>();
            AIComp.Parried();
            _parriedBlows.Clear();
        }
    }

    private bool OverlapCircles(Vector2 center, float radius, bool useShield)
    {
        
        if (useShield)
        {
            // Calculate the distance between the centers of the circles
            float distance = Vector2.Distance(center, _shield.transform.position);

            // Check if the distance is less than or equal to the sum of the radii
            return distance <= (radius + (transform.localScale.x *0.5f));
        }
        else
        {
            // Calculate the distance between the centers of the circles
            float distance = Vector2.Distance(center, transform.position);

            // Check if the distance is less than or equal to the sum of the radii
            return distance <= (radius + transform.localScale.x );
        }
        
    }
        
    private bool CheckForHit(Vector2 center, float radius, bool useShield)
    {
        if (useShield)
        {
            return (Mathf.Abs(_shield.transform.position.y - center.y)) < (_shield.transform.localScale.y *0.5f)
                ; 
        }
        else
        {
            float distance = Vector2.Distance(center, transform.position);

            // Check if the distance is less than or equal to the sum of the radii
            return distance <= (radius + transform.localScale.x);
        }
    }

    private bool ReturnOnIdle(float distance)
    {
        if (distance < 0.1f)
        {
            _blockState = BlockState.Idle;
            _blockPower = 0.0f;
            _txtBlockPower.text = "";
            return true;
        }
        return false;
    }

    private bool DetectAnalogMovement()
    {
        var diff = _previousDirection - _blockInputDirection;
        bool value = diff.magnitude > MIN_DIFF_BETWEEN_INPUT;
        //Debug.Log($"{diff.magnitude}");
        //Debug.Log($"{value}");

        _previousDirection = _blockInputDirection;
        return value;
    }

    private void ResetValues()
    {
        _blockState = BlockState.Idle;
        _currentBlockingTime = 0.0f;
        _accumulatedTime = 0.0f;
        //_blockPower = 0.0f;
        //_txtBlockPower.text = "";
        //_shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
        _shield.transform.localPosition = Vector2.zero;

    }

    private bool ParryOnZone()
    {

        if (_currentParryChance == ParryChanceState.Start && AroundParryZone())
        {
            //if (_currentParryChance != ParryChanceState.Succes && _currentParryAngle <= _startParryAngle - 2.1415f)
            if (_currentParryChance != ParryChanceState.Succes && _currentParryAngle >= _parryAngle)
            {
                _currentParryChance = ParryChanceState.Succes;
                AIController attComp = _attacker.GetComponent<AIController>();
                attComp.Parried();
            }

            return true;
        }
        //dont get chocked when there is no Parry attempt
        else if (_currentParryChance != ParryChanceState.Start)
            return true;
        return false;
    }

    //Returns true when gets blocked
    public bool StartHit(AttackStance height, int direction)
    {
        switch(_blockState)
        {
            case BlockState.Idle:
                _animator.GetHit();
                _blockState = BlockState.Broken;
                //Hit
                break;
            case BlockState.MovingShield:
                StartParryTime(height, direction); 
                break;
            case BlockState.HoldBlock:
            case BlockState.WeakeningBlock:
                //Block
                if (!SuccesFullBlock(height, direction))
                {
                    _animator.GetHit();
                    _blockState = BlockState.Broken;
                }
                else
                    return true;
                break;
            case BlockState.Broken:
                //would be cruel to get hit when broken
                break;
            default:
                break;
        }
        return false;
    }
    private void StartParryTime(AttackStance height, int direction)
    {
        if (_currentParryChance == ParryChanceState.None)
        {
            _startParryAngle = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x);
            _parryDirection = direction;
            if (_startParryAngle == 0f )
            {
                Debug.Log("To Slow");
                _currentParryChance = ParryChanceState.Stop;
                return;
            }

            if (_acceptAllHeightsToParry)
                _currentParryChance = ParryChanceState.Start;


            switch(height)
            {
                case AttackStance.Head:
                    if (_startParryAngle > 1f && _startParryAngle < 2.75f)
                        _currentParryChance = ParryChanceState.Start;
                    break;

                case AttackStance.Torso:
                    if ((_startParryAngle > 2f && _startParryAngle <= Mathf.PI) || (_startParryAngle < -2f && _startParryAngle >= -Mathf.PI) // left analog side
                     ||( _startParryAngle < 1.14 && _startParryAngle > -1.14)) //Right analog side
                        _currentParryChance = ParryChanceState.Start;
                    break;

                case AttackStance.Legs:
                    if (_startParryAngle < -1f && _startParryAngle > -2.75f)
                        _currentParryChance = ParryChanceState.Start;
                    break;


            }
            _currentParryAngle = 0.0f;
            _startParryVector = _blockInputDirection;

            if (_currentParryChance == ParryChanceState.None)
            {
                Debug.Log("Wrong start height");
                _currentParryChance = ParryChanceState.Stop;
                return;
            }
            //Debug.Log($"Start!!!");
        }
    }

    public void StopParryTime()
     {
        _currentParryChance = ParryChanceState.None;
        //Debug.Log($"Stop!!!");
     }

    private bool AroundParryZone()
    {
        //float angle = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x) - _startParryAngle;
        //if ( Mathf.Sign(angle) != Mathf.Sign(_currentParryAngle) && _currentParryAngle != 0f)
        //{
        //    angle -= Mathf.Sign(angle) * Mathf.PI *2f;
        //}
        //float diff = Mathf.Abs(angle - _currentParryAngle);
        //if (angle < _currentParryAngle && diff < 0.8f)
        //{
        //    _currentParryAngle = angle;
        //    return true;
        //}

        float angle = Vector2.Angle(_blockInputDirection, _startParryVector) * Mathf.Deg2Rad;
        float diff = Mathf.Abs(angle - _currentParryAngle);
        if( UsedCorrectParryDirection() && angle > _currentParryAngle  && diff < 0.8f)
        {
            _currentParryAngle = angle;
            return true;
        }

        Debug.Log($"Faill, angleDiff = {Mathf.Abs(angle - _currentParryAngle)} ");
        _currentParryChance = ParryChanceState.Stop;
        return false;
    }

    private bool UsedCorrectParryDirection()
    {
        if (_acceptBothParryDierctions)
            return true;

        float cross = _startParryVector.x * _blockInputDirection.y - _startParryVector.y * _blockInputDirection.x;
        return (cross * _parryDirection < 0);
    }

    private bool SuccesFullBlock(AttackStance height, int direction)
    {
        /*bool analogOnRightSide = Mathf.Abs(Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x)) > Mathf.PI * 0.5f;
        int blockDirection = analogOnRightSide ? 1 : -1;
        return direction * blockDirection > 0; */

        float orientation = _animator.GetOrientation();
        Vector2 orientationVector = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        float cross = orientationVector.x * _blockInputDirection.y - orientationVector.y * _blockInputDirection.x;
        return (cross * direction > 0 && Vector2.Angle(orientationVector, _blockInputDirection) < 90f);
    }

}
