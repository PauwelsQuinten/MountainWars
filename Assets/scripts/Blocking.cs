using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum BlockState
{
    Idle,
    StartBlock,
    HoldBlock,
    WeakeningBlock
}

public class Blocking : MonoBehaviour
{
    [SerializeField] private GameObject _shield;
    [SerializeField] private InputActionReference _useShieldAction;
    [SerializeField] private InputActionReference _blockAction;
    [SerializeField] private TextMeshPro _txtBlockPower;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private float _powerReducer = 0.1f;


    private Vector2 _previousDirection;
    private Vector2 _blockInputDirection;
    private const float MIN_DIFF_BETWEEN_INPUT = 0.00125f;
    private const float MAX_BLOCK_HOLD = 0.5f;
    private const float MIN_BLOCK_POWER = 5f;
    private const float MAX_BLOCK_POWER = 50f;
    private float _blockPower = 0.0f;
    private float _accumulatedTime = 0.0f;
    private float _currentBlockingTime = 0.0f;
    private BlockState _blockState = BlockState.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //BlockPrototype1();
        BlockPrototype2();

    }

    private void BlockPrototype1()
    {
        _blockInputDirection = _blockAction.action.ReadValue<Vector2>();
        float distance = _blockInputDirection.sqrMagnitude;

        if (!_useShieldAction.action.IsPressed()/* || distance < 0.1f*/)
        {
            ResetValues();
            return;
        }

        string blockPower = $"BlockPower: {_blockPower:F2}";
        _txtBlockPower.text = blockPower;


        switch (_blockState)
        {
            case BlockState.Idle:
                if (DetectAnalogMovement())
                {
                    _blockState = BlockState.StartBlock;
                }
                break;

            case BlockState.StartBlock:
                if (DetectAnalogMovement())
                {
                    _accumulatedTime += Time.deltaTime;

                    if (distance >= 0.9f)
                    {
                        _blockPower = distance / _accumulatedTime;
                        _blockPower = (_blockPower > MAX_BLOCK_POWER) ? MAX_BLOCK_POWER : _blockPower;
                        _blockState = BlockState.HoldBlock;
                        _accumulatedTime = 0.0f;
                    }
                }
                break;

            case BlockState.HoldBlock:
                _currentBlockingTime += Time.deltaTime;
                if (_currentBlockingTime > MAX_BLOCK_HOLD)
                {
                    _blockState = BlockState.WeakeningBlock;
                    _currentBlockingTime = 0.0f;
                }

                break;

            case BlockState.WeakeningBlock:
                _blockPower -= (_blockPower <= MIN_BLOCK_POWER) ? 0.0f : Time.deltaTime * _powerReducer;

                if (/*!_useShieldAction.action.IsPressed() ||*/ distance < 0.1f)
                {
                    ResetValues();
                    return;
                }

                break;


        }

        _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
    }

    private void BlockPrototype2()
    {
        _blockInputDirection = _blockAction.action.ReadValue<Vector2>();
        float distance = _blockInputDirection.sqrMagnitude;


        switch (_blockState)
        {
            case BlockState.Idle:
                if (DetectAnalogMovement())
                {
                    _blockState = BlockState.StartBlock;
                    _accumulatedTime = 0.0f;
                }
                break;

            case BlockState.StartBlock:
                _accumulatedTime += Time.deltaTime;
                _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);

                if (!DetectAnalogMovement())
                {
                    if (ReturnOnIdle(distance))
                        return;
                    _blockState = BlockState.HoldBlock;
                    _blockPower = distance / _accumulatedTime;
                    _currentBlockingTime = 0.0f;
                    _txtBlockPower.text = $"BlockPower : {_blockPower}";
                }
                break;

            case BlockState.HoldBlock:
                _currentBlockingTime += Time.deltaTime;
                if (_currentBlockingTime > MAX_BLOCK_HOLD)
                {
                    _blockState = BlockState.WeakeningBlock;
                }
                break;

            case BlockState.WeakeningBlock:
                if (ReturnOnIdle(distance))
                    return;

                _blockPower -= Time.deltaTime * _powerReducer;
                _blockPower = (_blockPower < MIN_BLOCK_POWER) ? MIN_BLOCK_POWER : 0.0f;
                _txtBlockPower.text = $"BlockPower : {_blockPower}";
                _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
                break;


        }
    }

    private bool ReturnOnIdle(float distance)
    {
        if (distance < 0.1f)
        {
            _blockState = BlockState.Idle;
            _blockPower = 0.0f;
            _txtBlockPower.text = "";
            return true;
        }
        return false;
    }

    private bool DetectAnalogMovement()
    {
        var diff = _previousDirection - _blockInputDirection;
        bool value = diff.magnitude > MIN_DIFF_BETWEEN_INPUT;
        //Debug.Log($"{diff.magnitude}");
        //Debug.Log($"{value}");

        _previousDirection = _blockInputDirection;
        return value;
    }

    private void ResetValues()
    {
        _blockState = BlockState.Idle;
        _currentBlockingTime = 0.0f;
        _accumulatedTime = 0.0f;
        _blockPower = 0.0f;
        _txtBlockPower.text = "";
        //_shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);
        _shield.transform.localPosition = Vector2.zero;

    }

}
