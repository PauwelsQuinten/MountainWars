using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AimingTest1 : MonoBehaviour
{
    [SerializeField, Range(1f, 20f)]
    private float _angleMargins = 15f;
    [SerializeField]
    private InputActionReference _actionReference;
    [SerializeField]
    private AnimationCurve _PowerScaleMultiplier;
    [SerializeField]
    private GameObject _visualization;
    [SerializeField]
    private TextMeshPro _visualText;

    private float _lowestPoint;
    private Vector2 _lowestVector;
    private float _highestPoint;
    private Vector2 _highestVector;
    private Direction _direction;
    private Height _height = Height.Middle;
    private Test2Directions _testDirection;
    private float _currentPower;

    private bool _powerSelected;

    private void Update()
    {
        float currentY = _actionReference.action.ReadValue<Vector2>().y;

        if (currentY < 0)
        {
            if (currentY < _lowestPoint)
            {
                _lowestPoint = currentY;
                _lowestVector = _actionReference.action.ReadValue<Vector2>();
            }
            else _powerSelected = true;
        }

        if (currentY > 0)
        {
            if (currentY > _highestPoint)
            {
                _highestPoint = currentY;
                _highestVector = _actionReference.action.ReadValue<Vector2>();
            }
            else if (_powerSelected) Attack();
        }
    }

    private void Attack()
    {
        DetermineSide();
        SetVisual();
        _currentPower = Vector2.Distance(Vector2.zero, _lowestVector);

        _currentPower = _PowerScaleMultiplier.Evaluate(_currentPower);
        _currentPower = Mathf.Clamp01(_currentPower);
        Debug.Log($"direction: {_testDirection}, Power: {_currentPower * 10}");
        _lowestPoint = 0;
        _highestPoint = 0;
        _lowestVector = Vector2.zero;
        _highestVector = Vector2.zero;
        _powerSelected = false;
        _height = Height.Middle;
        _direction = Direction.Left;
    }

    private void DetermineSide()
    {
        float angle = Vector2.Angle(_lowestVector, _highestVector);

        if (angle < 180 - _angleMargins || angle > 180 + _angleMargins) return;
        float aimAngle = Vector2.Angle( Vector2.right, _highestVector );
        
        if (aimAngle > 90 - _angleMargins && aimAngle < 90 + _angleMargins)
        {
            _direction = Direction.Middle;

            switch (_height) 
            {
                case Height.Middle:
                    break;
                case Height.Upper:
                    _testDirection = Test2Directions.DownUp;
                    break;
                case Height.Lower:
                    _testDirection = Test2Directions.UpDown;
                    break;
            }
            return;
        }
        if (aimAngle > 45 - _angleMargins && aimAngle < 45 + _angleMargins)
        {
            _direction = Direction.Right;
            switch (_height)
            {
                case Height.Middle:
                    _testDirection = Test2Directions.RightLeft;
                    break;
                case Height.Upper:
                    _testDirection = Test2Directions.RightUp;
                    break;
                case Height.Lower:
                    _testDirection = Test2Directions.RightDown;
                    break;
            }
            return;
        }
        if (aimAngle > 135 - _angleMargins && aimAngle < 135 + _angleMargins)
        {
            _direction = Direction.Left;
            switch (_height)
            {
                case Height.Middle:
                    _testDirection = Test2Directions.LeftRight;
                    break;
                case Height.Upper:
                    _testDirection = Test2Directions.LeftUp;
                    break;
                case Height.Lower:
                    _testDirection = Test2Directions.LeftDown;
                    break;
            }
            return;
        }
        _direction = Direction.None;
    }

    public void SelectUpper(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;
        _height = Height.Upper;
    }

    public void SelectLower(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        _height = Height.Lower;
    }

    private void SetVisual()
    {
        switch (_testDirection)
        {
            case Test2Directions.UpDown:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Test2Directions.UpLeft:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 135);
                break;
            case Test2Directions.UpRight:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, -135);
                break;
            case Test2Directions.LeftUp:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 45);
                break;
            case Test2Directions.LeftRight:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Test2Directions.LeftDown:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 135);
                break;
            case Test2Directions.DownUp:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Test2Directions.DownLeft:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, 45);
                break;
            case Test2Directions.DownRight:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, -135);
                break;
            case Test2Directions.RightUp:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, -45);
                break;
            case Test2Directions.RightLeft:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Test2Directions.RightDown:
                _visualization.transform.rotation = Quaternion.Euler(0, 0, -135);
                break;
        }
        _visualText.text = $"{_testDirection.ToString()}, Power, {_currentPower * 10:F2}";
    }
}

public enum Direction
{
    Left,
    Middle,
    Right,
    None
}

public enum Height
{
    Lower,
    Middle,
    Upper
}
