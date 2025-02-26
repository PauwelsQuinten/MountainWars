using System;
using UnityEngine;

public class SwordParry : MonoBehaviour
{
    private bool _parryMode = false;
    private bool _parryState = false;
    private GameObject _attacker;
    private WalkAnimate _walkAnimate;
    private Vector2 _inputSwordMovement;

    private float _currentParryAngle = 0f;
    private Vector2 _startParryVector;
    [SerializeField]private float _parryAngle = 90f;

    void Start()
    {
       _walkAnimate = GetComponent<WalkAnimate>();
    }

   
    void Update()
    {
        if (!_parryMode)
            return;

        if (_parryState && !AroundParryZone())
        {
            FailParry();
        }
        else if (_parryState && _currentParryAngle >= _parryAngle)
        {
            //Stop sword attack
            SwordSwing sw = _attacker.GetComponent <SwordSwing>();
            sw.SetIdle();
            //Set parried animation to attacker
            AIController attComp = _attacker.GetComponent<AIController>();
            attComp.Parried();
            _attacker = null;
            _parryState = false;    
        }

    }

    public void StartParryMode(bool start)
    {
        _parryMode = start;
    }

    public void StartParry(bool isGoing, GameObject attacker)
    {
        _attacker = attacker;
        
        //check for parrystate to make sure it is only set once
        if (isGoing && !_parryState)
        {
            _startParryVector = _inputSwordMovement;
            _currentParryAngle = 0f;
            _parryState = isGoing;
        }
        else if (!isGoing && _parryState)
        {
            FailParry();
            Debug.Log("Time up!!");
        }

    }

    public void SetSwordMovent(Vector2 input)
    {
        _inputSwordMovement = input;
    }

    private bool AroundParryZone()
    {
        float angle = Vector2.Angle(_inputSwordMovement, _startParryVector) ;
        float fullRot = _inputSwordMovement.magnitude;
        if (/*UsedCorrectParryDirection() &&*/ angle > _currentParryAngle && fullRot > 0.6f)
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
        _currentParryAngle = 0f;
    }

    public bool IsParrying()
    {
        return  _parryMode;
    }

}
