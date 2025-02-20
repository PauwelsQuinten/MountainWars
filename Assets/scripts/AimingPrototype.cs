using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class AimingPrototype : MonoBehaviour
{
    //private AimInputFull _input;
    [SerializeField] private InputActionReference AimAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference AimHead;
    [SerializeField] private InputActionReference AimFeet;
    [SerializeField] private InputActionReference LockOn;
    private CharacterController characterController;
    private WalkAnimate _animator;


    [SerializeField] GameObject _sword;
    [SerializeField] GameObject _hitZone;
    [SerializeField] float radius = 10.0f;
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] float _leaningSpeed = 2.0f;
    [SerializeField] float _minDiffBetwnAnalogMov = 0.00125f;
    [SerializeField] float _MaxTimeNotLeaning = 0.5f;
    [SerializeField] private TextMeshPro _txtActionPower;
    [SerializeField] private TextMeshPro _texMessage;
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _useLockOnMovement = false;


    private Vector2 _inputMovement = Vector2.zero;

    private Vector2 _inputDirection = Vector2.zero;
    private Vector2 _startDirection = Vector2.zero;
    private Vector2 _previousDirection = Vector2.zero;
    private SlashState _attackState = SlashState.Rest;
    private MovingDirection _aimingHightState = MovingDirection.Neutral;
    

    private float _accumulatedTime = 0;
    private float _accumulatedPower = 0;
    private float _currentTimeNotLeaning = 0;
    private bool _isLockOn = false;

    private const float DEFAULT_SWORD_ORIENTATION = 218.0f;
    private const float MAX_HITBOX_HEIGHT = 4.0f;
    private const float MIN_HITBOX_HEIGHT = 2.0f;
    private const float _defaultHitzoneHeight = 3.3f;

    private void Start()
    {
        _hitZone.SetActive(false);
        _animator = GetComponent<WalkAnimate>();
    }

    private void OnEnable()
    {
        characterController = GetComponent<CharacterController>();
        //moveAction = _input.FindAction("Move");
        //AimAction = _input.FindAction("Look");
        //AimHead = _input.Player.Stab1;
        //AimFeet = _input.Player.RightBlock;
        //rightBlockAction = _input.Player.RightBlock;
    }


    // Update is called once per frame
    void Update()
    {
        Walk();
        AnalogAiming();

        ////attack animation
        //if (AimHead.action.IsPressed())
        //{
        //    _animator.Attack();
        //}
    }

    private void AnalogAiming()
    {
        //get input values
        RegisterInput();
        float newLength = _inputDirection.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_inputDirection.y, _inputDirection.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //Debug.Log($"{newLength:F2}");
        Lean(newLength);

        if (DetectAnalogMovement())
        {
            _accumulatedPower += newLength;
            _accumulatedPower /= 2.0f;

            float angle = Vector2.Angle(_startDirection, _inputDirection);
            _accumulatedTime += Time.deltaTime;
            float power = angle * _accumulatedTime;

            _txtActionPower.text = $"Power: {_accumulatedPower:F4}";
            _texMessage.text = $"Speed: {power:F4}";

             _hitZone.SetActive((_accumulatedPower * power) > 0.5f);
        }
        else
        {
            _startDirection = _inputDirection;
            _accumulatedTime = 0.0f;
            _accumulatedPower = 0.0f;

            _hitZone.SetActive(false);
        }

        _sword.transform.localPosition = new Vector3(_inputDirection.x * radius, _inputDirection.y * radius, 0.0f);
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngleDegree + DEFAULT_SWORD_ORIENTATION - 90.0f);

        //Force direction to be correct on idle
        if (newLength < 0.1f)
        {
            _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, DEFAULT_SWORD_ORIENTATION);
        }

    }

    private bool DetectAnalogMovement()
    {
        var diff = _previousDirection - _inputDirection;
        bool value = diff.magnitude > _minDiffBetwnAnalogMov;
        //Debug.Log($"{diff.magnitude}");
        //Debug.Log($"{value}");

        _previousDirection = _inputDirection;
        return value;
    }
    private void RegisterInput()
    {
        _inputDirection = AimAction.action.ReadValue<Vector2>();
        _inputMovement = moveAction.action.ReadValue<Vector2>();

        if ( LockOn.action.WasReleasedThisFrame())
        {
            _animator.DoLockOn(_target);
            _isLockOn = !_isLockOn;
        }


        if (AimHead.action.IsPressed())
        {
            _aimingHightState = MovingDirection.MovingUp;
        }
        else if (AimFeet.action.IsPressed())
        {
            _aimingHightState = MovingDirection.MovingDown;
        }
        else
            _aimingHightState = MovingDirection.Neutral;
    }

    private void Walk()
    {
        if (_isLockOn && _useLockOnMovement)
        {
            var lookDir = _target.transform.position - transform.position;
            //lookDir.Normalize();
            //float angleD = Vector2.Angle(Vector2.up, lookDir);
            float angleD = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90;
            var newDirection = Rotate(_inputMovement, angleD);
            newDirection.Normalize();
            float newLength = _inputMovement.magnitude;

            newDirection *= newLength;
            characterController.Move(newDirection * movementSpeed * Time.deltaTime);
        }
        else
            characterController.Move(_inputMovement * movementSpeed * Time.deltaTime);

        _animator.Walk(_inputMovement);
    }

    private void Lean(float newLength)
    {
        if (newLength < 0.6f)
        {

            if (AimHead.action.IsPressed() && _hitZone.transform.position.y <= MAX_HITBOX_HEIGHT)
            {
                _hitZone.transform.position += Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZone.transform.position.y >= MAX_HITBOX_HEIGHT)
                    _hitZone.transform.position = new Vector3(0.0f, MAX_HITBOX_HEIGHT, 0.0f);

            }
            else if (AimFeet.action.IsPressed() && _hitZone.transform.position.y >= MIN_HITBOX_HEIGHT)
            {
                _hitZone.transform.position -= Vector3.up * Time.deltaTime * _leaningSpeed;
                if (_hitZone.transform.position.y <= MIN_HITBOX_HEIGHT)
                    _hitZone.transform.position = new Vector3(0.0f, MIN_HITBOX_HEIGHT, 0.0f);

            }
            //Fall back to default after a time of not pressing
            else
            {
                if (_currentTimeNotLeaning < _MaxTimeNotLeaning)
                {
                    _currentTimeNotLeaning += Time.deltaTime;
                }
                else
                {
                    int sign = 0;
                    float diff = _hitZone.transform.position.y - _defaultHitzoneHeight;
                    if (Mathf.Abs(diff) > 0.1f)
                    {
                        sign = (diff > 0) ? 1 : -1;
                        _hitZone.transform.position += Vector3.down * sign * Time.deltaTime * _leaningSpeed;
                    }
                    else
                    {
                        _hitZone.transform.position = new Vector3(0.0f, _defaultHitzoneHeight, 0.0f);
                        _currentTimeNotLeaning = 0.0f;

                    }


                }
            }
        }
    }

    private Vector2 Rotate(Vector2 v, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 rotatedVector3 = rotation * new Vector3(v.x, v.y, 0);
        return new Vector2(rotatedVector3.x, rotatedVector3.y);
    }

}
