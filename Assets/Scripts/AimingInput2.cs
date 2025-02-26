using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AimingInput2 : MonoBehaviour
{
    public Vector2 Direction;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;

    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;
    public AttackStance CurrentStanceState = AttackStance.Torso;
    private AttackStance _previousStance = AttackStance.Torso;
    public AttackType CurrentAttackType = AttackType.None;
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
    

    //extra state for second prototype
    [SerializeField] private GameObject _sword;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private float radius = 10.0f;
    private Vector2 _startLocation = Vector2.zero;
    private float _chargedTime = 0.0f;
    private (float,float) _chargeZone = (-2.0f * Mathf.Rad2Deg, -1.0f * Mathf.Rad2Deg);
    private float _currentAttackTime = 0.0f;
    private const float MAX_ATTAK_TIME = 2.0f;
    private float defaultPower = 5.0f;
    private const float MAX_CHARGE_TIME = 5.0f;
    private float _idleTime = 0.0f;
    private const float MAX_Idle_TIME = 0.150f;
    public float DEFAULT_SWORD_ORIENTATION = 218.0f;
    public float YRotation = 45.0f;
    //private const float DEFAULT_SWORD_ORIENTATION = 218.0f;
    //[SerializeField] private List<GameObject> _hitZones;
    private bool _isSlash = true;
    private int _startDirection = 0;

    public bool SlashUp;
    public bool SlashDown;

    private Vector2 _startDrawPos;
    private float _slashAngle;
    private float _slashTime;
    [SerializeField]
    private float _slashStrength;

    private int _currentHitBoxIndex;
    private List<AttackType> _possibleAttacks = new List<AttackType>();
    //private List<AttackType> _OverComitAttacks = new List<AttackType>();
    private bool _isAttackSet;

    [SerializeField]
    private float _overCommitAngle = 170f;
    [SerializeField]
    private float _minSlashAngle = 25f;

    private Coroutine _resetAtackText;
    private Coroutine _resetAttackStance;
    private bool _isResetingStance;
    [SerializeField]
    private float _stanceResetTimer;

    private bool _checkFeint;

    private FindPossibleAttacks _attackFinder;
    private bool _isCharging;

    private WalkAnimate _WalkOrientation;
    private float _orientationAngle;

    private Vector2 _faintStart;
    [SerializeField] private string _attackMessage;
    [SerializeField] private string _attackPower;

    private LockOnTest1 _lockOnScript;

    private void Start()
    {
        InputManager input = FindObjectOfType<InputManager>();
        input.AimingScript = this;
        _startLocation = _sword.transform.position;
        _attackFinder = FindObjectOfType<FindPossibleAttacks>();
        _WalkOrientation = GetComponent<WalkAnimate>();
        _texMessage = GameObject.Find(_attackPower).GetComponent<TextMeshPro>();
        _txtActionPower = GameObject.Find("action power").GetComponent<TextMeshPro>();
        _AttackMessage = GameObject.Find(_attackMessage).GetComponent<TextMeshPro>();
        _lockOnScript = GetComponent<LockOnTest1>();

        //foreach (var hitZone in _hitZones)
        //{
        //    hitZone.SetActive(false);
        //}
    }

    private void Update()
    {
        _orientationAngle = _WalkOrientation.Orientation * Mathf.Rad2Deg;
        AnalogAiming4();
    }

    private void AnalogAiming4()
    {
        //get angle
        float newLength = Direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(Direction.y, Direction.x);
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
            if (_resetAttackStance != null) 
            {
                StopCoroutine(_resetAttackStance);
                _isResetingStance = false;
            }
            _idleTime = 0.0f;

            //Charging
            Chargepower(currentAngleDegree);
            if (_isCharging) return;
            //Attacking
            SetSwingDirection(currentAngleDegree);

            //slashDirection or stab
            SetAttackType(newLength, currentAngleDegree);

            CalculateAttackPower(newLength, currentAngleDegree);

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
        //_txtActionPower.enabled = false;
        _startDirection = 0;
        CurrentAttackType = AttackType.None;
        _isAttackSet = false;
        //foreach (var hitZone in _hitZones)
        //{
        //    hitZone.SetActive(false);
        //}
        _arrow.SetActive(false);
        if(!_isResetingStance) CurrentStanceState = AttackStance.Torso;
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
        switch (CurrentStanceState)
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
        //_hitZones[index].SetActive(true);
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

        if (angle > 110.0f + _orientationAngle - 90f || angle < 70.0f + _orientationAngle - 90f)
        {
            //_currentAttackType = (_currentAttackType == AttackType.Stab) ? AttackType.HorizontalSlashLeft : _currentAttackType;
            CurrentAttackType = AttackType.HorizontalSlashLeft;
            _isAttackSet = true;
        }
        else if(!_isAttackSet)
        {
            CurrentAttackType = AttackType.Stab;
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
                if (SlashUp && SlashDown)
                {
                    _isAttackSet = true;
                    Debug.Log("Special attack from the left");
                }
                else if (SlashDown)
                {
                    CurrentAttackType = AttackType.DownSlashRight;
                    _isAttackSet = true;
                }
                else if (SlashUp)
                {
                    CurrentAttackType = AttackType.UpperSlashRight;
                    _isAttackSet = true;
                }
                if (CurrentAttackType == AttackType.HorizontalSlashLeft) 
                {
                    CurrentAttackType = AttackType.HorizontalSlashRight;
                    _isAttackSet = true;
                }
                break;
            case 1:
                if (SlashUp && SlashDown)
                {
                    _isAttackSet = true;
                    Debug.Log("Special attack from the right");
                }
                else if (SlashDown)
                {
                    CurrentAttackType = AttackType.DownSlashLeft;
                    _isAttackSet = true;
                }
                else if (SlashUp)
                {
                    CurrentAttackType = AttackType.UpperSlashLeft;
                    _isAttackSet = true;
                }
                break;
        }
    }

    private void SetHitboxAngle()
    {
        //switch (CurrentAttackType)
        //{
        //    case AttackType.HorizontalSlashLeft:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f + 180f);
        //        break;
        //    case AttackType.HorizontalSlashRight:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //        break;
        //    case AttackType.DownSlashRight:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
        //        break;
        //    case AttackType.UpperSlashRight:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
        //        break;
        //    case AttackType.DownSlashLeft:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f + 180f);
        //        break;
        //    case AttackType.UpperSlashLeft:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f - 180f);
        //        break;
        //    case AttackType.Stab:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(0.45f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        //        break;
        //    case AttackType.StraightUp:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        //        break;
        //    case AttackType.StraightDown:
        //        //_hitZones[_currentHitBoxIndex].transform.localScale = new Vector3(1.4f, 0.45f, 0.0f);
        //        //_hitZones[_currentHitBoxIndex].transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        //        break;
        //    default: break;
        //}
        //_arrow.transform.position = _hitZones[_currentHitBoxIndex].transform.position;
        //Vector3 arrowAngle = _hitZones[_currentHitBoxIndex].transform.eulerAngles;
        //arrowAngle.z -= 90;
        //_arrow.transform.rotation = Quaternion.Euler(arrowAngle);
    }

    private void Chargepower(float drawAngle)
    {
        if (drawAngle > (_chargeZone.Item1) + _orientationAngle - 90 
            && drawAngle < (_chargeZone.Item2) + _orientationAngle - 90)
        {
            _isCharging = true;
            if (_chargedTime < MAX_CHARGE_TIME)
                _chargedTime += (Time.deltaTime * 4.0f);
            _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, _orientationAngle + 90);
            _sword.transform.localPosition = Vector3.zero;
            _txtActionPower.enabled = false;
            return;
        }
        else _isCharging = false;

        //_txtActionPower.enabled = true;
        //_txtActionPower.text = (defaultPower + _chargedTime).ToString();
    }

    private void SetSwingDirection(float drawAngle)
    {
        float orientationAngle = _orientationAngle - 90f;
        Vector2 drawAngleVector = new Vector2(Mathf.Cos(drawAngle * Mathf.Deg2Rad), Mathf.Sin(drawAngle * Mathf.Deg2Rad));
        Vector2 orientationVector = new Vector2(Mathf.Cos(orientationAngle * Mathf.Deg2Rad), Mathf.Sin(orientationAngle * Mathf.Deg2Rad));
        float cross = drawAngleVector.x * orientationVector.x + drawAngleVector.y * orientationVector.y;

        if (_startDirection == 0)
            _startDirection = (cross > 0) ? 1 : -1;
    }

    private void CalculateAttackPower(float drawLength, float currentangle)
    {
        bool canRun = false;
        if (drawLength >= 0.97f)
        {
            if (_faintStart == Vector2.zero) _faintStart = Direction;
            if (_startDrawPos == Vector2.zero) _startDrawPos = Direction;
            int newAngle = (int)Vector2.Angle(_startDrawPos, Direction);
            canRun = true;

            float cross = Direction.x * _faintStart.x + Direction.y * _faintStart.y;

            if ((int)_slashAngle <= newAngle)
            {
                _slashAngle = newAngle;
                _slashTime += Time.deltaTime;

                if (!_checkFeint)
                {
                    if (CheckOverCommit()) return;
                    _texMessage.text = $"Slash power: {(_slashStrength + (_slashAngle / 100) + _chargedTime) / _slashTime}";
                }
                else if (_checkFeint && _startDirection == -1)
                {
                    if(cross < 0) _checkFeint = !CheckFeint(_slashAngle, 90, _slashTime);
                    else _checkFeint = false;
                }
                else if (_checkFeint && _startDirection == 1) 
                {
                    if (cross > 0) _checkFeint = !CheckFeint(_slashAngle, 90, _slashTime);
                    else _checkFeint = false;
                }
            }
            else if (canRun)
            {
                if(_slashAngle > 15) _checkFeint = true;
                CheckAttack();
                _slashTime = 0.0f;
                _slashAngle = 0.0f;
                _startDrawPos = Vector2.zero;
                canRun = false;
            }
        }
        else if(canRun)
        {
            CheckAttack();
            _slashTime = 0.0f;
            _slashAngle = 0.0f;
            _startDrawPos = Vector2.zero;
            _faintStart = Vector2.zero;
            canRun = false;
            CurrentStanceState = AttackStance.Torso;
        }
        if (drawLength <= MIN_WINDUP_LENGTH)
        {
            _isAttackSet = false;
        }
    }

    private void CheckAttack()
    {
        //if (CheckFeint(_slashAngle, _minSlashAngle)) return;
        GetpossibleAtack();
         foreach(AttackType Possebility in _possibleAttacks) 
        {
            if (CurrentAttackType == Possebility)
            {
                CurrentAttackType = Possebility;
                Attack();
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
        CurrentAttackType = AttackType.None;
        if (_resetAtackText != null) StopCoroutine(_resetAtackText);
        _AttackMessage.text = "Attack was invalid";
        _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
        Debug.Log("Attack was invalid!");
        SetPreviousAttacks();
    }

    private bool CheckOverCommit()
    {
        if (_slashAngle > _overCommitAngle)
        {
            if (_resetAtackText != null) StopCoroutine(_resetAtackText);
            _AttackMessage.text = "Player over commited";
            _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
            CheckAttack();
            _slashTime = 0.0f;
            _slashAngle = 0.0f;
            _startDrawPos = Vector2.zero;
            return true;
        }
        return false;
    }

    private bool CheckFeint(float angle, float minAngle, float time)
    {
        if (angle < minAngle && time < 0.5f)
        {
            _AttackMessage.text = "Feint";
            Debug.Log(_AttackMessage.text);
            if (_resetAtackText != null) StopCoroutine(_resetAtackText);
            _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
            return true;
        }
        return false;
    }

    private void SetPreviousAttacks()
    {
        _previousAttack = CurrentAttackType;
    }

    private void GetpossibleAtack()
    {
        _possibleAttacks.Clear();

        _possibleAttacks = _attackFinder.GetpossibleAtack(CurrentStanceState, _previousAttack);
    }

    private void Attack()
    {
        Sprite sword = _sword.GetComponent<SpriteRenderer>().sprite;
        float swordlength = Mathf.Sqrt((sword.rect.width * sword.rect.width) + (sword.rect.height * sword.rect.height));
        float enemyDistance = Vector2.Distance(_lockOnScript.LockOnTarget.transform.position, transform.position);
        if (swordlength >= enemyDistance) _lockOnScript.LockOnTarget.GetComponent<HitDetection>().HitDetected(this.gameObject);
    }

    private void SwordVisual(float angle)
    {
        //Sword follows analog -> visualization 
        _sword.transform.localPosition = new Vector3(Direction.x * radius, Direction.y * radius, 0.0f);
        Vector3 swordRotation = new Vector3(0, 0, angle);
        swordRotation.z += DEFAULT_SWORD_ORIENTATION - 90f;
        _sword.transform.rotation = Quaternion.Euler(swordRotation);
    }

    private IEnumerator ResetAttackStance(float time)
    {
        _isResetingStance = true;
        yield return new WaitForSeconds(time);
        CurrentStanceState = AttackStance.Torso;
        _isResetingStance = false;
    }

    private IEnumerator ResetText(float time, TextMeshPro text)
    {
        yield return new WaitForSeconds(time);
        text.text = " ";
    }

    public void ChangeStance(AttackStance stance)
    {
        if (_resetAttackStance != null) StopCoroutine(_resetAttackStance);
        CurrentStanceState = stance;
        _resetAttackStance = StartCoroutine(ResetAttackStance(_stanceResetTimer));
    }
}