using UnityEngine;
using UnityEngine.InputSystem;


public class AimingInput : MonoBehaviour
{
    [SerializeField]
    private HitDetection _hitDetector;
    private AimInput _input;
    private Vector2 _direction;
    private InputAction moveAction;

    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;
    private float longestWindup = 0;
    private const float MIN_WINDUP_LENGTH = 0.25f;
    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;

    private float _currentReleaseTime = 0.0f;
    private const float MAX_RELEASE_TIME = 1.0f; 

    private void Awake()
    {
        _input = new AimInput();
    }

    private void OnEnable()
    {
        _input.Enable();
        moveAction = _input.FindAction("Look");
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Update()
    {
        _direction = moveAction.ReadValue<Vector2>();
        float newLength = _direction.magnitude;


        switch (state)
        {
            case SlashState.Windup:
                if (newLength >= longestWindup)
                {
                    longestWindup = newLength;
                    loadDirection = _direction;
                    //Debug.Log($"power {longestWindup} ");
                }
                else if (newLength < MIN_WINDUP_LENGTH && longestWindup > MIN_WINDUP_LENGTH)
                {
                    //longestWindup = 0;
                    state = SlashState.Release;
                    //Decide slash state
                    slashState = FindSlashState();
                    //Debug.Log($"aiming to {slashState}");
                }
                break;

            case SlashState.Release:
                if (TimeOut())
                {
                    longestWindup = 0;
                    loadDirection = Vector2.zero;
                    slashDirection = Vector2.zero;
                    state = SlashState.Rest;
                    return;
                }
                else if (newLength > 0.9f)
                {
                    slashDirection = _direction;
                    //Check if load and release angle is correct
                    float angle = Vector2.Angle(loadDirection, slashDirection);
                    float acceptedAngle = 60.0f;
                    if (angle < acceptedAngle)
                    {
                        //Fail
                        state = SlashState.Rest;
                        Debug.Log($"Fail, angle{angle}");
                        return;
                    }

                    //Get power
                    float power = longestWindup;

                    //SLASH!!!!
                    Debug.Log($"Slash type: {slashState}, power: {power}");
                    state = SlashState.Rest;
                    _hitDetector.GetHitPos();
                }
                break;

            case SlashState.Rest:
                Debug.Log($"Reset");
                if (newLength < MIN_WINDUP_LENGTH)
                {
                    state = SlashState.Windup;
                    longestWindup = 0;
                    loadDirection = Vector2.zero;
                    slashDirection = Vector2.zero;
                    _currentReleaseTime = 0;

                }
                break;
        }
    }

    private SlashDirection FindSlashState()
    {
        SlashDirection slash = new SlashDirection();

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
