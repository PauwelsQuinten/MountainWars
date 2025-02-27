using System;
using UnityEngine;
using UnityEngine.UIElements;
public class SwordSwing : MonoBehaviour
{
    [SerializeField] GameObject _sword;
    [SerializeField] float _swingSpeed = 25f;
    [SerializeField] float _stabSpeed = 25f;
    [SerializeField] float _swingAngle = 90.0f;
    [SerializeField] float _anglePerspective = 71.0f;
    [SerializeField] private GameObject _target;
    [SerializeField] private LayerMask _targetMask;

    private WalkAnimate _animationRef;
    private SphereCollider _targetCollider;

    private bool _isSwinging = false;
    private Vector2 _defaultPosition;
    private float _startSwingAngle = 0.0f;
    private int _swingDirection = 0;
    private float _defaultAngle;
    private float _currentAngleMovement = 0.0f;
    private AttackStance _attackStance;
    private Vector3 _currentOrientationVector;
    private Vector3 _startStabVector;

    public EventHandler OnStopHit;
    public EventHandler<HitEventArgs> OnStartHit;
    public class HitEventArgs : EventArgs
    {
        public AttackStance AttackHeight { get; }
        public int Direction { get; }

        public HitEventArgs(AttackStance param1, int param2)
        {
            AttackHeight = param1;
            Direction = param2;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animationRef = GetComponent<WalkAnimate>();

        _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
        _defaultAngle = _sword.transform.rotation.eulerAngles.z;
        _defaultPosition = _sword.transform.position;

        _targetCollider = gameObject.AddComponent<SphereCollider>();
        _targetCollider.isTrigger = false;
        _targetCollider.enabled = false;
        _targetCollider.radius = 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_isSwinging)
        {
            Attack();
        }
        else
            PointToTarget();
        
    }

    public void StartSwing(AttackType attackType, AttackStance attackHeight, int startDirection)
    {
        //set swing values
        _isSwinging = true;
        _swingDirection = -startDirection;
        _attackStance = attackHeight;

        float height = gameObject.transform.position.y + gameObject.transform.localScale.y * (int)attackHeight * 0.5f;
        _sword.transform.position = new Vector3(_sword.transform.position.x, height, 0.0f);

        //AdjustHeightPerspective();
        float orientation = (_animationRef) ? _animationRef.GetOrientation()  : 0.0f;
        float orientationDegree = orientation * Mathf.Rad2Deg;
        float rotation = startDirection * _swingAngle;

        switch (attackType)
        {
            case AttackType.HorizontalSlashLeft:
            case AttackType.HorizontalSlashRight:
               //minus the 90 degree because default is North
                _sword.transform.Rotate(0.0f, 0.0f, orientationDegree);
                _startSwingAngle = _sword.transform.rotation.eulerAngles.z;
                break;
            case AttackType.UpperSlashRight:
            case AttackType.UpperSlashLeft:
                break;
            case AttackType.DownSlashRight:
            case AttackType.DownSlashLeft:
                
                break;
            case AttackType.StraightUp:
            case AttackType.StraightDown:
                break;
            case AttackType.Stab:
                _currentOrientationVector = new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0f);
                _sword.transform.position -= _currentOrientationVector * 1.25f;
                _startStabVector = _sword.transform.position;
                _swingDirection = 0;
                break;
            case AttackType.None:
                break;
        }
    }

    private void Attack()
    {
        MoveHitCollider();

        if (_swingDirection == 0)
            Stab();

        else
            Swing();
    }

    private void Stab()
    {
        _currentAngleMovement +=_stabSpeed * Time.fixedDeltaTime;
        _sword.transform.position = _startStabVector + _currentOrientationVector * _currentAngleMovement;

        if (_currentAngleMovement > 2.0f)
        {
            SetIdle();
            if (_target)
            {
                SwordParry swordParry = _target.GetComponent<SwordParry>();
                swordParry.StartParry(false, null);
            }

        }

        else if (_target && _currentAngleMovement > 0.75f)
        {
            Blocking blocker = _target.GetComponent<Blocking>();
            SwordParry swordParry = _target.GetComponent<SwordParry>();
            if (swordParry && swordParry.IsParrying())
            {
                swordParry.StartParry(true, gameObject);
            }
            else if (blocker.StartHit(_attackStance, _swingDirection, gameObject))
            {
                //attack was succesfully blocked
                SetIdle();
                _animationRef.GetHit();
            }
        }
    }

    private void Swing()
    {
        _currentAngleMovement += _swingSpeed * Time.fixedDeltaTime;
        _sword.transform.Rotate(0.0f, 0.0f, _swingDirection * _swingSpeed * Time.fixedDeltaTime);
        float angle = _sword.transform.rotation.eulerAngles.z;
        float diff = _startSwingAngle + _swingDirection * angle;

        if (_target && _currentAngleMovement > _swingAngle * 1.5f)
        {
            Blocking blocker = _target.GetComponent<Blocking>();
            blocker.StopParryTime();
        }
        else if (_target && _currentAngleMovement > _swingAngle * 0.85f)
        {
            Blocking blocker = _target.GetComponent<Blocking>();
            if (blocker.StartHit(_attackStance, _swingDirection, gameObject))
            {
                SetIdle();
                _animationRef.GetHit();
            }
        }


        if (_currentAngleMovement >= _swingAngle * 2)
        {
            SetIdle();
        }
    }

    //public void GetKnocked()
    //{
    //    SetIdle();
    //    _animationRef.GetHit();
    //}

    private void MoveHitCollider()
    {
        float radius = _targetCollider.radius;
        float orientation = _animationRef.GetOrientation();
        _targetCollider.center = new Vector3(Mathf.Cos(orientation) * radius, Mathf.Sin(orientation) * radius, 0.0f);

        Collider[] hitColliders = Physics.OverlapSphere(_targetCollider.center, _targetCollider.radius, _targetMask);
        bool foundTarget = false; 
        foreach (Collider hitCollider in hitColliders)
        {
            if (_target == null && hitCollider is CharacterController)
            {
                _target = hitCollider.gameObject;
                foundTarget = true;
                break;
            }
            else if (_target != null &&!foundTarget && hitCollider is not CharacterController)
                _target = null;
        }
        if (hitColliders.Length == 0)
            _target = null; 
    }

    public void SetIdle()
    {
        float orientationDegree = (_animationRef) ? _animationRef.GetOrientation() * Mathf.Rad2Deg : 0.0f;

        _isSwinging = false;
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, _defaultAngle + orientationDegree -90f);
        _sword.transform.position = _defaultPosition;
        _currentAngleMovement = 0.0f;
    }

    private void PointToTarget()
    {
        float orientationDegree = (_animationRef) ? _animationRef.GetOrientation() * Mathf.Rad2Deg : 0.0f;
        _sword.transform.rotation = Quaternion.Euler(_anglePerspective, 0.0f, orientationDegree + _defaultAngle - 90f);        

    }

    private void AdjustHeightPerspective()
    {
        float orientationDegree = (_animationRef) ? _animationRef.GetOrientation() * Mathf.Rad2Deg : 0.0f;
        
        if (orientationDegree > -45 && orientationDegree < 45)
        {
            _sword.transform.Rotate(_anglePerspective, 0f, 0f);        
            
        }
    }


}