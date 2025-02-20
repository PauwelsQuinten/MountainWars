using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum AttackStance
{
    Head,
    Torso,
    Legs
}

public enum AttackType
{
    UpperSlashRight,
    UpperSlashLeft,
    DownSlashRight,
    DownSlashLeft,
    HorizontalSlash,
    StraightUp,
    StraightDown,
    Stab
}

public enum MovingDirection
{
    MovingUp,
    MovingDown,
    Neutral
}

public class AimingInput : MonoBehaviour
{
    //[SerializeField] private InputActionReference _txtActionPower;
    private AimInputFull _input;
    private InputAction AimAction;
    private InputAction moveAction;
    private InputAction AimHead;
    private InputAction rightBlockAction;
    private InputAction AimFeet;
    private CharacterController characterController;
    [SerializeField] GameObject arrow;

    private Vector2 _direction;
    private Vector2 _moveDirection;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;

    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;
    private AttackStance stanceState = AttackStance.Torso;

    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private TextMeshPro _texMessage;
    [SerializeField] private TextMeshPro _txtActionPower;

    private float _chargeUpTime = 0.0f;
    private float longestWindup = 0;
    private float _currentReleaseTime = 0.0f;
    private const float MAX_RELEASE_TIME = 0.5f; 
    private const float MIN_WINDUP_LENGTH = 0.15f;
    private const float MIN_CHARGEUP_TIME = 0.2f;
    private bool _isStab = false;
    private bool _isExhausted = false;
    private float _restTime = 0.0f;

    //extra state for second prototype
    [SerializeField] GameObject _sword;
    [SerializeField] float radius = 10.0f;
    private Vector2 _startLocation = Vector2.zero;
    private float _chargedTime = 0.0f;
    private (float,float) _chargeZone = (-2.0f, -1.0f);
    private float _currentAttackTime = 0.0f;
    private const float MAX_ATTAK_TIME = 2.0f;
    private float defaultPower = 5.0f;
    private const float MAX_CHARGE_TIME = 5.0f;
    private float _idleTime = 0.0f;
    private const float MAX_Idle_TIME = 0.150f;
    public float DEFAULT_SWORD_ORIENTATION = 218.0f;
    public float YRotation = 45.0f;
    //private const float DEFAULT_SWORD_ORIENTATION = 218.0f;
    [SerializeField] private List<GameObject> _hitZones;
    private bool _isSlash = true;

    //extra for analog3
    private const float MAX_HITBOX_HEIGHT = 4.0f;
    private const float MIN_HITBOX_HEIGHT = 2.0f;
    private AttackType _newAttackType = AttackType.Stab;
    private int _startDirection = 0;
    private MovingDirection _MovingState = MovingDirection.Neutral;
    [SerializeField] private float _leaningSpeed = 1.5f;
    [SerializeField] private float _defaultHitzoneHeight = 0.0f;
    [SerializeField] private float _MaxTimeNotLeaning = 0.2f;
    private float _currentTimeNotLeaning = 0.0f;

    private void Awake()
    {
        _input = new AimInputFull();
    }

    private void Start()
    {
        _startLocation = _sword.transform.position; 

        foreach (var hitZone in _hitZones)
        {
            hitZone.SetActive(false);
        }
    }

    private void OnEnable()
    {
        //_input.Player.Move.performed += OnMove;
        //_input.Player.Move.canceled += OnMove;
        _input.Enable();
        characterController = GetComponent<CharacterController>();
        moveAction = _input.FindAction("Move");
        AimAction = _input.FindAction("Look");
        AimHead = _input.Player.Stab1;
        AimFeet = _input.Player.RightBlock;
        //rightBlockAction = _input.Player.RightBlock;
    }

    private void OnDisable()
    {
        //_input.Player.Move.performed -= OnMove;
       // _input.Player.Move.canceled -= OnMove;
        _input.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        //_direction = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        //analog attack in 8 direction in 3 zones (24 attacks)
        //AnalogAiming();

        //analog attack 2 direction plus charge
        //AnalogAiming2();
        
        //as above but with moveable hitzone, achieve diagonal attacks by moving it up down during attack
        //AnalogAiming3();

        //as above but with rewarding big slash
        AnalogAiming4();

        //Movement
        _moveDirection = moveAction.ReadValue<Vector2>();
        characterController.Move(_moveDirection * _speed * Time.deltaTime);
    }

    private void AnalogAiming4()
    {

        //get angle
        _direction = AimAction.ReadValue<Vector2>();
        float newLength = _direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_direction.y, _direction.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //Debug.Log($"{newLength:F2}");

        //Check attackStance and lock it when attacking
        if (newLength < 0.6f)
        {
            
            if (AimHead.IsPressed() && _hitZones[6].transform.position.y <= MAX_HITBOX_HEIGHT)
            {
                _hitZones[6].transform.position += Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZones[6].transform.position.y >= MAX_HITBOX_HEIGHT)
                    _hitZones[6].transform.position = new Vector3(0.0f, MAX_HITBOX_HEIGHT, 0.0f);

                _MovingState = MovingDirection.MovingUp;
            }
            else if (AimFeet.IsPressed() && _hitZones[6].transform.position.y >= MIN_HITBOX_HEIGHT)
            {
                _hitZones[6].transform.position -= Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZones[6].transform.position.y <= MIN_HITBOX_HEIGHT)
                    _hitZones[6].transform.position = new Vector3(0.0f, MIN_HITBOX_HEIGHT, 0.0f);

                _MovingState = MovingDirection.MovingDown;
            }
            //Fall back to default after a time of not pressing
            else
            {
                _newAttackType = (_newAttackType == AttackType.HorizontalSlash) ? AttackType.HorizontalSlash : AttackType.Stab;
                _MovingState = MovingDirection.Neutral;

                if (_currentTimeNotLeaning < _MaxTimeNotLeaning)
                {
                    _currentTimeNotLeaning += Time.deltaTime;
                }
                else
                {
                    int sign = 0;
                    float diff = _hitZones[6].transform.position.y - _defaultHitzoneHeight;
                    if (Mathf.Abs(diff) > 0.1f)
                    {
                        sign = (diff > 0) ? 1 : -1;
                        _hitZones[6].transform.position += Vector3.down * sign * Time.deltaTime * _leaningSpeed;
                    }
                    else
                    {
                        _hitZones[6].transform.position = new Vector3(0.0f, _defaultHitzoneHeight, 0.0f); 
                        _currentTimeNotLeaning = 0.0f;

                    }


                }
            }
        }

        ////retract sword when power is gone
        //if (_isExhausted && newLength > 0)
        //{
        //    return;
        //}
        //else
        //{
        //    _isExhausted = false;
        //}

        //Reset values when idle to long (cant stay charged up when staying in center position)
        if (newLength < MIN_WINDUP_LENGTH && _chargedTime < MAX_Idle_TIME)
        {
            _idleTime += Time.deltaTime;
            _chargedTime = (_idleTime >= MAX_Idle_TIME) ? 0.0f : _chargedTime;
        }

        //Start moving analog , Attack or Charge up
        if ((newLength > MIN_WINDUP_LENGTH || _chargedTime > 0))
        {
            _idleTime = 0.0f;

            //Charging
            if (currentAngle > _chargeZone.Item1 && currentAngle < _chargeZone.Item2)
            {
                if (_chargedTime < MAX_CHARGE_TIME)
                    _chargedTime += (Time.deltaTime * 4.0f);
                //Debug.Log($"{_chargedTime}");
                _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, DEFAULT_SWORD_ORIENTATION);
                _sword.transform.localPosition = _startLocation;
                _txtActionPower.enabled = false;

            }
            //Attacking
            else
            {
                if (_startDirection == 0)
                    _startDirection = (_direction.x > 0.0f)? 1 : -1;

                //slashDirection or stab
                if ((currentAngleDegree > 110.0f || currentAngleDegree < 70.0f) && newLength > 0.85f)
                {
                    _newAttackType = (_newAttackType == AttackType.Stab)? AttackType.HorizontalSlash : _newAttackType;

                    switch(_MovingState)
                    {
                        case MovingDirection.MovingUp:
                            if (_hitZones[6].transform.position.y >= MAX_HITBOX_HEIGHT)
                                _newAttackType = AttackType.HorizontalSlash;
                            else if (_startDirection < 0 )
                                _newAttackType = AttackType.UpperSlashLeft;
                            else if (_startDirection > 0 )
                                _newAttackType = AttackType.UpperSlashRight;
                            else 
                                _newAttackType = AttackType.StraightUp;
                            break;

                        case MovingDirection.MovingDown:
                            if (_hitZones[6].transform.position.y <= MIN_HITBOX_HEIGHT)
                                _newAttackType = AttackType.HorizontalSlash;
                            else if (_startDirection < 0 )
                                _newAttackType = AttackType.DownSlashLeft;
                            else if (_startDirection > 0 )
                                _newAttackType = AttackType.DownSlashRight;
                            else 
                                _newAttackType = AttackType.StraightDown;
                            break;
                        default:
                            break;

                    }

                }

                ////Whenmoving a stab up and down, change it to upper/down slash
                //if (_MovingState == MovingDirection.MovingUp && _newAttackType == AttackType.Stab)
                //    _newAttackType = AttackType.StraightUp;
                // if (_MovingState == MovingDirection.MovingDown && _newAttackType == AttackType.Stab)
                //    _newAttackType = AttackType.StraightDown;


                //Show hitZone
                _hitZones[6].SetActive(true);
                
                switch(_newAttackType)
                {
                    case AttackType.HorizontalSlash:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        break;
                    case AttackType.DownSlashRight:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                        break;
                    case AttackType.UpperSlashRight:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                        break;
                    case AttackType.DownSlashLeft:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                        break;
                    case AttackType.UpperSlashLeft:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                        break;
                    case AttackType.Stab:
                        _hitZones[6].transform.localScale = new Vector3(0.45f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        break;
                    case AttackType.StraightUp:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    case AttackType.StraightDown:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    default:break;
                }


                //decrease power the longer your arm is stretched out in front
                if ((currentAngleDegree < 110.0f && currentAngleDegree > 70.0f) || newLength < MIN_WINDUP_LENGTH)
                {
                    defaultPower -= (defaultPower > 0) ? (Time.deltaTime * 9.0f) : 0.0f;
                    _chargedTime -= (_chargedTime > 0) ? (Time.deltaTime * 4.0f) : 0.0f;
                }

                //Sword follows analog -> visualization 
                _sword.transform.localPosition = new Vector3(_direction.x * radius, _direction.y * radius, 0.0f);
                _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngleDegree + DEFAULT_SWORD_ORIENTATION - 90.0f);

                //message -> visualization
                _txtActionPower.enabled = true;
                _txtActionPower.text = (defaultPower + _chargedTime).ToString();

                if (defaultPower <= 0)
                {
                    _isExhausted = true;
                    ResetAnalog2();
                }
            }
        }
        else
        {
            ResetAnalog2();

        }
        //Force direction to be correct on idle
        if (newLength < MIN_WINDUP_LENGTH)
        {
            _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }


    }
    private void AnalogAiming3()
    {

        //get angle
        _direction = AimAction.ReadValue<Vector2>();
        float newLength = _direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_direction.y, _direction.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //Debug.Log($"{newLength:F2}");

        //Check attackStance
        if (newLength < 0.6f)
        {
            //if (AimHead.IsPressed() && _hitZones[6].transform.position.y < MAX_HITBOX_HEIGHT  )
            //{
            //    _hitZones[6].transform.position += Vector3.up * Time.deltaTime;
            //    _newAttackType = AttackType.UpperSlashRight;
            //}
            //else if (AimFeet.IsPressed() && _hitZones[6].transform.position.y > MIN_HITBOX_HEIGHT)
            //{
            //    _hitZones[6].transform.position -= Vector3.up * Time.deltaTime;
            //    _newAttackType = AttackType.DownSlashRight;
            //}
            //else
            //    _newAttackType = (_newAttackType == AttackType.HorizontalSlash) ? AttackType.HorizontalSlash : AttackType.Stab;

            if (AimHead.IsPressed() && _hitZones[6].transform.position.y < MAX_HITBOX_HEIGHT)
            {
                _hitZones[6].transform.position += Vector3.up * Time.deltaTime;
                _MovingState = MovingDirection.MovingUp;
            }
            else if (AimFeet.IsPressed() && _hitZones[6].transform.position.y > MIN_HITBOX_HEIGHT)
            {
                _hitZones[6].transform.position -= Vector3.up * Time.deltaTime;
                _MovingState = MovingDirection.MovingDown;
            }
            else
            {
                _newAttackType = (_newAttackType == AttackType.HorizontalSlash) ? AttackType.HorizontalSlash : AttackType.Stab;
                _MovingState = MovingDirection.Neutral;
            }
        }
        //retract sword when power is gone
        if (_isExhausted && newLength > 0)
        {
            return;
        }
        else
        {
            _isExhausted = false;
        }

        //Reset values when idle to long
        if (newLength < MIN_WINDUP_LENGTH && _chargedTime < MAX_Idle_TIME)
        {
            _idleTime += Time.deltaTime;
            _chargedTime = (_idleTime >= MAX_Idle_TIME) ? 0.0f : _chargedTime;
        }

        //Start moving analog or use your charged power to attack
        if ((newLength > MIN_WINDUP_LENGTH || _chargedTime > 0))
        {
            _idleTime = 0.0f;

            //Charging
            if (currentAngle > _chargeZone.Item1 && currentAngle < _chargeZone.Item2)
            {
                if (_chargedTime < MAX_CHARGE_TIME)
                    _chargedTime += (Time.deltaTime * 4.0f);
                //Debug.Log($"{_chargedTime}");
                _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, DEFAULT_SWORD_ORIENTATION);
                _sword.transform.localPosition = _startLocation;
                _txtActionPower.enabled = false;

            }
            //Attacking
            else
            {
                if (_startDirection == 0)
                    _startDirection = (_direction.x > 0.0f)? 1 : -1;

                //slashDirection or stab
                if ((currentAngleDegree > 110.0f || currentAngleDegree < 70.0f) && newLength > 0.85f)
                {
                    //_isSlash = true;
                    _newAttackType = (_newAttackType == AttackType.Stab)? AttackType.HorizontalSlash : _newAttackType;

                    //if (_startDirection < 0 && _newAttackType == AttackType.UpperSlashRight)
                    //    _newAttackType = AttackType.UpperSlashLeft;
                    //
                    //if (_startDirection < 0 && _newAttackType == AttackType.DownSlashRight)
                    //    _newAttackType = AttackType.DownSlashLeft;
                    
                    switch(_MovingState)
                    {
                        case MovingDirection.MovingUp:
                            if (_startDirection < 0 )
                                _newAttackType = AttackType.UpperSlashLeft;
                            else if (_startDirection > 0 )
                                _newAttackType = AttackType.UpperSlashRight;
                            else 
                                _newAttackType = AttackType.StraightUp;
                            break;

                        case MovingDirection.MovingDown:
                            if (_startDirection < 0 )
                                _newAttackType = AttackType.DownSlashLeft;
                            else if (_startDirection > 0 )
                                _newAttackType = AttackType.DownSlashRight;
                            else 
                                _newAttackType = AttackType.StraightDown;
                            break;
                        default:
                            break;

                    }

                }

                //Whenmoving a stab up and down, change it to upper/down slash
                if (_MovingState == MovingDirection.MovingUp && _newAttackType == AttackType.Stab)
                    _newAttackType = AttackType.StraightUp;
                 if (_MovingState == MovingDirection.MovingDown && _newAttackType == AttackType.Stab)
                    _newAttackType = AttackType.StraightDown;


                //Show hitZone
                _hitZones[6].SetActive(true);
                //_hitZones[6].transform.localScale = (_newAttackType == AttackType.HorizontalSlash) ? new Vector3(1.4f, 0.45f, 0.0f) : new Vector3(0.45f, 0.45f, 0.0f);
                
                switch(_newAttackType)
                {
                    case AttackType.HorizontalSlash:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        break;
                    case AttackType.DownSlashRight:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                        break;
                    case AttackType.UpperSlashRight:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                        break;
                    case AttackType.DownSlashLeft:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                        break;
                    case AttackType.UpperSlashLeft:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                        break;
                    case AttackType.Stab:
                        _hitZones[6].transform.localScale = new Vector3(0.45f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        break;
                    case AttackType.StraightUp:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    case AttackType.StraightDown:
                        _hitZones[6].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                        _hitZones[6].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                    default:break;
                }
           

                //decrease power the longer your arm is stretched out
                defaultPower -= (defaultPower > 0) ? (Time.deltaTime * 6.0f) : 0.0f;
                _chargedTime -= (_chargedTime > 0) ? (Time.deltaTime * 4.0f) : 0.0f;

                //Sword follows analog 
                _sword.transform.localPosition = new Vector3(_direction.x * radius, _direction.y * radius, 0.0f);
                _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngleDegree + DEFAULT_SWORD_ORIENTATION - 90.0f);
                //message
                _txtActionPower.enabled = true;
                _txtActionPower.text = (defaultPower + _chargedTime).ToString();
                if (defaultPower <= 0)
                {
                    _isExhausted = true;
                    ResetAnalog2();
                }
            }
        }
        else
        {
            ResetAnalog2();

        }
        if (newLength < MIN_WINDUP_LENGTH)
        {
            _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }


    }
     private void AnalogAiming2()
    {
        //Check attackStance
        if (AimHead.IsPressed())
            stanceState = AttackStance.Head;
        else if (AimFeet.IsPressed())
            stanceState = AttackStance.Legs;
        else
            stanceState = AttackStance.Torso;

        //get angle
        _direction = AimAction.ReadValue<Vector2>();
        float newLength = _direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_direction.y, _direction.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //Debug.Log($"{newLength:F2}");

        //Track movement
        if (_isExhausted && newLength > 0)
        {
            return;
        }
        else
        {
            _isExhausted = false;
        }

        //Reset values when idle to long
        if (newLength < MIN_WINDUP_LENGTH && _chargedTime < MAX_Idle_TIME)
        {
            _idleTime += Time.deltaTime;
            _chargedTime = (_idleTime >= MAX_Idle_TIME) ? 0.0f : _chargedTime;
        }
        
        if ((newLength > MIN_WINDUP_LENGTH || _chargedTime > 0))
        {
            _idleTime = 0.0f;

            

            if (currentAngle > _chargeZone.Item1 && currentAngle < _chargeZone.Item2)
            {
                if (_chargedTime < MAX_CHARGE_TIME)
                    _chargedTime += (Time.deltaTime * 4.0f);
                //Debug.Log($"{_chargedTime}");
                _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, DEFAULT_SWORD_ORIENTATION);
                _sword.transform.localPosition = _startLocation;
                _txtActionPower.enabled = false;

            }
            else 
            {
                if ((currentAngleDegree > 110 || currentAngleDegree < 70) && newLength > 0.85f)
                {
                    _isSlash = true;
                }

                int index = 0;
                //Show hitZone
                switch(stanceState)
                {
                    case AttackStance.Head:
                        index = _isSlash ? 0 : 3;
                        break;
                    case AttackStance.Torso:
                        index = _isSlash ? 1 : 4;
                        break;
                    case AttackStance.Legs:
                        index = _isSlash ? 2 : 5;
                        break;

                }
                _hitZones[index].SetActive(true);
               

                //decrease power the longer your arm is stretched out
                defaultPower -= (defaultPower > 0)? (Time.deltaTime * 6.0f) : 0.0f;
                _chargedTime -= (_chargedTime > 0)? (Time.deltaTime * 4.0f) : 0.0f;

                //Sword follows analog 
                _sword.transform.localPosition = new Vector3(_direction.x * radius, _direction.y * radius, 0.0f);
                _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngleDegree + DEFAULT_SWORD_ORIENTATION - 90.0f);
                //message
                _txtActionPower.enabled = true;
                _txtActionPower.text = (defaultPower + _chargedTime).ToString();
                if (defaultPower <= 0)
                {
                    _isExhausted = true;
                    ResetAnalog2();
                }
            }
        }
        else
        {
            ResetAnalog2 ();

        }
        if (newLength < MIN_WINDUP_LENGTH)
        {
            _sword.transform.rotation= Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }

        Debug.Log($"{_isSlash}");

    }

    private void ResetAnalog2()
    {
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        _sword.transform.localPosition = _startLocation;
        _chargedTime = 0.0f;
        defaultPower = 5.0f;
        _txtActionPower.enabled = false;
        //_isSlash = false;
        _startDirection = 0;
        _newAttackType = AttackType.Stab;
        foreach (var hitZone in _hitZones)
        {
            hitZone.SetActive(false);
        }
    }

    private void AnalogAiming()
    {
        arrow.transform.localScale *= (0.990f);

        //Check attackStance
        if (AimHead.IsPressed())
            stanceState = AttackStance.Head;
        else if (AimFeet.IsPressed())
            stanceState = AttackStance.Legs;
        else
            stanceState = AttackStance.Torso;
        //Debug.Log($" Stance: {stanceState}");

        _direction = AimAction.ReadValue<Vector2>();
        float newLength = _direction.SqrMagnitude();
        //Debug.Log($" length: {newLength}");

        switch (state)
        {
            case SlashState.Windup:
                //Move analog up
                if (longestWindup > MIN_WINDUP_LENGTH)
                    _chargeUpTime += (_chargeUpTime >= 1.0f)? 0.0f : Time.deltaTime;

                if (newLength >= longestWindup)
                {
                    longestWindup = newLength;
                    loadDirection = _direction;
                    //Debug.Log($"power {longestWindup} ");

                }
                   
                //Move analog down
                else if (newLength < MIN_WINDUP_LENGTH && longestWindup > MIN_WINDUP_LENGTH)
                {
                    slashState = FindSlashState();
                    if (_chargeUpTime < MIN_CHARGEUP_TIME && longestWindup >= 0.9f)
                    {
                        _isStab = true;
                        slashState = ((int)slashState <= 0) ? slashState + 180 : slashState - 180;
                        Debug.Log($"Hight : {stanceState} in direction {slashState} with power: {_chargeUpTime:F2}");
                        Attack();
                        return;
                    }
                    _isStab = false;
                    state = SlashState.Release;
                }
                break;

            case SlashState.Release:
                if (TimeOut())
                {
                    state = SlashState.Rest;
                    return;
                }
                //if (newLength > longestWindup)
                //{
                //    longestWindup = newLength;
                //    //Debug.Log($"ready to release ");
                //}
                //else if (newLength < longestWindup)
                else if (newLength > 0.9f)
                {
                    slashDirection = _direction;
                    //Check if load and release angle is correct
                    float angle = Vector2.Angle(loadDirection, slashDirection);
                    float acceptedAngle = 60.0f;
                    //float acceptedAngle = ((int)slashState % 90 == 0) ? 140.0f : 110.0f;
                    if (angle < acceptedAngle)
                    {
                        //Fail
                        state = SlashState.Rest;
                        Debug.Log($"Fail, angle{angle}");
                        return;
                    }
                     //SLASH!!!!
                    Debug.Log($"{stanceState} in direction {slashState} with power: {_chargeUpTime}");
                    Attack();

                }
                break;

            case SlashState.Rest:
                //Debug.Log($"Reset");
                _restTime = (_restTime <= 0.0f)? 0.0f : _restTime - Time.deltaTime;
                if (newLength < MIN_WINDUP_LENGTH && _restTime <= 0.0f)
                {
                    state = SlashState.Windup;
                    longestWindup = 0;
                    loadDirection = Vector2.zero;
                    slashDirection = Vector2.zero;
                    _currentReleaseTime = 0;
                    _chargeUpTime = 0;
                }
                break;
        }


        //Debug.Log($"direction: {_direction}");
    }

    private void Attack()
    {
        RotateArrow();
        state = SlashState.Rest;
        _restTime = 0.25f;
    }

    private SlashDirection FindSlashState()
    {
        SlashDirection slash = new SlashDirection();

        //float angle = Vector2.Angle(loadDirection, Vector2.right);
        float angle = Mathf.Atan2(loadDirection.y, loadDirection.x) * Mathf.Rad2Deg;
        int enumValue = Mathf.RoundToInt(angle / 45) * 45;
        enumValue = (enumValue == -180) ? 180 : enumValue;
        slash = (SlashDirection)enumValue;
        return slash;
    }

    private bool TimeOut()
    {
        _currentReleaseTime += Time.deltaTime;
        if (_currentReleaseTime >= MAX_RELEASE_TIME)
        {
            _currentReleaseTime = 0;
            return true;
        }
        return false;

    }

    private void RotateArrow()
    {
        arrow.transform.localScale = (Vector3.one * 0.2f);

        string attack = _isStab ? "Stab" : "Slash";
        _texMessage.text = slashState.ToString();
        string text = $"{attack} Aim at {stanceState} with power: {_chargeUpTime:F2}";
        _txtActionPower.text = text;
        switch (slashState)
        {
            case SlashDirection.LeftUp:
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;
            case SlashDirection.RightUp: 
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;     
            case SlashDirection.LeftDown:
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;
            case SlashDirection.RightDown: 
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;     
            case SlashDirection.LeftToRight:
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;
            case SlashDirection.RightToLeft: 
                arrow.transform.rotation = Quaternion.Euler(0, 0, (int)slashState + 180);
                break;     
            case SlashDirection.StraightDown:
                arrow.transform.rotation = Quaternion.Euler(0, 0, -(int)slashState);
                break;
            case SlashDirection.Upper:
                arrow.transform.rotation = Quaternion.Euler(0, 0, -(int)slashState);
                break;
            default:
                break;
        }
        

    }
    
}