using UnityEngine;
using UnityEngine.UIElements;
public class SwordSwing : MonoBehaviour
{
    [SerializeField] GameObject _sword;
    [SerializeField] float _swingSpeed = 25f;
    [SerializeField] float _swingAngle = 90.0f;
    [SerializeField] float _anglePerspective = 71.0f;
    [SerializeField] private GameObject _target;

    private WalkAnimate _animationRef;

    private bool _isSwinging = false;
    private Vector2 _defaultPosition;
    private float  _startSwingAngle = 0.0f;
    private float _swingDirection = 0.0f;
    private float _defaultAngle;
    private float _currentAngleMovement = 0.0f;
    private AttackStance _attackStance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animationRef = GetComponent<WalkAnimate>();
        _defaultAngle = _sword.transform.rotation.eulerAngles.z;
        _defaultPosition = _sword.transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_isSwinging)
        {
            Swing();
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
        float orientationDegree = (_animationRef) ? _animationRef.GetOrientation() * Mathf.Rad2Deg : 0.0f;
        float rotation = startDirection * _swingAngle;

        switch (attackType)
        {
            case AttackType.HorizontalSlashLeft:
            case AttackType.HorizontalSlash:
            case AttackType.HorizontalSlashRight:


                //minus the 90 degree because default is North
                _sword.transform.Rotate(0.0f, 0.0f, rotation);
                _startSwingAngle = _sword.transform.rotation.eulerAngles.z;

                break;
            case AttackType.UpperSlashRight:
            case AttackType.UpperSlashLeft:
                break;
            case AttackType.DownSlashRight:
            case AttackType.DownSlashLeft:
                //minus the 90 degree because default is North
                _sword.transform.Rotate(0.0f, 0.0f, rotation);
                _sword.transform.Rotate(0.0f, rotation, 0.0f);
                _startSwingAngle = _sword.transform.rotation.eulerAngles.z;
                break;
            case AttackType.StraightUp:
            case AttackType.StraightDown:
                break;
            case AttackType.Stab:
                break;
            case AttackType.None:
                break;
        }
    }

    private void Swing()
    {
        _currentAngleMovement += _swingSpeed * Time.fixedDeltaTime;
        _sword.transform.Rotate(0.0f, 0.0f, _swingDirection * _swingSpeed * Time.fixedDeltaTime);
        //_sword.transform.rotation = Quaternion.Euler(_anglePerspective, 0.0f, _sword.transform.rotation.z);
        float angle = _sword.transform.rotation.eulerAngles.z;
        float diff = _startSwingAngle + _swingDirection * angle;

        
        if (_currentAngleMovement > _swingAngle*1.5f)
        {
            Blocking blocker = _target.GetComponent<Blocking>();
            blocker.StopParryTime();
        }

        else if (_currentAngleMovement > _swingAngle*0.85f)
        {
            Blocking blocker = _target.GetComponent<Blocking>();
            blocker.StartParryTime(_attackStance);
        }



        if (_currentAngleMovement >= _swingAngle * 2)
        {
            SetIdle();
        }
        return;
        Debug.Log($"diff: {diff}");
        Debug.Log($"angle: {angle}");
    }

    private void SetIdle()
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