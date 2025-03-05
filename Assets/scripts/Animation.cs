using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;


public class WalkAnimate : MonoBehaviour
{
    Animator _animator;
    public float Orientation = 0.0f;
    private GameObject _target;

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
        Orientation = angleR;
        _animator.SetFloat("orientation", Orientation);

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
        Orientation = Mathf.Atan2(direction.y, direction.x);
        Orientation = (Orientation <= -3.1415f) ? Mathf.PI : Orientation;
        _animator.SetFloat("orientation", Orientation);


    }


    public void LockOn(GameObject target)
    {
        //Debug.Log("Lock");
        //bool value = _animator.GetBool("LockOn");
        //value = !value;
        //_animator.SetBool("LockOn", value);
        //
        //_target = value ? target : null;
        
        _target = target;
        _animator.SetBool("LockOn", _target != null);
    }

    public void Rotate(float angle)
    {
        _animator.SetFloat("orientation", angle);
        Orientation = angle;
    }


    public void GetHit()
    {
        _animator.SetTrigger("GetHit");
    }

    public float GetOrientationDegree()
    {
        return _animator.GetFloat("orientation") * Mathf.Rad2Deg;
    }

    public float GetOrientation()
    {
        return _animator.GetFloat("orientation");
        //return Orientation;
    }

    public bool IsLockedOn()
    {
        return _target != null;
    }

    public void Parried()
    {
        _animator.SetTrigger("Parried");

    }
    
    public void Disarmed()
    {
        _animator.SetTrigger("Disarmed");

    }
}
