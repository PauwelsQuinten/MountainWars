using UnityEngine;

public class StabAction : GoapAction
{
    [SerializeField] float _stabSpeed = 1.5f;
    private AimingInput2 _attackComp;
    private WalkAnimate _spriteComp;
    private float _progress = 0f;
    private Vector2 _startVec = Vector2.zero;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        
        _progress = 0f;
        float orientation = _spriteComp.GetOrientation();
        Vector2 orienVec = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        _startVec = orienVec * -0.5f;
        _attackComp.Direction = _startVec;

    }

    public override bool IsVallid(WorldState currentWorldState)
    {
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
        if ( _progress >= 1.5f)
        {
            _attackComp.Direction = Vector2.zero;

            return true;
        }
        return false;
    }

}
