using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _blockInputDirection = _blockAction.action.ReadValue<Vector2>();
        float distance = _blockInputDirection.sqrMagnitude;

        if (!_useShieldAction.action.IsPressed() || distance < 0.1f)
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
                _blockPower -= (_blockPower <= 0)? 0.0f : Time.deltaTime * _powerReducer;
                
                break;


        }

        

        _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);

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
        _shield.transform.localPosition = new Vector3(_blockInputDirection.x * _radius, _blockInputDirection.y * _radius, 0.0f);

    }

}
