using System.Collections;
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

    private float _lowestPoint;
    private Vector2 _lowestVector;
    private float _highestPoint;
    private Vector2 _highestVector;
    private Direction _direction;
    private Height _height = Height.Middle;

    private bool _powerSelected;

    private void Update()
    {
        float power = Vector2.Distance(Vector2.zero, _actionReference.action.ReadValue<Vector2>());
        Debug.Log($"raw {power}");
        power = _PowerScaleMultiplier.Evaluate(power);
        Debug.Log($"changed{power}");

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
        float power = Vector2.Distance(Vector2.zero, _lowestVector);
        
        power = _PowerScaleMultiplier.Evaluate(power) * 10;
        Debug.Log($"Start height:{_height}, direction: {_direction}, Power: {power}");
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
            return;
        }
        if (aimAngle > 45 - _angleMargins && aimAngle < 45 + _angleMargins)
        {
            _direction = Direction.Right;
            return;
        }
        if (aimAngle > 135 - _angleMargins && aimAngle < 135 + _angleMargins)
        {
            _direction = Direction.Left;
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
