using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Aiming : MonoBehaviour
{
    private float _lowestPoint;
    private Vector2 _lowestVector;
    private float _highestPoint;
    private Vector2 _highestVector;
    private Direction _direction;

    public void Aim(InputAction.CallbackContext ctx)
    {
        float currentY = ctx.ReadValue<Vector2>().y;

        if (currentY < 0)
        {
            if (currentY < _lowestPoint)
            {
                _lowestPoint = currentY;
                _lowestVector = ctx.ReadValue<Vector2>();
            }
        }

        if (currentY > 0)
        {
            if (currentY > _highestPoint)
            {
                _highestPoint = currentY;
                _highestVector = ctx.ReadValue<Vector2>();
            }
            else Attack();
        }
    }

    private void Attack()
    {
        DetermineHit();
    }

    private void DetermineHit()
    {
        float angle = Vector2.Angle(_lowestVector, _highestVector);
        if (angle < 175 || angle > 185) return;

        float aimAngle = Vector2.Angle(Vector2.zero, _highestVector);
        Debug.Log(aimAngle);
        if (aimAngle > 50 || aimAngle < 40) _direction = Direction.Left; 
        if (aimAngle > 85 || aimAngle < 95) _direction = Direction.Up;
        if (aimAngle > 140 || aimAngle < 130) _direction = Direction.right;
        Debug.Log(_direction);
        _lowestPoint = 0;
        _highestPoint = 0;
        _lowestVector = Vector2.zero;
        _highestVector = Vector2.zero;
    }
}

public enum Direction
{
    Left,
    Up,
    right
}
