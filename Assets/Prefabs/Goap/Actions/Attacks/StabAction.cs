using UnityEngine;

public class StabAction : GoapAction
{
    [SerializeField] float _stabSpeed = 1.5f;
    private AimingInput2 _attackComp;
    private WalkAnimate _spriteComp;
    private float _progress = 0f;
    private Vector2 _startVec = Vector2.zero;
    [SerializeField] bool _startFromZero = false;
    [SerializeField] float _coolDownTime = 2f;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        
        _progress = 0f;
        if (_startFromZero)
        {
            _startVec = Vector2.zero;   
        }
        else
        {
            float orientation = _spriteComp.GetOrientation();
            Vector2 orienVec = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
            _startVec = orienVec * -0.5f;
            _attackComp.Direction = _startVec;
        }


    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        if (currentWorldState.AttackCoolDown <= 0f && currentWorldState._worldStateValues2[EWorldState.TargetOpening] == WorldStateValue.InPosesion)
        {
            if ( currentWorldState.CurrentOpening.Direction == OpeningDirection.Center
                || currentWorldState.CurrentOpening.Direction == OpeningDirection.Full)
            {
                if (_startFromZero && currentWorldState.CurrentOpening.Size == Size.Small)
                    Cost = 0.4f;
                else if (!_startFromZero && (currentWorldState.CurrentOpening.Size == Size.Medium || currentWorldState.CurrentOpening.Size == Size.Large))
                    Cost = 0.3f;
            }

        }
        else
            Cost = 1f;

        _attackComp = currentWorldState.GetOwner().GetComponent<AimingInput2>();
        _spriteComp = currentWorldState.GetOwner().GetComponent<WalkAnimate>();
        return _attackComp && _spriteComp;
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        _progress += Time.deltaTime * _stabSpeed;
        float orientation = _spriteComp.GetOrientation();
        Vector2 orienVec = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        orienVec = _startVec + orienVec * _progress;
        _attackComp.Direction = orienVec;
    }

    public override bool IsCompleted(WorldState currentWorldState, WorldState activeActionDesiredState)
    {
        float maxProgres = _startFromZero ? 1.5f : 2f;

        if ( _progress >= maxProgres)
        {
            _attackComp.Direction = Vector2.zero;
            currentWorldState.AttackCoolDown += _coolDownTime;
            return true;
        }
        return false;
    }
    public override void CancelAction()
    {
        _attackComp.Direction = Vector2.zero;

    }

}
