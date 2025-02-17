using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class AimingPrototype : MonoBehaviour
{
    private AimInputFull _input;
    private InputAction AimAction;
    private InputAction moveAction;
    private InputAction AimHead;
    private InputAction AimFeet;
    private CharacterController characterController;
    private WalkAnimate _animator;


    [SerializeField] GameObject _sword;
    [SerializeField] GameObject _hitZone;
    [SerializeField] float radius = 10.0f;
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] float _minDiffBetwnAnalogMov = 0.00125f;
    [SerializeField] private TextMeshPro _txtActionPower;
    [SerializeField] private TextMeshPro _texMessage;


    private Vector2 _inputMovement = Vector2.zero;

    private Vector2 _inputDirection = Vector2.zero;
    private Vector2 _startDirection = Vector2.zero;
    private Vector2 _previousDirection = Vector2.zero;
    private SlashState _attackState = SlashState.Rest;
    private MovingDirection _aimingHightState = MovingDirection.Neutral;

    private float _accumulatedTime = 0;
    private float _accumulatedPower = 0;

    private const float DEFAULT_SWORD_ORIENTATION = 218.0f;

    private void Awake()
    {
        _input = new AimInputFull();
    }

    private void Start()
    {
        _hitZone.SetActive(false);
        _animator = GetComponent<WalkAnimate>();
    }

    private void OnEnable()
    {
        _input.Enable();
        characterController = GetComponent<CharacterController>();
        moveAction = _input.FindAction("Move");
        AimAction = _input.FindAction("Look");
        AimHead = _input.Player.Stab1;
        AimFeet = _input.Player.RightBlock;
        //rightBlockAction = _input.Player.RightBlock;
    }


    // Update is called once per frame
    void Update()
    {
        Walk();
        AnalogAiming();
    }

    private void AnalogAiming()
    {
        //get input values
        RegisterInput();
        float newLength = _inputDirection.SqrMagnitude();
        float currentAngle = Mathf.Atan2(_inputDirection.y, _inputDirection.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //Debug.Log($"{newLength:F2}");

        if (DetectAnalogMovement())
        {
            _accumulatedPower += newLength;
            _accumulatedPower /= 2.0f;

            float angle = Vector2.Angle(_startDirection, _inputDirection);
            _accumulatedTime += Time.deltaTime;
            float power = angle * _accumulatedTime;

            _txtActionPower.text = $"Power: {_accumulatedPower:F4}";
            _texMessage.text = $"Speed: {power:F4}";
        }
        else
        {
            _startDirection = _inputDirection;
            _accumulatedTime = 0.0f;
            _accumulatedPower = 0.0f;

        }

        _sword.transform.localPosition = new Vector3(_inputDirection.x * radius, _inputDirection.y * radius, 0.0f);
        _sword.transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngleDegree + DEFAULT_SWORD_ORIENTATION - 90.0f);



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
        _inputDirection = AimAction.ReadValue<Vector2>();
        _inputMovement = moveAction.ReadValue<Vector2>();
        
        if (AimHead.IsPressed())
        {
            _aimingHightState = MovingDirection.MovingUp;
        }
        else if (AimFeet.IsPressed())
        {
            _aimingHightState = MovingDirection.MovingDown;
        }
        else
            _aimingHightState = MovingDirection.Neutral;
    }

    private void Walk()
    {
        characterController.Move(_inputMovement * movementSpeed * Time.deltaTime);
        _animator.Walk(_inputMovement);
    }

}
