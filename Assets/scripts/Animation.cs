using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;


public class WalkAnimate : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    private float _orientation = 0.0f;
    [SerializeField]
    private GameObject _target;

    public bool LockOn;
    public CharacterOrientation Orientation;
    private float _rotationCutOff = 45f / 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        LockOnTarget();
    }

    public void Walk(Vector2 direction)
    {
        if (_target && LockOn)
        {
            var lookDir = _target.transform.position - transform.position;
            lookDir.Normalize();
            _animator.SetFloat("yInput", lookDir.y);
            _animator.SetFloat("xInput", lookDir.x);
        }
        else
        {
            //_animator.SetFloat("yInput", direction.y);
            //_animator.SetFloat("xInput", direction.x);
        }

         float length = direction.magnitude;
         _animator.SetFloat("DirectionMagnitude", length);

        if (length <= 0.01f || _target) 
            return; 
        _orientation = Mathf.Atan2(direction.y, direction.x);
        _orientation = (_orientation <= -3.1415f) ? Mathf.PI : _orientation;
        //_animator.SetFloat("orientation", _orientation);


    }

    public void Attack()
    {
        _animator.SetTrigger("Test");
    }

    public void DoLockOn(GameObject target)
    {
        Debug.Log("Lock");
        bool value = _animator.GetBool("LockOn");
        value = !value;
        _animator.SetBool("LockOn", value);

        _target = value ? target : null;
        
    }

    private void LockOnTarget()
    {
        if (!LockOn) return;
        if (!_target)
            return;

        var lookDirection = _target.transform.position - transform.position;
        float angleR = Mathf.Atan2(lookDirection.y, lookDirection.x);
        _orientation = angleR;
        float currentAngleDegree = angleR * Mathf.Rad2Deg;
        _animator.SetFloat("orientation", _orientation);

        if (currentAngleDegree < 0f + _rotationCutOff && currentAngleDegree >= 0f || currentAngleDegree > 0f - _rotationCutOff && currentAngleDegree <= 0f)
            Orientation = CharacterOrientation.East;
        else if (currentAngleDegree < 45f + _rotationCutOff && currentAngleDegree > 45f - _rotationCutOff)
            Orientation = CharacterOrientation.NorthEast;
        else if (currentAngleDegree < 90f + _rotationCutOff && currentAngleDegree > 90f - _rotationCutOff)
            Orientation = CharacterOrientation.North;
        else if (currentAngleDegree < 135f + _rotationCutOff && currentAngleDegree > 135f - _rotationCutOff)
            Orientation = CharacterOrientation.NorthWest;
        else if (currentAngleDegree <= 180f && currentAngleDegree >= 180f - _rotationCutOff || currentAngleDegree < -180f + _rotationCutOff && currentAngleDegree <= -180f)
            Orientation = CharacterOrientation.West;
        else if (currentAngleDegree < -135f + _rotationCutOff && currentAngleDegree > -135f - _rotationCutOff)
            Orientation = CharacterOrientation.SouthWest;
        else if (currentAngleDegree < -90f + _rotationCutOff && currentAngleDegree > -90f - _rotationCutOff)
            Orientation = CharacterOrientation.South;
        else if (currentAngleDegree < -45f + _rotationCutOff && currentAngleDegree > -45f - _rotationCutOff)
            Orientation = CharacterOrientation.SouthEast;
    }
}
