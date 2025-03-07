using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    
    [SerializeField]
    private float _movementSpeed;
    //[SerializeField]
    //private float _rotationSpeed;

    private CharacterController _controller;
    private Vector2 _movement;
    private float _rotationCutOff = 45f / 2f;
    public  CharacterOrientation CurrentCharacterOrientation = CharacterOrientation.North;
    private WalkAnimate _walkAnimate;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _walkAnimate = GetComponent<WalkAnimate>();
    }
    private void Update()
    {
        Move();
        //SetRotation();
    }

   
    public void SetInputDirection(Vector2 direction)
    {
        _movement = direction;
    }

    private void Move()
    {
        _controller.Move((_movement * _movementSpeed) * Time.deltaTime);
        _walkAnimate.Walk(_movement);
    }

    private void SetRotation()
    {
        float currentAngle = Mathf.Atan2(_movement.y, _movement.x);
        float currentAngleDegree = currentAngle * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, (float)_currentCharacterOrientation);

        if (_movement == Vector2.zero) return;
        if (currentAngleDegree < 0f + _rotationCutOff && currentAngleDegree >= 0f || currentAngleDegree > 0f - _rotationCutOff && currentAngleDegree <= 0f)
            CurrentCharacterOrientation = CharacterOrientation.East;
        else if (currentAngleDegree < 45f + _rotationCutOff && currentAngleDegree > 45f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.NorthEast;
        else if (currentAngleDegree < 90f + _rotationCutOff && currentAngleDegree > 90f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.North;
        else if (currentAngleDegree < 135f + _rotationCutOff && currentAngleDegree > 135f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.NorthWest;
        else if (currentAngleDegree <= 180f  && currentAngleDegree >= 180f - _rotationCutOff || currentAngleDegree < -180f + _rotationCutOff && currentAngleDegree <= -180f)
            CurrentCharacterOrientation = CharacterOrientation.West;
        else if (currentAngleDegree < -135f + _rotationCutOff && currentAngleDegree > -135f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.SouthWest;
        else if (currentAngleDegree < -90f + _rotationCutOff && currentAngleDegree > -90f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.South;
        else if (currentAngleDegree < -45f + _rotationCutOff && currentAngleDegree > -45f - _rotationCutOff)
            CurrentCharacterOrientation = CharacterOrientation.SouthEast;
    }
}


public enum CharacterOrientation
{
    North = 0,
    NorthEast = -45,
    East = -90,
    SouthEast = -135,
    South = 180,
    SouthWest = 135,
    West = 90,
    NorthWest = 45,
}
