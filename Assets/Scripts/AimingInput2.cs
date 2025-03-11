using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AimingInput2 : MonoBehaviour
{
    public Vector2 Direction;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;

    
    public AttackStance CurrentStanceState = AttackStance.Torso;
    private AttackStance _previousStance = AttackStance.Torso;
    public AttackType CurrentAttackType = AttackType.None;
    private AttackType _previousAttack = AttackType.None;

    //[SerializeField] private float _speed = 10.0f;
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
    [SerializeField] public float radius = 10.0f;
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
    private float _speed;
    [SerializeField]
    private float _slashStrength;

    private int _currentHitBoxIndex;
    private List<AttackType> _possibleAttacks = new List<AttackType>();
    //private List<AttackType> _OverComitAttacks = new List<AttackType>();
    private bool _isAttackSet;

    [SerializeField]
    private float _overCommitAngle = 170f;

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

    private float _feintStartAngle;
    [SerializeField] float _minFeintAngle = 15;
    [SerializeField] private string _attackMessage;
    [SerializeField] private string _attackPower;

    private LockOnTest1 _lockOnScript;
    private bool _feinted;
    private bool _attemptedAttack;
    private float _damage;
    private bool _overcommited;
    private bool _canRun;
    public bool IsParrying;

    [SerializeField] private int _staminaCost;
    private StaminaManager _staminaManager;

    private bool _isInitialized;

    private int _power = 10;
    private HealthManager _healthManager;
    private WalkAnimate _animationRef;

    private bool _checkedForBlock;

    private void Start()
    {
        //InputManager input = FindObjectOfType<InputManager>();
        //input.AimingScript = this;
        //_attackFinder = FindObjectOfType<FindPossibleAttacks>();

        //initPlayer();
        _healthManager = GetComponent<HealthManager>();
        _animationRef = GetComponent<WalkAnimate>();
        if (_healthManager.Physique > 5) _power -= 5 - _healthManager.Physique;
        else if (_healthManager.Physique < 5) _power -= 5 - _healthManager.Physique;
    }

    private void Update()
    {
        if (!_isInitialized) return;
       /* if (GetComponent<AIController>() != null)
            return;*/
        _orientationAngle = _WalkOrientation.Orientation * Mathf.Rad2Deg;
        AnalogAiming4();
    }

    private void AnalogAiming4()
    {
        //get angle
        float newLength = Direction.SqrMagnitude();
        float currentAngle = Mathf.Atan2(Direction.y, Direction.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;

        //Reset values when idle to long (cant stay charged up when staying in center position)
        if (newLength < MIN_WINDUP_LENGTH && _chargedTime < MAX_Idle_TIME)
        {
            _idleTime += Time.deltaTime;
            _chargedTime = (_idleTime >= MAX_Idle_TIME) ? 0.0f : _chargedTime;
        }

        //Start moving analog , Attack or Charge up
        if ((newLength > MIN_WINDUP_LENGTH))
        {
            if(_staminaManager != null) _staminaManager.IsAttacking = true;
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

            if (_lockOnScript.LockOnTarget)
                CalculateAttackPower(newLength, currentAngleDegree);

            //decrease power the longer your arm is stretched out in front
            if ((currentAngleDegree < 110.0f && currentAngleDegree > 70.0f) || newLength < MIN_WINDUP_LENGTH)
            {
                defaultPower -= (defaultPower > 0) ? (Time.deltaTime * 9.0f) : 0.0f;
                _chargedTime -= (_chargedTime > 0) ? (Time.deltaTime * 4.0f) : 0.0f;
            }
        }
        else
        {
            if (_checkedForBlock)
            {
                _checkedForBlock = false;
            }
                ResetValues();
            if (_staminaManager != null) _staminaManager.IsAttacking = false;
        }
        //Force direction to be correct on idle
        if (_sword && newLength < MIN_WINDUP_LENGTH)
        {
            _sword.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }
        SwordVisual(currentAngleDegree);
    }

    private void ResetValues()
    {
        if (_sword)
        {
            _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
            _sword.transform.localPosition = _startLocation;
        }

        _chargedTime = 0.0f;
        defaultPower = 5.0f;
        _speed = 0f;
        //_txtActionPower.enabled = false;
        _startDirection = 0;
        CurrentAttackType = AttackType.None;
        _isAttackSet = false;
        _feinted = false;
        //foreach (var hitZone in _hitZones)
        //{
        //    hitZone.SetActive(false);
        //}
        if (_arrow)
            _arrow.SetActive(false);
        if(!_isResetingStance) CurrentStanceState = AttackStance.Torso;
    }

    private void SetStance()
    {
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
    }

    private void SetAttackType(float drawLength, float angle)
    {
        if (_isAttackSet) return;

        if (angle > 110.0f + _orientationAngle - 90f || angle < 70.0f + _orientationAngle - 90f)
        {
            //_currentAttackType = (_currentAttackType == AttackType.Swing) ? AttackType.HorizontalSlashLeft : _currentAttackType;
            CurrentAttackType = AttackType.HorizontalSlashLeft;
            _isAttackSet = true;
        }
        else
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

    private void Chargepower(float drawAngle)
    {
        float newOrientationAngle = 0f;
        if (_orientationAngle < 0) newOrientationAngle = _orientationAngle + 90 + 180;
        else newOrientationAngle = _orientationAngle - 90;

        if (drawAngle > (_chargeZone.Item1) + newOrientationAngle
            && drawAngle < (_chargeZone.Item2) + newOrientationAngle)
        {
            _isCharging = true;
            if (_chargedTime < MAX_CHARGE_TIME)
                _chargedTime += (Time.deltaTime * 4.0f);
            if (_sword)
            {
                _sword.transform.rotation = Quaternion.Euler(0.0f, YRotation, _orientationAngle + 90);
                _sword.transform.localPosition = Vector3.zero;
            }

            if (_txtActionPower)
                _txtActionPower.enabled = false;
            return;
        }
        else _isCharging = false;

        //_txtActionPower.enabled = true;
        //_txtActionPower.text = (defaultPower + _chargedTime).ToString();
    }

    private void SetSwingDirection(float drawAngle)
    {
        float newOrientationAngle = 0f;
        if (_orientationAngle < 0) newOrientationAngle = _orientationAngle + 90 + 180;
        else newOrientationAngle = _orientationAngle - 90;
        Vector2 drawAngleVector = new Vector2(Mathf.Cos(drawAngle * Mathf.Deg2Rad), Mathf.Sin(drawAngle * Mathf.Deg2Rad));
        Vector2 orientationVector = new Vector2(Mathf.Cos(newOrientationAngle * Mathf.Deg2Rad), Mathf.Sin(newOrientationAngle * Mathf.Deg2Rad));
        float cross = drawAngleVector.x * orientationVector.x + drawAngleVector.y * orientationVector.y;

        if (_startDirection == 0)
            _startDirection = (cross > 0) ? 1 : -1;
    }

    private void CalculateAttackPower(float drawLength, float currentangle)
    {
        if (CurrentAttackType == AttackType.Stab)
        {
            if (_lockOnScript.LockOnTarget && !_checkedForBlock)
            {
                _checkedForBlock = true;
                Blocking blocker = _lockOnScript.LockOnTarget.GetComponent<Blocking>();
                SwordParry swordParry = _lockOnScript.LockOnTarget.GetComponent<SwordParry>();
                if (swordParry && swordParry.IsParrying())
                {
                    swordParry.StartParry(true, gameObject, _power);
                }
                else if (blocker.StartHit(CurrentStanceState, _startDirection, gameObject))
                {
                    _animationRef.GetHit();
                }
            }
            CheckAttack();
            return;
        }
            if (_feinted) _canRun = false;
        if (drawLength >= 0.97f)
        {
            if (_startDrawPos == Vector2.zero) _startDrawPos = Direction;
            int newAngle = (int)Vector2.Angle(_startDrawPos, Direction);

            if ((int)_slashAngle < newAngle)
            {
                _slashAngle = newAngle;
                _slashTime += Time.deltaTime;
                _canRun = true;

                var comp = GetComponent<GoapPlanner>();
                if (comp) comp.UpdateSwingSpeed(_slashAngle / _slashTime);
                else
                    _speed = _slashAngle / _slashTime;

                if (!_checkFeint)
                {
                    if(_slashAngle > _minFeintAngle && CurrentAttackType != AttackType.Stab && !_checkedForBlock)
                    {
                        _checkedForBlock = true;
                        SwordParry swordParry = _lockOnScript.LockOnTarget.GetComponent<SwordParry>();
                        Blocking blocker = _lockOnScript.LockOnTarget.GetComponent<Blocking>();
                        if (swordParry && swordParry.IsParrying())
                        {
                            swordParry.StartParry(true, gameObject, _power, _startDirection);
                        }
                        else if (blocker.StartHit(CurrentStanceState, _startDirection, gameObject))
                        {
                            _animationRef.GetHit();
                        }
                    }

                    if (CheckOverCommit())
                    {
                        _checkedForBlock = false;
                        SwordParry swordParry = _lockOnScript.LockOnTarget.GetComponent<SwordParry>();
                        Blocking blocker = _lockOnScript.LockOnTarget.GetComponent<Blocking>();
                        blocker.StopParryTime();
                        swordParry.StartParry(false, null, 0);
                        _overcommited = true;
                        return;
                    }
                    else _overcommited = false;
                    _damage = (int)((_slashStrength + (_slashAngle / 100) + _chargedTime) / _slashTime) / 5;
                    _damage = Mathf.Clamp(_damage, 0, 10);
                    if (_texMessage)
                        _texMessage.text = $"Slash power: {_damage}";
                }
                else if (_checkFeint && _startDirection == -1)
                {
                    if (_feintStartAngle < currentangle - 90f) _checkFeint = !CheckFeint(_slashAngle, 90, _slashTime);
                    else
                    {
                        _feinted = false;
                        _checkFeint = false;
                        //Debug.Log($"set CheckFeint{_checkFeint}");
                        _feintStartAngle = 0f;
                    }
                }
                else if (_checkFeint && _startDirection == 1)
                {
                    if (_feintStartAngle > currentangle - 90f) _checkFeint = !CheckFeint(_slashAngle, 90, _slashTime);
                    else
                    {
                        _feinted = false;
                        _checkFeint = false;
                        //Debug.Log($"set CheckFeint{_checkFeint}");
                        _feintStartAngle = 0f;
                    }
                }
            }
            else if (_canRun && !_feinted && !_overcommited)
            {
                _checkedForBlock = false;
                _canRun = false;
                _attemptedAttack = true;
                if (_feintStartAngle == 0 && _slashAngle > _minFeintAngle) _feintStartAngle = currentangle - 90;
                if (_slashAngle > _minFeintAngle && _attemptedAttack)
                {
                    _checkFeint = true;
                    //Debug.Log($"set CheckFeint{_checkFeint}");
                }
                if(!_checkFeint)CheckAttack();
                _slashTime = 0.0f;
                _slashAngle = 0.0f;
                _startDrawPos = Vector2.zero;
            }
            if(_feinted) _feinted = false;
        }
        else if (_canRun && !_feinted && !_overcommited)
        {
            _checkedForBlock = false;
            CheckAttack();
            _slashTime = 0.0f;
            _slashAngle = 0.0f;
            _startDrawPos = Vector2.zero;
            _feintStartAngle = 0;
            _canRun = false;
            CurrentStanceState = AttackStance.Torso;
        }
        if (drawLength <= MIN_WINDUP_LENGTH)
        {
            _checkedForBlock = false;
            _isAttackSet = false;
            _feinted = false;
        }
    }

    private void CheckAttack()
    {
        if (IsParrying) return;
        //if (CheckFeint(_slashAngle, _minSlashAngle)) return;
        GetpossibleAtack();
         foreach(AttackType Possebility in _possibleAttacks) 
        {
            if (CurrentAttackType == Possebility)
            {
                CurrentAttackType = Possebility;

                _feinted = false;
                _checkFeint = false;
                //Debug.Log($"set CheckFeint{_checkFeint}");
                _feintStartAngle = 0f;
                _overcommited = false;
                Attack();
                SetPreviousAttacks();
                return;
            }
        }
        CurrentAttackType = AttackType.None;
        if (_resetAtackText != null) StopCoroutine(_resetAtackText);
        _AttackMessage.text = "Attack was invalid";
        _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
       //Debug.Log("Attack was invalid!");
        SetPreviousAttacks();
    }

    private bool CheckOverCommit()
    {
        if (_slashAngle > _overCommitAngle)
        {
            if (_resetAtackText != null) StopCoroutine(_resetAtackText);
            if (_AttackMessage)
            {
                _AttackMessage.text = "Player over commited";
                _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
            }

            _slashTime = 0.0f;
            _slashAngle = 0.0f;
            _startDrawPos = Vector2.zero;
            _feintStartAngle = 0;
            return true;
        }
        return false;
    }

    private bool CheckFeint(float angle, float minAngle, float time)
    {
        if (angle < minAngle && time < 0.5f)
        {
            if(CurrentAttackType != AttackType.Stab)
            {
                SwordParry swordParry = _lockOnScript.LockOnTarget.GetComponent<SwordParry>();
                Blocking blocker = _lockOnScript.LockOnTarget.GetComponent<Blocking>();
                blocker.StopParryTime();
                swordParry.StartParry(false, null, 0);
            }
            if (_AttackMessage)
                _AttackMessage.text = "Feint";
            //Debug.Log(_AttackMessage.text);
            if (_resetAtackText != null) 
                StopCoroutine(_resetAtackText);
            if (_AttackMessage)
                _resetAtackText = StartCoroutine(ResetText(0.5f, _AttackMessage));
            _feinted = true;
            _attemptedAttack = false;
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
        if(CurrentAttackType == AttackType.Stab)
        {
            SwordParry swordParry = _lockOnScript.LockOnTarget.GetComponent<SwordParry>();
            swordParry.StartParry(false, null, 0);
            Blocking blocker = _lockOnScript.LockOnTarget.GetComponent<Blocking>();
            blocker.StopParryTime();
        }

        SpriteRenderer sword = _sword.GetComponent<SpriteRenderer>();
        float swordlength = Mathf.Sqrt((sword.bounds.size.x * sword.bounds.size.x) + (sword.bounds.size.y * sword.bounds.size.y));
        if(_lockOnScript.LockOnTarget == null) return;
        float enemyDistance = Vector2.Distance(_lockOnScript.LockOnTarget.transform.position, transform.position);
        if (swordlength >= enemyDistance)
        {
            _lockOnScript.LockOnTarget.GetComponent<HitDetection>().HitDetected(gameObject, _damage);

            if (CurrentAttackType == AttackType.Stab && _staminaManager.CurrentStamina > _staminaCost) _staminaManager.DepleteStamina(_staminaCost);
            else if (_staminaManager.CurrentStamina > _staminaCost * 1.5f) _staminaManager.DepleteStamina((int)(_staminaCost * 1.5f));
            else return;
        }
    }

    private void SwordVisual(float angle)
    {
        if (!_sword)
            return;
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

    public void NewSword(GameObject newSword)
    {
        _sword = newSword;
        if (_sword == null)
            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);

        //Hide fist away when it was selected
        else if (_sword.GetComponent<Equipment>().GetEquipmentType() == EquipmentType.Fist)
        {
            _sword.transform.localScale = Vector3.zero;
            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
        }
        radius = 0.4f;
    }

    public void SwordBroke()
    {
        if (GetComponent<HeldEquipment>().HoldsEquipment(EquipmentType.Weapon))
            return;

        if (GetComponent<HeldEquipment>().HoldsEquipment(EquipmentType.Shield))
        {
            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Shield);
            radius = 1.0f;
        }

        else
        {
            _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Fist);
            _sword.transform.localScale = Vector3.one;
            radius = 1.5f;
        }
    }

    public void initPlayer()
    {
        _attackFinder = GetComponent<FindPossibleAttacks>();
        _WalkOrientation = GetComponent<WalkAnimate>();
        _sword = GetComponent<HeldEquipment>().GetEquipment(EquipmentType.Weapon);
        if (_sword)
        {
            _startLocation = _sword.transform.position;
            _slashStrength = _sword.GetComponent<Equipment>().GetEquipmentstrength();
        }

        _lockOnScript = GetComponent<LockOnTest1>();
        _staminaManager = GetComponent<StaminaManager>();
        _isInitialized = true;

        if (GetComponent<AIController>() != null)
            return;
        _texMessage = GameObject.Find(_attackPower).GetComponent<TextMeshPro>();
        _txtActionPower = GameObject.Find("action power").GetComponent<TextMeshPro>();
        _AttackMessage = GameObject.Find(_attackMessage).GetComponent<TextMeshPro>();
    }

    public float GetSwingSpeed()
    {
        return _speed;
    }

}