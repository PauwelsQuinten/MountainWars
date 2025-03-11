using System;
using UnityEngine;

public class SwordSwingAction : GoapAction
{
    [SerializeField] float _swingSpeed = 1.5f;
    private AimingInput2 _attackComp;
    private WalkAnimate _spriteComp;
    private float _progress = 0f;
    private bool _SwingBack = false;
    private Vector2 _startVec = Vector2.zero;
    [SerializeField] bool _startFromRight = false;
    [SerializeField] bool _randomDirection = false;
    [SerializeField] bool _isFeint = false;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        _progress = 0f;
        _SwingBack = false;

        _startFromRight = _randomDirection? UnityEngine.Random.Range(0, 2) == 0 : _startFromRight;
        if (_startFromRight)
        {
            float orientation = _isFeint ? _spriteComp.GetOrientation() - Mathf.PI * 0.65f : _spriteComp.GetOrientation() - Mathf.PI * 0.5f;
            _startVec = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        }
        else
        {
            float orientation = _isFeint ? _spriteComp.GetOrientation() + Mathf.PI * 0.65f : _spriteComp.GetOrientation() + Mathf.PI * 0.5f;
            _startVec = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        }
       
        _attackComp.Direction = _startVec;
        //_SwordParry.SetSwordMovent(_storedInput);

    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (!_attackComp)
            _attackComp = currentWorldState.GetOwner().GetComponent<AimingInput2>();
        if (!_spriteComp)
            _spriteComp = currentWorldState.GetOwner().GetComponent<WalkAnimate>();
        return _attackComp && _spriteComp;
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        int multiplier = _startFromRight ? 1 : -1;  
        _progress += multiplier * Time.deltaTime * _swingSpeed;

        float angle = MathF.PI * _progress;
        float cosAngle = MathF.Cos(angle);
        float sinAngle = MathF.Sin(angle);

        Vector2 rotatedVec = new Vector2(
            _startVec.x * cosAngle - _startVec.y * sinAngle,
            _startVec.x * sinAngle + _startVec.y * cosAngle
        );

        rotatedVec = rotatedVec.normalized; // Normalize after rotation

        _attackComp.Direction = rotatedVec;
        float newangle = Mathf.Atan2(rotatedVec.y, rotatedVec.x);
        Debug.Log($"{newangle}");
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        float targetProgress = _isFeint ? 0.2f : 1;
        if (_isFeint)
        {
            if (_SwingBack && (_progress <= 0.15f && _progress >= -0.15f))
            {
                _attackComp.Direction = Vector2.zero;
                _startFromRight = !_startFromRight;
                ActionCompleted();
                Debug.Log("feint compleet");
                return true;
            }
            else if (_progress >= targetProgress || _progress <= -targetProgress)
            {
                

                _SwingBack = true;
                _startFromRight = !_startFromRight;
            }
        }

        else if (_progress >= targetProgress || _progress <= -targetProgress)
        {
            _attackComp.Direction = Vector2.zero;

            ActionCompleted();
            return true;
        }
        return false;
    }

}
