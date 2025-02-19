using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimingTest2 : MonoBehaviour
{
    [SerializeField]
    private float _timer = 0.4f;
    [SerializeField]
    private float _power = 10f;
    [SerializeField]
    private InputActionReference _powerMultiplier;
    [SerializeField]
    private HitDetection2 _hitDetection;
    [SerializeField]
    private InputActionReference _selectHead;
    [SerializeField]
    private InputActionReference _selectLegs;

    private Test2Directions _direction;
    private List<Buttons> _buttonsPressed = new List<Buttons>();
    private AttackStance _attackStance;
    private bool _isStab;
    public void NorthButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.North);

        CheckButon();
        Debug.Log($"Slash to {_direction} with a power of {_power * _powerMultiplier.action.ReadValue<float>()}");
    }

    public void EastButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.East);

        CheckButon();
        Debug.Log($"Slash to {_direction} with a power of {_power * _powerMultiplier.action.ReadValue<float>()}");
    }

    public void SouthButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.South);

        CheckButon();
        Debug.Log($"Slash to {_direction} with a power of {_power * _powerMultiplier.action.ReadValue<float>()}");
    }

    public void WestButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.West);

        CheckButon();
        Debug.Log($"Slash to {_direction} with a power of {_power * _powerMultiplier.action.ReadValue<float>()}");
    }

    private void CheckButon()
    {
        SetStance();
        if (_buttonsPressed.Count < 1) return;

        if (_buttonsPressed.Count == 1)
        {
            StartCoroutine(DoTimer(_timer));
            return;
        }

        switch (_buttonsPressed[0])
        {
            case Buttons.North:
                switch (_buttonsPressed[1])
                {
                    case Buttons.East:
                        _isStab = false;
                        _direction = Test2Directions.UpRight;
                        break;
                    case Buttons.South:
                        _isStab = false;
                        _direction = Test2Directions.UpDown;
                        break;
                    case Buttons.West:
                        _isStab = false;
                        _direction = Test2Directions.UpLeft;
                        break;
                    case Buttons.North:
                        _isStab = true;
                        _direction = Test2Directions.UpDown;
                        break;
                }
                break;
            case Buttons.East:
                switch (_buttonsPressed[1])
                {
                    case Buttons.North:
                        _isStab = false;
                        _direction = Test2Directions.RightUp;
                        break;
                    case Buttons.South:
                        _isStab = false;
                        _direction = Test2Directions.RightDown;
                        break;
                    case Buttons.West:
                        _isStab = false;
                        _direction = Test2Directions.RightLeft;
                        break;
                    case Buttons.East:
                        _isStab = true;
                        _direction = Test2Directions.RightDown;
                        break;
                }
                break;
            case Buttons.South:
                switch (_buttonsPressed[1])
                {
                    case Buttons.East:
                        _isStab = false;
                        _direction = Test2Directions.DownRight;
                        break;
                    case Buttons.North:
                        _isStab = false;
                        _direction = Test2Directions.DownUp;
                        break;
                    case Buttons.West:
                        _isStab = false;
                        _direction = Test2Directions.DownLeft;
                        break;
                    case Buttons.South:
                        _isStab = true;
                        _direction = Test2Directions.DownRight;
                        break;
                }
                break;
            case Buttons.West:
                switch (_buttonsPressed[1])
                {
                    case Buttons.East:
                        _isStab = false;
                        _direction = Test2Directions.LeftRight;
                        break;
                    case Buttons.South:
                        _isStab = false;
                        _direction = Test2Directions.LeftDown;
                        break;
                    case Buttons.North:
                        _isStab = false;
                        _direction = Test2Directions.LeftUp;
                        break;
                    case Buttons.West:
                        _isStab = true;
                        _direction = Test2Directions.LeftDown;
                        break;
                }
                break;
        }

        _hitDetection.GetHitPos(_direction, _attackStance, _isStab);
        _attackStance = AttackStance.Hips;
        StopCoroutine(DoTimer(0.2f));
        _buttonsPressed.Clear();
    }

    private void SetStance()
    {
        if (_selectHead.action.IsPressed()) _attackStance = AttackStance.Head;
        else if (_selectLegs.action.IsPressed()) _attackStance = AttackStance.Legs;
        else _attackStance = AttackStance.Hips;
    }

    private IEnumerator DoTimer(float timer)
    {
        yield return new WaitForSeconds(timer);
        _buttonsPressed.Clear();
    }
}

public enum Test2Directions
{
    UpDown,
    DownUp,
    LeftRight,
    RightLeft,
    LeftUp,
    UpLeft,
    RightUp,
    UpRight,
    RightDown,
    DownRight,
    LeftDown,
    DownLeft,
    None
}

public enum Buttons
{
    North,
    East,
    South,
    West
}
