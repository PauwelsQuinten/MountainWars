using System;
using UnityEngine;

public enum ParryState
{
    Idle,
    Parry,
    Failed,
    Dissarm
}

public class SwordParry : MonoBehaviour
{
    private bool _parryMode = false;
    private bool _parryState = false;
    private GameObject _attacker;
    private WalkAnimate _walkAnimate;
    private Vector2 _inputSwordMovement;
    private ParryState _parryFase;

    private float _currentParryAngle = 0f;
    private Vector2 _startParryVector;
    private Vector2 _secondParryVector;
    [SerializeField]private float _parryAngle = 90f;

    private float _disarmTime = 0.0f;
    [SerializeField]private float _timeForDisarming = 0.5f;

    void Start()
    {
       _walkAnimate = GetComponent<WalkAnimate>();
    }

   
    void Update()
    {
        if (!_parryMode)
            return;

        //if (_parryState && !AroundParryZone())
        if (_parryFase == ParryState.Parry && !AroundParryZone(_startParryVector))
        {
            FailParry();
        }
        //else if (_parryState && _currentParryAngle >= _parryAngle)
        else if (_parryFase == ParryState.Parry && _currentParryAngle >= _parryAngle)
        {
            //Failled sword attack
            SwordSwing sw = _attacker.GetComponent <SwordSwing>();
            sw.SetIdle();
            //Set parried animation to attacker
            AIController attComp = _attacker.GetComponent<AIController>();
            attComp.Parried();
            _parryState = false; 

            //Set to disarm
            _parryFase = ParryState.Dissarm;
            _currentParryAngle = 0;
            _secondParryVector = _inputSwordMovement;
        }
        else if (_parryFase == ParryState.Dissarm)
        {
            _disarmTime += Time.deltaTime;
            

            Debug.Log($"{Vector2.Distance(_startParryVector, _inputSwordMovement)}");
            if (Vector2.Distance(_startParryVector, _inputSwordMovement) < 0.1f)
            {
                AIController attComp = _attacker.GetComponent<AIController>();
                attComp.Disarmed();
                _attacker = null;
                _disarmTime = 0;
                _parryFase = ParryState.Idle;
                return;
            }

            if (_disarmTime >=  _timeForDisarming)
            {
                _attacker = null;
                _disarmTime = 0;
                _parryFase = ParryState.Idle;
                return;
            }
        }

    }

    public void StartParryMode(bool start)
    {
        _parryMode = start;
    }

    public void StartParry(bool isGoing, GameObject attacker)
    {
        
        //check for parrystate to make sure it is only set once
        //if (isGoing && !_parryState)
        if (isGoing && _parryFase == ParryState.Idle)
        {
            _attacker = attacker;
            _startParryVector = _inputSwordMovement;
            _currentParryAngle = 0f;
            _parryState = isGoing;
            _parryFase = ParryState.Parry;
        }
        //else if (!isGoing && _parryState)
        else if (!isGoing && _parryFase == ParryState.Parry)
        {
            FailParry();
            Debug.Log("Time up!!");
        }
        else if (!isGoing)
        {
            _parryFase = ParryState.Idle;
        }

    }

    public void SetSwordMovent(Vector2 input)
    {
        _inputSwordMovement = input;
    }

    private bool AroundParryZone(Vector2 controlVector)
    {
        float angle = Vector2.Angle(_inputSwordMovement, controlVector) ;
        float fullRot = _inputSwordMovement.magnitude;
        if (/*UsedCorrectParryDirection() &&*/ angle >= _currentParryAngle && fullRot > 0.6f)
        {
            _currentParryAngle = angle;
            return true;
        }

        Debug.Log($"Faill, angle = {Mathf.Abs(angle )}, currentAngle= {_currentParryAngle}, length = {fullRot} ");
        return false;
    }

    private void FailParry()
    {
        _walkAnimate.GetHit();
        _attacker = null;
        _parryState = false;
        _parryFase = ParryState.Failed;
        _currentParryAngle = 0f;
    }

    public bool IsParrying()
    {
        return  _parryMode;
    }

}
