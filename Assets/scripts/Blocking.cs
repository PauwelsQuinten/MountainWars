using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum ParryChanceState
{
    Start,
    Failled,
    None,
    Succes
}
public enum BlockState
{
    Idle,
    MovingShield,
    HoldBlock,
    WeakeningBlock,
    Broken
}

public class Blocking : MonoBehaviour
{
    //[SerializeField] private GameObject _shield;
    [SerializeField] private string _txtBlockName;
    private TextMeshPro _txtBlockPower;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _powerReducer = 0.1f;
    [SerializeField] private float _maxTimeHoldBlock = 0.15f;
    [SerializeField] private float _parryAngle = 1.75f;
    [SerializeField] private bool _acceptBothParryDierctions = true;
    [SerializeField] private bool _acceptAllHeightsToParry = true;
    [SerializeField] private float _tempShieldDamageOnUse = 5f;

    private HeldEquipment _heldEquipment;
    private GameObject _attacker;
    private Vector2 _previousDirection;
    private Vector2 _blockInputDirection;
    private const float MIN_DIFF_BETWEEN_INPUT = 0.00125f;
    private const float MIN_BLOCK_POWER = 1f;
    private const float MAX_BLOCK_POWER = 20f;
    private float _blockPower = 0.0f;
    private float _accumulatedTime = 0.0f;
    private float _currentBlockingTime = 0.0f;
    private float _currentBrokenTime = 0.0f;
    private BlockState _blockState = BlockState.Idle;

    private ParryChanceState _currentParryChance = ParryChanceState.None;
    private float _currentParryAngle = 0.0f;
    private float _startParryAngle = 0.0f;
    private Vector2 _startParryVector;
    private int _parryDirection = 1;
    WalkAnimate _animator;

    private float _angleDiffWithOrientation = 0f;
    private bool _followOrientation;

    public EventHandler OnBlockedAttack;

    private void Start()
    {
        _heldEquipment = GetComponent<HeldEquipment>();
        _animator = gameObject.GetComponent<WalkAnimate>();
        var obj = GameObject.Find(_txtBlockName);
        if (obj)
            _txtBlockPower = obj.GetComponent<TextMeshPro>();   
        ActivateBlock(false);
    }

    void Update()
    {
        BlockPrototype();

    }

    //-------------------------------------------------------
    //Public functions


    public void ActivateBlock(bool activate)
    {
        if (!_heldEquipment.HoldsEquipment(EquipmentType.Shield))
            return;

        if (!activate)
        {
            _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localScale = Vector3.zero;
            ResetValues();
        }
        else
            _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localScale = Vector3.one;
    }

    public void HoldBlock(bool hold)
    {
        if (!_heldEquipment.HoldsEquipment(EquipmentType.Shield))
            return;

        float orient = _animator ? _animator.GetOrientation() : 0.0f;
        float angleInput = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x );
        _angleDiffWithOrientation = angleInput - orient;    
        _followOrientation = hold;
        if (!hold)
            ResetValues();
    }

    public bool StartHit(AttackStance height, int direction, GameObject attacker)
    {
        //Check if shield is equiped before trying to Block
        if (!_heldEquipment.HoldsEquipment(EquipmentType.Shield))
            return false;

        _attacker = attacker;

        switch(_blockState)
        {
            case BlockState.Idle:
                _animator.GetHit();
                _blockState = BlockState.Broken;
                //Hit
                break;
            case BlockState.MovingShield:
                StartParryTime(height, direction); 
                if (_currentParryChance == ParryChanceState.Failled)
                {
                    _animator.GetHit();
                    _blockState = BlockState.Broken;
                }

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
                {
                    _heldEquipment.EquipmentEnduresHit(EquipmentType.Shield, _tempShieldDamageOnUse);
                    return true;
                }
                _attacker = null;
                break;
            case BlockState.Broken:
                //would be cruel to get hit when broken
                break;
            default:
                break;
        }
        return false;
    }

    public void SetInputDirection(Vector2 input)
    {
        _blockInputDirection = input;
    }

    public void StopParryTime()
     {
        _currentParryChance = ParryChanceState.None;
        //Debug.Log($"Failled!!!");
     }
 

    //-------------------------------------------------------
    //private functions

    private void ReduceBlockPower()
    {
        _blockPower -= Time.deltaTime * _powerReducer;
        _blockPower = (_blockPower < MIN_BLOCK_POWER) ? MIN_BLOCK_POWER : _blockPower;
        if (_txtBlockPower)
            _txtBlockPower.text = $"BlockPower : {_blockPower}";

    }

    private void BlockPrototype()
    {
        //Check if shield is equiped before trying to Block
        if (!_heldEquipment.HoldsEquipment(EquipmentType.Shield))
            return;

        //when holding block while attacking
        if (_followOrientation)
        {
            FollowTarget();
            ReduceBlockPower();
            return;
        }


        float distance = _blockInputDirection.sqrMagnitude;
     
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
                _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);

                if (!SuccesfullParryOnZone())
                {
                    _animator.GetHit();
                    _blockState = BlockState.Broken;
                    if(!_heldEquipment.EquipmentEnduresHit(EquipmentType.Shield, _tempShieldDamageOnUse * 0.25f))
                    {
                        _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localScale = Vector3.zero;
                        ResetValues();
                    }

                }

                if (!DetectAnalogMovement())
                {
                    if (ReturnOnIdle(distance))
                        return;
                    _blockState = BlockState.HoldBlock;
                    _blockPower = distance / _accumulatedTime;
                    _blockPower =(_blockPower > MAX_BLOCK_POWER)? MAX_BLOCK_POWER : _blockPower;
                    _currentBlockingTime = 0.0f;
                    if (_txtBlockPower)
                        _txtBlockPower.text = $"BlockPower : {_blockPower}";

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

                ReduceBlockPower();
                _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
                break;

            case BlockState.Broken:
                _currentBrokenTime += Time.deltaTime;
                if (_currentBrokenTime >= 1.0f)
                {
                    ResetValues();
                    _currentBrokenTime = 0.0f;
                }
                break;

        }
    }

    private bool ReturnOnIdle(float distance)
    {
        if (distance < 0.1f)
        {
            _blockState = BlockState.Idle;
            _blockPower = 0.0f;
            if (_txtBlockPower)
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
        if (_txtBlockPower)
            _txtBlockPower.text = "";
        //_shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
        _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition = Vector2.zero;

    }

    private bool SuccesfullParryOnZone()
    {
        if (_currentParryChance == ParryChanceState.Start && AroundParryZone())
        {
            if (_currentParryChance != ParryChanceState.Succes && _currentParryAngle >= _parryAngle)
            {
                _currentParryChance = ParryChanceState.Succes;
                AIController attComp = _attacker.GetComponent<AIController>();
                attComp.Parried();
                _attacker = null;
            }

            return true;
        }
        //dont get chocked when there is no Parry attempt
        else if (_currentParryChance != ParryChanceState.Start)
            return true;
        return false;
    }


    //return true when succesfully blocked
    private void StartParryTime(AttackStance height, int direction)
    {
        if (_currentParryChance == ParryChanceState.None)
        {
            _startParryAngle = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x);
            _parryDirection = direction;
            if (_startParryAngle == 0f )
            {
                Debug.Log("To Slow");
                _currentParryChance = ParryChanceState.Failled;
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

            if (_currentParryChance == ParryChanceState.None || direction == 0)
            {
                Debug.Log("Wrong start height");
                _currentParryChance = ParryChanceState.Failled;
                return;
            }
            //Debug.Log($"Start!!!");
        }
    }


    private bool AroundParryZone()
    {
        float angle = Vector2.Angle(_blockInputDirection, _startParryVector) * Mathf.Deg2Rad;
        float diff = Mathf.Abs(angle - _currentParryAngle);
        if( UsedCorrectParryDirection() && angle > _currentParryAngle  && diff < 0.8f)
        {
            _currentParryAngle = angle;
            return true;
        }

        Debug.Log($"Faill, angleDiff = {Mathf.Abs(angle - _currentParryAngle)} ");
        _currentParryChance = ParryChanceState.Failled;
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
        float maxAcceptedAngle = 90.0f;
        float minAcceptedAngle = 30.0f;
        if (direction == 0)
        {
            maxAcceptedAngle = 30f;
            minAcceptedAngle = 0.0f;
        }

        float orientation = _animator.GetOrientation();
        Vector2 orientationVector = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        float cross = orientationVector.x * _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition.y - orientationVector.y * _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition.x;
        //cross = orientationVector.x * _blockInputDirection.y - orientationVector.y * _blockInputDirection.x;
        float blockAngle = Vector2.Angle(orientationVector, _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition);
        //float blockAngle = Vector2.Angle(orientationVector, _blockInputDirection);

        return cross * direction >= 0 && blockAngle < maxAcceptedAngle && blockAngle > minAcceptedAngle;
    }

    private void FollowTarget()
    {
       
       float orient = _animator ? _animator.GetOrientation() : 0.0f;
       float newAngle = orient + _angleDiffWithOrientation;
       //newAngle *= Mathf.Rad2Deg;
       Vector2 angleVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
        _heldEquipment.GetEquipment(EquipmentType.Shield).transform.localPosition = new Vector3(angleVector.x * _radius, angleVector.y * _radius, 0.0f);
       
    }

}
