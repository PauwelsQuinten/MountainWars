using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;


public class WalkAnimate : MonoBehaviour
{
    Animator _animator;
    private float _orientation = 0.0f;
    [SerializeField] private GameObject _target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        LockOnTarget();
    }

    //--------------------------------------------------------
    //PRIVATE FUNCTIONS
    private void LockOnTarget()
    {
        if (!_target)
            return;

        var lookDirection = _target.transform.position - transform.position;
        float angleR = Mathf.Atan2(lookDirection.y, lookDirection.x);
        _orientation = angleR;
        _animator.SetFloat("orientation", _orientation);

    }

    //---------------------------------------------------------------------
    //PUBLIC FUNCTIONS
    public void Attack()
    {
        _animator.SetTrigger("Test");
    }
    public void Walk(Vector2 direction)
    {
        if (_target)
        {
            var lookDir = _target.transform.position - transform.position;
            lookDir.Normalize();
            _animator.SetFloat("yInput", lookDir.y);
            _animator.SetFloat("xInput", lookDir.x);
        }
        else
        {
            _animator.SetFloat("yInput", direction.y);
            _animator.SetFloat("xInput", direction.x);
        }

         float length = direction.magnitude;
         _animator.SetFloat("DirectionMagnitude", length);

        if (length <= 0.01f || _target) 
            return; 
        _orientation = Mathf.Atan2(direction.y, direction.x);
        _orientation = (_orientation <= -3.1415f) ? Mathf.PI : _orientation;
        _animator.SetFloat("orientation", _orientation);


    }


    public void LockOn(GameObject target)
    {
        Debug.Log("Lock");
        bool value = _animator.GetBool("LockOn");
        value = !value;
        _animator.SetBool("LockOn", value);

        _target = value ? target : null;
        
    }

    public float GetOrientationDegree()
    {
        return _orientation * Mathf.Rad2Deg;
    }

    public float GetOrientation()
    {
        return _orientation;
    }

    public bool IsLockedOn()
    {
        return _target != null;
    }
}
