using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum ParryChanceState
{
    Start,
    Stop,
    None,
    Succes
}
public enum BlockState
{
    Idle,
    MovingShield,
    HoldBlock,
    WeakeningBlock,
    Broken
}


public class Shield : MonoBehaviour
{
    //angle in radians to move to complete parry
    [SerializeField] private float _angleMovementToParry = 1.57f;

    private ParryChanceState _currentParryChance = ParryChanceState.None;

    private Vector2 _blockInputDirection;
    private Vector2 _previousDirection;
    private Vector2 _startParryVector;

    private float _currentParryAngle = 0f;
    private float _startParryAngle = 0f;

    bool _useShield = false;

    private const float MIN_DIFF_BETWEEN_INPUT = 0.00125f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //---------------------------------------------
    //FUNCTIONS
    private bool DetectAnalogMovement()
    {
        var diff = _previousDirection - _blockInputDirection;
        bool value = diff.magnitude > MIN_DIFF_BETWEEN_INPUT;
        //Debug.Log($"{diff.magnitude}");
        //Debug.Log($"{value}");

        _previousDirection = _blockInputDirection;
        return value;
    }


    private bool ParryOnZone(GameObject attacker)
    {

        if (_currentParryChance == ParryChanceState.Start && AroundParryZone())
        {
            //if (_currentParryChance != ParryChanceState.Succes && _currentParryAngle <= _startParryAngle - 2.1415f)
            if (_currentParryChance != ParryChanceState.Succes && _currentParryAngle >= _angleMovementToParry)
            {
                Debug.Log($"SUCCES!!!");
                _currentParryChance = ParryChanceState.Succes;
                AttackTimer attComp = attacker.GetComponent<AttackTimer>();
                attComp.Parried();
            }

        }
        return false;
    }

    private bool AroundParryZone()
    {
        //float angle = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x) - _startParryAngle;
        //if ( Mathf.Sign(angle) != Mathf.Sign(_currentParryAngle) && _currentParryAngle != 0f)
        //{
        //    angle -= Mathf.Sign(angle) * Mathf.PI *2f;
        //}
        //float diff = Mathf.Abs(angle - _currentParryAngle);
        //if (angle < _currentParryAngle && diff < 0.8f)
        //{
        //    _currentParryAngle = angle;
        //    return true;
        //}

        float angle = Vector2.Angle(_blockInputDirection, _startParryVector) * Mathf.Deg2Rad;
        float diff = Mathf.Abs(angle - _currentParryAngle);
        if (angle > _currentParryAngle && diff < 0.8f)
        {
            _currentParryAngle = angle;
            return true;
        }

        Debug.Log($"Faill, angleDiff = {Mathf.Abs(angle - _currentParryAngle)} ");
        _currentParryChance = ParryChanceState.Stop;
        return false;
    }

    //-------------------------------------------------------------------------------
    //PUBLIC FUNCTIONS

    public void UseShield(bool useShield)
    {
        _useShield = useShield; 
    }

    public void MoveShield(Vector2 shieldMoveInput)
    {
        _blockInputDirection = shieldMoveInput;
    }

    public void StartParryTime(AttackStance height)
    {
        if (_currentParryChance == ParryChanceState.None)
        {
            _startParryAngle = Mathf.Atan2(_blockInputDirection.y, _blockInputDirection.x);

            if (_startParryAngle == 0f)
            {
                Debug.Log("To Slow");
                _currentParryChance = ParryChanceState.Stop;
                return;
            }

            switch (height)
            {
                case AttackStance.Head:
                    if (_startParryAngle > 1f && _startParryAngle < 2.75f)
                        _currentParryChance = ParryChanceState.Start;
                    break;

                case AttackStance.Torso:
                    if ((_startParryAngle > 2f && _startParryAngle <= Mathf.PI) || (_startParryAngle < -2f && _startParryAngle >= -Mathf.PI))
                        _currentParryChance = ParryChanceState.Start;
                    break;

                case AttackStance.Legs:
                    if (_startParryAngle < -1f && _startParryAngle > -2.75f)
                        _currentParryChance = ParryChanceState.Start;
                    break;


            }
            _currentParryAngle = 0.0f;
            _startParryVector = _blockInputDirection;

            if (_currentParryChance == ParryChanceState.None)
            {
                Debug.Log("Wrong start height");
                _currentParryChance = ParryChanceState.Stop;
                return;
            }
            //Debug.Log($"Start!!!");
        }
    }

    public void StopParryTime()
    {
        _currentParryChance = ParryChanceState.None;
        //Debug.Log($"Stop!!!");
    }


}
