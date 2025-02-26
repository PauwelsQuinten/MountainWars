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
    Stop,
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
    [SerializeField] private GameObject _shield;
    [SerializeField] private GameObject _attacker;
    [SerializeField] private TextMeshPro _txtBlockPower;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _powerReducer = 0.1f;
    [SerializeField] private float _maxTimeHoldBlock = 0.15f;
    [SerializeField] private float _parryAngle = 1.75f;
    [SerializeField] private bool _acceptBothParryDierctions = true;
    [SerializeField] private bool _acceptAllHeightsToParry = true;


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
        _animator = gameObject.GetComponent<WalkAnimate>();
        ActivateBlock(false);
    }

    void Update()
    {
        BlockPrototype();

    }

    private void BlockPrototype()
    {
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
                    //_txtBlockPower.text = $"BlockPower : {_blockPower}";

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

    private void ReduceBlockPower()
    {
        _blockPower -= Time.deltaTime * _powerReducer;
        _blockPower = (_blockPower < MIN_BLOCK_POWER) ? MIN_BLOCK_POWER : _blockPower;
        //_txtBlockPower.text = $"BlockPower : {_blockPower}";
    }

    public void SetInputDirection(Vector2 input)
    {
        _blockInputDirection = input;
    }

    public void HoldBlock(bool hold)
    {
        float orient = _animator ? _animator.GetOrientation() : 0.0f;
        float angleInput = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x );
        _angleDiffWithOrientation = angleInput - orient;    
        _followOrientation = hold;
        if (!hold)
            ResetValues();
    }

    public void ActivateBlock(bool activate)
    {
        if (!activate)
        {
            _shield.transform.localScale = Vector3.zero;
            ResetValues();
        }
        else
            _shield.transform.localScale = Vector3.one;
    }
 
    private bool ReturnOnIdle(float distance)
    {
        if (distance < 0.1f)
        {
            _blockState = BlockState.Idle;
            _blockPower = 0.0f;
            //_txtBlockPower.text = "";
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

    public bool StartHit(AttackStance height, int direction, GameObject attacker)
    {
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
                if (_currentParryChance == ParryChanceState.Stop)
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
                    //_attacker.GetComponent<SwordSwing>().GetKnocked();

                    return true;
                }
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

            if (_currentParryChance == ParryChanceState.None || direction == 0)
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
        float maxAcceptedAngle = 90.0f;
        float minAcceptedAngle = 30.0f;
        if (direction == 0)
        {
            maxAcceptedAngle = 30f;
            minAcceptedAngle = 0.0f;
        }

        float orientation = _animator.GetOrientation();
        Vector2 orientationVector = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        float cross = orientationVector.x * _blockInputDirection.y - orientationVector.y * _blockInputDirection.x;
        float blockAngle = Vector2.Angle(orientationVector, _blockInputDirection);
        return cross * direction >= 0 && blockAngle < maxAcceptedAngle && blockAngle > minAcceptedAngle;
    }

    private void FollowTarget()
    {
       
       float orient = _animator ? _animator.GetOrientation() : 0.0f;
       float newAngle = orient + _angleDiffWithOrientation;
       //newAngle *= Mathf.Rad2Deg;
       Vector2 angleVector = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
       _shield.transform.localPosition = new Vector3(angleVector.x * _radius, angleVector.y * _radius, 0.0f);

       Debug.Log($"start: {_angleDiffWithOrientation}, new: {newAngle}");
       
    }

}
