using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GoapSwingState
{
    Start, Parry, Block, EndParry
}
public class SwordSwingAction : GoapAction
{
    [SerializeField] float _swingSpeed = 1.5f;
    [SerializeField] float _coolDownTime = 1.5f;
    private AimingInput2 _attackComp;
    private WalkAnimate _spriteComp;
    private AIController _aiComp;
    private float _progress = 0f;
    private bool _SwingBack = false;
    private Vector2 _startVec = Vector2.zero;
    [SerializeField] bool _startFromRight = false;
    [SerializeField] bool _randomDirection = false;
    [SerializeField] bool _isFeint = false;
    private bool _attackCoolDown = false;
    private GoapSwingState _currentSwingState= GoapSwingState.Start;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        _progress = 0f;
        _SwingBack = false;
        _currentSwingState = GoapSwingState.Start;

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
       
        _aiComp.AimAction_performed(_startVec, FightStyle.Sword);
        //_attackComp.Direction = _startVec;

    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (!_attackComp)
            _attackComp = currentWorldState.GetOwner().GetComponent<AimingInput2>();
        if (!_spriteComp)
            _spriteComp = currentWorldState.GetOwner().GetComponent<WalkAnimate>();
        if (!_aiComp)
            _aiComp = currentWorldState.GetOwner().GetComponent<AIController>();

        if ( _isFeint )
        {
            Cost = Random.Range(0.4f, 1.5f);
            return _attackComp && _spriteComp && _aiComp;
        }

        if (currentWorldState.AttackCoolDown <= 0f && !_startFromRight 
            && currentWorldState._worldStateValues2[EWorldState.TargetOpening] == WorldStateValue.InPosesion)
        {
            if (currentWorldState.CurrentOpening.Direction == OpeningDirection.Right
                || currentWorldState.CurrentOpening.Direction == OpeningDirection.Full)
                Cost = 0.35f;
        }
        else if (currentWorldState.AttackCoolDown <= 0f && _startFromRight 
            && currentWorldState._worldStateValues2[EWorldState.TargetOpening] == WorldStateValue.InPosesion)
        {
            if (currentWorldState.CurrentOpening.Direction == OpeningDirection.Left
                || currentWorldState.CurrentOpening.Direction == OpeningDirection.Full)
                Cost = 0.35f;
        }
        else
            Cost = 1f;

        return _attackComp && _spriteComp && _aiComp;
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

        _aiComp.AimAction_performed(rotatedVec, FightStyle.Sword);

        switch(_currentSwingState)
        {
            case GoapSwingState.Start:
                break;
            case GoapSwingState.Parry:
                break;
            case GoapSwingState.Block:
                break;
            case GoapSwingState.EndParry:
                break;
                default:
                break;
        }
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        float targetProgress = _isFeint ? 0.2f : 0.7f;
        if (_isFeint)
        {
            if (_SwingBack && (_progress <= 0.15f && _progress >= -0.15f))
            {
                //_attackComp.Direction = Vector2.zero;
                _aiComp.AimAction_performed(Vector2.zero, FightStyle.Sword);
                _startFromRight = !_startFromRight;
                ActionCompleted();
                Debug.Log("feint compleet");
                return true;
            }
            else if (_progress >= targetProgress || _progress <= -targetProgress)
            {                
                _SwingBack = true;

                int multiplier = _startFromRight ? 1 : -1;
                _progress = multiplier * targetProgress;
                
                _startFromRight = !_startFromRight;
            }
        }

        else if (_progress >= targetProgress || _progress <= -targetProgress)
        {
            //_attackComp.Direction = Vector2.zero;
            _aiComp.AimAction_performed(Vector2.zero, FightStyle.Sword);
            currentWorldState.AttackCoolDown += _coolDownTime;

            ActionCompleted();
            return true;
        }
        return false;
    }

    public override void CancelAction()
    {
        _aiComp.AimAction_performed(Vector2.zero, FightStyle.Sword);

    }

}
