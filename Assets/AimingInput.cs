using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackStance
{
    Head,
    Torso,
    Legs
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
    [SerializeField] HitDetection _hitDetector;

    private Vector2 _direction;
    private Vector2 _moveDirection;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;

    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;
    private AttackStance stanceState = AttackStance.Torso;

    [SerializeField] private float _speed = 10.0f;

    private float _chargeUpTime = 0.0f;
    private float longestWindup = 0;
    private float _currentReleaseTime = 0.0f;
    private const float MAX_RELEASE_TIME = 0.5f; 
    private const float MIN_WINDUP_LENGTH = 0.25f;
    private const float MIN_CHARGEUP_TIME = 0.10f;
    private bool _isStab = false;

    private void Awake()
    {
        _input = new AimInputFull();
    }

    private void OnEnable()
    {
        //_input.Player.Move.performed += OnMove;
        //_input.Player.Move.canceled += OnMove;
        _input.Enable();
        //characterController = GetComponent<CharacterController>();
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
        //arrow.transform.localScale *= (0.990f );


        AnalogAiming();

        _moveDirection = moveAction.ReadValue<Vector2>();
        //characterController.Move(_moveDirection * _speed * Time.deltaTime);
    }

    private void AnalogAiming()
    {
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
                if (newLength >= longestWindup)
                {
                    longestWindup = newLength;
                    loadDirection = _direction;
                    //Debug.Log($"power {longestWindup} ");

                    if (longestWindup > MIN_WINDUP_LENGTH)
                        _chargeUpTime += Time.deltaTime;
                }
                else if (newLength < MIN_WINDUP_LENGTH && longestWindup > MIN_WINDUP_LENGTH)
                {
                    slashState = FindSlashState();
                    if (_chargeUpTime < MIN_CHARGEUP_TIME)
                    {
                        _isStab = true;
                        slashState = ((int) slashState < 0)? slashState + 180 : slashState - 180;
                        Debug.Log($"Stab at{stanceState} in direction {slashState} with power: {_chargeUpTime:F2}");
                        state = SlashState.Rest;
                        _hitDetector.GetHitPos(slashState, stanceState, _isStab);
                        return;
                    }
                    state = SlashState.Release;
                    //Decide slash state
                    //Debug.Log($"aiming to {slashState}");
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
                    _isStab = false;
                    Debug.Log($"Slash at{stanceState} in direction {slashState} with power: {_chargeUpTime}");
                    _hitDetector.GetHitPos(slashState, stanceState, _isStab);
                    //RotateArrow();
                    state = SlashState.Rest;
                }
                break;

            case SlashState.Rest:
                //Debug.Log($"Reset");
                if (newLength < MIN_WINDUP_LENGTH)
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
}