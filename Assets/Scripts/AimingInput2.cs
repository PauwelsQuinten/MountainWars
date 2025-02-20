using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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
    HorizontalSlashLeft,
    HorizontalSlashRight,
    StraightUp,
    StraightDown,
    Stab,
    None
}

public enum MovingDirection
{
    MovingUp,
    MovingDown,
    Neutral
}

public class AimingInput2 : MonoBehaviour
{
    //[SerializeField] private InputActionReference _txtActionPower;
    [SerializeField]
    private InputActionReference AimAction;
    [SerializeField]
    private InputActionReference _aimHead;
    [SerializeField]
    private InputActionReference _aimFeet;
    [SerializeField]
    private InputActionReference _aimTorso;
    [SerializeField]
    private InputActionReference _slashUp;
    [SerializeField]
    private InputActionReference _slashDown;
    private InputAction rightBlockAction;
    private CharacterController characterController;
    [SerializeField] GameObject arrow;

    private Vector2 _direction;
    private Vector2 _moveDirection;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;

    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;
    private AttackStance _currentStanceState = AttackStance.Torso;
    private AttackStance _previousStance = AttackStance.Torso;
    private AttackType _currentAttackType = AttackType.None;
    private AttackType _previousAttack = AttackType.None;

    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private TextMeshPro _texMessage;
    [SerializeField] private TextMeshPro _txtActionPower;
    [SerializeField] private TextMeshPro _AttackMessage;

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
    [SerializeField] GameObject _arrow;
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
    private int _startDirection = 0;
    private MovingDirection _MovingState = MovingDirection.Neutral;

    private Vector2 _startDrawPos;
    private float _slashAngle;
    private float _slashTime;
    [SerializeField]
    private float _slashStrength;

    [SerializeField]
    private float _leaningSpeed = 2f;
    [SerializeField]
    private float _MaxTimeNotLeaning = 0.5f;

    private float _currentTimeNotLeaning;
    private float _defaultHitzoneHeight = 3.3f;

    [SerializeField]
    private GameObject _leanIndicator;

    private bool _isHeightLocked = false;
    private int _currentHitBoxIndex;
    private int _currentStanceIndex;
    private List<AttackType> _possibleAttacks = new List<AttackType>();
    //private List<AttackType> _OverComitAttacks = new List<AttackType>();
    private bool _isAttackSet;
    private bool _changedStanceThisAction;
    private bool _hasOverCommited;
    private CharacterMovement _characterOrientation;


    private void Start()
    {
        _characterOrientation = GetComponent<CharacterMovement>();
        _startLocation = _sword.transform.position; 

        foreach (var hitZone in _hitZones)
        {
            hitZone.SetActive(false);
        }
    }

    private void OnEnable()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        AnalogAiming4();
    }

    private void AnalogAiming4()
    {
        //get angle
        _direction = AimAction.action.ReadValue<Vector2>();
        float newLength = _direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_direction.y, _direction.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;

        //SetHitboxHeight(newLength);

        //Reset values when idle to long (cant stay charged up when staying in center position)
        if (newLength < MIN_WINDUP_LENGTH && _chargedTime < MAX_Idle_TIME)
        {
            _idleTime += Time.deltaTime;
            _chargedTime = (_idleTime >= MAX_Idle_TIME) ? 0.0f : _chargedTime;
        }

        //Start moving analog , Attack or Charge up
        if ((newLength > MIN_WINDUP_LENGTH))
        {
            _idleTime = 0.0f;

            //Charging
            Chargepower(currentAngle);
            //Attacking
            SetSwingDirection();

            //slashDirection or stab
            SetAttackType(newLength, currentAngleDegree);

            CalculateAttackPower(newLength);

            SetHitboxAngle();

            //decrease power the longer your arm is stretched out in front
            if ((currentAngleDegree < 110.0f && currentAngleDegree > 70.0f) || newLength < MIN_WINDUP_LENGTH)
            {
                defaultPower -= (defaultPower > 0) ? (Time.deltaTime * 9.0f) : 0.0f;
                _chargedTime -= (_chargedTime > 0) ? (Time.deltaTime * 4.0f) : 0.0f;
            }
        }
        else
        {
            ResetValues();
        }
        //Force direction to be correct on idle
        if (newLength < MIN_WINDUP_LENGTH)
        {
            _sword.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }
        SwordVisual(currentAngleDegree);
    }

    private void ResetValues()
    {
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        _sword.transform.localPosition = _startLocation;
        _chargedTime = 0.0f;
        defaultPower = 5.0f;
        _txtActionPower.enabled = false;
        _startDirection = 0;
        _currentAttackType = AttackType.Stab;
        foreach (var hitZone in _hitZones)
        {
            hitZone.SetActive(false);
        }
        _arrow.SetActive(false);
        _isAttackSet = false;
    }

    private void SetHitboxHeight(float length)
    {
        _leanIndicator.transform.position = _hitZones[6].transform.position;

        if (length < 0.6f)
        {
            _aimHead.action.performed += HeightChange_performed;
            _aimFeet.action.performed += HeightChange_performed;
            if (_aimHead.action.IsPressed())
            {
                if (_hitZones[6].transform.position.y >= MAX_HITBOX_HEIGHT || _isHeightLocked) return;

                //float zoneHeight = Mathf.Abs(MAX_HITBOX_HEIGHT - _defaultHitzoneHeight);
                //_hitZones[6].transform.position = new Vector3(0, _defaultHitzoneHeight + (zoneHeight * _aimHead.action.ReadValue<float>()), 0);

                _hitZones[6].transform.position += Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZones[6].transform.position.y >= MAX_HITBOX_HEIGHT)
                    _hitZones[6].transform.position = new Vector3(0.0f, MAX_HITBOX_HEIGHT, 0.0f);

                _MovingState = MovingDirection.MovingUp;
            }
            else if (_aimFeet.action.IsPressed())
            {
                if (_hitZones[6].transform.position.y <= MIN_HITBOX_HEIGHT || _isHeightLocked) return;

                //float zoneHeight = Mathf.Abs(MIN_HITBOX_HEIGHT - _defaultHitzoneHeight);
                //_hitZones[6].transform.position = new Vector3(0, _defaultHitzoneHeight - (zoneHeight * _aimFeet.action.ReadValue<float>()), 0);

                _hitZones[6].transform.position -= Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZones[6].transform.position.y <= MIN_HITBOX_HEIGHT)
                    _hitZones[6].transform.position = new Vector3(0.0f, MIN_HITBOX_HEIGHT, 0.0f);

                _MovingState = MovingDirection.MovingDown;
            }
            //else if (_aimFeet.action.ReadValue<float>() == 0 && _aimHead.action.ReadValue<float>() == 0 && _hitZones[6].transform.position.y != _defaultHitzoneHeight)
            //{
            //    if (_hitZones[6].transform.position.y > _defaultHitzoneHeight)
            //        _hitZones[6].transform.position = new Vector3(0, _hitZones[6].transform.position.y - 10 * Time.deltaTime, 0);
            //    else if (_hitZones[6].transform.position.y < _defaultHitzoneHeight) return;
            //    _hitZones[6].transform.position = new Vector3(0, _hitZones[6].transform.position.y + 10 * Time.deltaTime, 0);
            //}

            //Fall back to default after a time of not pressing
            else
            {
                if (_isHeightLocked) return;
                _currentAttackType = (_currentAttackType == AttackType.Stab) ? AttackType.HorizontalSlashLeft : AttackType.Stab;
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
    }

    private void SetStance()
    {
        //_previousStance = _currentStanceState;
        //if (_previousAttack == AttackType.StraightUp ||
        //     _previousAttack == AttackType.UpperSlashRight ||
        //     _previousAttack == AttackType.UpperSlashLeft)
        //{
        //    if (_currentStanceState == AttackStance.Legs)
        //    {
        //        _currentStanceState = AttackStance.Hips;
        //        _ChangedStanceThisAction = true;
        //    }
        //    else if(_currentStanceState == AttackStance.Hips)
        //    {
        //        _currentStanceState = AttackStance.Shoulders;
        //        _ChangedStanceThisAction = true;
        //    }
        //    else if (_currentStanceState == AttackStance.Shoulders)
        //    {
        //        _currentStanceState = AttackStance.Head;
        //        _ChangedStanceThisAction = true;
        //    }
        //}

        //if (_previousAttack == AttackType.StraightDown ||
        //     _previousAttack == AttackType.DownSlashLeft ||
        //     _previousAttack == AttackType.DownSlashRight)
        //{
        //    if (_currentStanceState == AttackStance.Head)
        //    {
        //        _currentStanceState = AttackStance.Shoulders;
        //        _ChangedStanceThisAction = true;
        //    }
        //    else if (_currentStanceState == AttackStance.Shoulders)
        //    {
        //        _currentStanceState = AttackStance.Hips;
        //        _ChangedStanceThisAction = true;
        //    }
        //    else if (_currentStanceState == AttackStance.Hips) 
        //    {
        //        _currentStanceState = AttackStance.Legs;
        //        _ChangedStanceThisAction = true;
        //    }
        //}

        //if (_previousAttack == AttackType.None)
        //{
        //    _currentStanceState = AttackStance.Hips;
        //} 
        _aimFeet.action.performed += AimFeet_performed;
        _aimTorso.action.performed += AimTorso_performed;
        _aimHead.action.performed += AimHead_performed;
        //switch (_currentStanceIndex)
        //{
        //    case 0:
        //        _currentStanceState = AttackStance.Legs;
        //        break;
        //    case 1:
        //        _currentStanceState = AttackStance.Torso;
        //        break;
        //    case 2:
        //        _currentStanceState = AttackStance.Head;
        //        break;
        //}

        int index = 0;
        //Show hitZone
        switch (_currentStanceState)
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
        _currentHitBoxIndex = index;
        _hitZones[index].SetActive(true);
        _arrow.SetActive(true);
        //if (_previousAttack != AttackType.None)
        //{
        //    _hitZones[index].SetActive(true);
        //    _arrow.SetActive(true);
        //}

        SetHitboxAngle();
    }

    private void SetAttackType(float drawLength, float angle)
    {
        if (_isAttackSet) return;

        if (angle > 110.0f || angle < 70.0f)
        {
            //_currentAttackType = (_currentAttackType == AttackType.Stab) ? AttackType.HorizontalSlashLeft : _currentAttackType;
            _currentAttackType = AttackType.HorizontalSlashLeft;
            _isAttackSet = true;
        }
        else if(!_isAttackSet)
        {
            _currentAttackType = AttackType.Stab;
            _isAttackSet = true;
        }
        SetSlashType();
        SetStance();
    }
    private void SetSlashType()
    {
        switch (_startDirection)
        {
            case -1:
                if (_slashDown.action.IsPressed())
                {
                    _currentAttackType = AttackType.DownSlashRight;
                    if (_currentAttackType == AttackType.Stab) _currentAttackType = AttackType.StraightDown;
                    _isAttackSet = true;
                }
                else if (_slashUp.action.IsPressed())
                {
                    _currentAttackType = AttackType.UpperSlashRight;
                    if (_currentAttackType == AttackType.Stab) _currentAttackType = AttackType.StraightUp;
                    _isAttackSet = true;
                }
                if (_currentAttackType == AttackType.HorizontalSlashLeft) 
                {
                    _currentAttackType = AttackType.HorizontalSlashRight;
                    _isAttackSet = true;
                }
                break;
            case 1:
                if (_slashDown.action.IsPressed())
                {
                    _currentAttackType = AttackType.DownSlashLeft;
                    if (_currentAttackType == AttackType.Stab) _currentAttackType = AttackType.StraightDown;
                    _isAttackSet = true;
                }
                else if (_slashUp.action.IsPressed())
                {
                    _currentAttackType = AttackType.UpperSlashLeft;
                    if (_currentAttackType == AttackType.Stab) _currentAttackType = AttackType.StraightUp;
                    _isAttackSet = true;
                }
                break;
        }
    }

    private void SetHitboxAngle()
    {
        switch (_currentAttackType)
        {
            case AttackType.HorizontalSlashLeft:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f + 180f);
                break;
            case AttackType.HorizontalSlashRight:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case AttackType.DownSlashRight:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                break;
            case AttackType.UpperSlashRight:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                break;
            case AttackType.DownSlashLeft:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f + 180f);
                break;
            case AttackType.UpperSlashLeft:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f - 180f);
                break;
            case AttackType.Stab:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(0.45f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case AttackType.StraightUp:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                break;
            case AttackType.StraightDown:
                _hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
                _hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
                break;
            default: break;
        }
        _arrow.transform.position = _hitZones[_currentHitBoxIndex].transform.position;
        Vector3 arrowAngle = _hitZones[_currentHitBoxIndex].transform.eulerAngles;
        arrowAngle.z -= 90;
        _arrow.transform.rotation = Quaternion.Euler(arrowAngle);
    }

    private void Chargepower(float drawAngle)
    {
        if (drawAngle > _chargeZone.Item1 && drawAngle < _chargeZone.Item2)
        {
            if (_chargedTime < MAX_CHARGE_TIME)
                _chargedTime += (Time.deltaTime * 4.0f);
            _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, DEFAULT_SWORD_ORIENTATION);
            _sword.transform.localPosition = _startLocation;
            _txtActionPower.enabled = false;
            return;
        }

        _txtActionPower.enabled = true;
        _txtActionPower.text = (defaultPower + _chargedTime).ToString();
    }

    private void SetSwingDirection()
    {
        if (_startDirection == 0)
            _startDirection = (_direction.x > 0.0f) ? 1 : -1;
    }

    private void CalculateAttackPower(float drawLength)
    {
        bool canRun = false;
        if (drawLength >= 0.97f)
        {
            if (_startDrawPos == Vector2.zero) _startDrawPos = _direction;
            float newAngle = Vector2.Angle(_startDrawPos, _direction);
            canRun = true;
            if (_slashAngle <= newAngle)
            {
                _slashAngle = newAngle;
                _slashTime += Time.deltaTime;
                if(_slashAngle > 175) _hasOverCommited = true;
                _texMessage.text = $"Slash power: {(_slashStrength + (_slashAngle / 100) + _chargedTime) / _slashTime}";
            }
            else
            {
                if (canRun && !_hasOverCommited)
                {
                    CheckAttack();
                    _slashTime = 0.0f;
                    _slashAngle = 0.0f;
                    _startDrawPos = Vector2.zero;
                    canRun = false;
                }
                _isAttackSet = false;
            }
        }
        else if(canRun && !_hasOverCommited)
        {
            CheckAttack();
            _slashTime = 0.0f;
            _slashAngle = 0.0f;
            _startDrawPos = Vector2.zero;
            canRun = false;
        }
        if (drawLength <= MIN_WINDUP_LENGTH)
        {
            _isAttackSet = false;
        }

        if (canRun && _hasOverCommited)
        {
            StopCoroutine(ResetAtackText(0.5f));
            _AttackMessage.text = "Player over commited";
            StartCoroutine(ResetAtackText(0.5f));
            _hasOverCommited = false;
            canRun = false;
        }


    }

    private void CheckAttack()
    {
        GetpossibleAtack();
         foreach(AttackType Possebility in _possibleAttacks) 
        {
            if (_currentAttackType == Possebility)
            {
                _currentAttackType = Possebility;
                SetPreviousAttacks();
                return;
            }
        }
        //foreach (AttackType overCommit in _OverComitAttacks)
        //{
        //    if (_currentAttackType == overCommit)
        //    {
        //        _currentAttackType = AttackType.None;
        //        StopCoroutine(ResetAtackText(0.5f));
        //        _AttackMessage.text = "Player over commited";
        //        StartCoroutine(ResetAtackText(0.5f));
        //        Debug.Log("Player over commited");
        //        SetPreviousAttacks();
        //        return;
        //    }
        //}
        _currentAttackType = AttackType.None;
        StopCoroutine(ResetAtackText(0.5f));
        _AttackMessage.text = "Attack was invalid";
        StartCoroutine(ResetAtackText(0.5f));
        Debug.Log("Attack was invalid!");
        SetPreviousAttacks();
    }

    private void SetPreviousAttacks()
    {
        _previousAttack = _currentAttackType;
    }

    private void GetpossibleAtack()
    {
        _possibleAttacks.Clear();
        //_OverComitAttacks.Clear();
        //AttackStance stanceState = AttackStance.Torso;
        //if (_ChangedStanceThisAction) stanceState = _previousStance;
        //else stanceState = _currentStanceState;

        AttackStance stanceState = _currentStanceState;

        //Debug.Log($"ChangedHeight{_ChangedStanceThisAction} {stanceState}");
        switch (_previousAttack)
        {
            case AttackType.UpperSlashRight:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.UpperSlashLeft:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.DownSlashRight:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.DownSlashLeft:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.HorizontalSlashLeft:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.HorizontalSlashRight:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.StraightUp:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);

                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.StraightDown:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.Stab:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.None);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.None);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.None);
                        break;
                }
                break;
            case AttackType.None:
                switch (stanceState)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
        }
        _changedStanceThisAction = false;
    }

    private void SwordVisual(float angle)
    {
        //Sword follows analog -> visualization 
        _sword.transform.localPosition = new Vector3(_direction.x * radius, _direction.y * radius, 0.0f);
        Vector3 swordRotation = transform.forward * angle;
        swordRotation.z += DEFAULT_SWORD_ORIENTATION - 90f + (int)_characterOrientation.CurrentCharacterOrientation;
        _sword.transform.rotation = Quaternion.Euler(swordRotation);
    }

    private IEnumerator ResetAtackText(float time)
    {
        yield return new WaitForSeconds(time);
        _AttackMessage.text = " ";
    }
    private void AimHead_performed(InputAction.CallbackContext obj)
    {
        _currentStanceState = AttackStance.Head;
        //if(_currentStanceIndex < 2) _currentStanceIndex += 1;
    }

    private void AimTorso_performed(InputAction.CallbackContext obj)
    {
        _currentStanceState = AttackStance.Torso;
    }

    private void AimFeet_performed(InputAction.CallbackContext obj)
    {
         _currentStanceState = AttackStance.Legs;
        //if (_currentStanceIndex > 0) _currentStanceIndex -= 1;
    }

    private void HeightChange_performed(InputAction.CallbackContext obj)
    {
        if (_hitZones[6].transform.position.y > _defaultHitzoneHeight) _isHeightLocked = !_isHeightLocked;
        if (_hitZones[6].transform.position.y < _defaultHitzoneHeight) _isHeightLocked = !_isHeightLocked;
    }
}