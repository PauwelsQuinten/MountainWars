using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

    private Test2Directions _direction;
    private List<Buttons> _buttonsPressed = new List<Buttons>();
    public void NorthButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.North);

        CheckButon();
    }

    public void EastButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.East);

        CheckButon();
    }

    public void SouthButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.South);

        CheckButon();
    }

    public void WestButtonPressed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _buttonsPressed.Add(Buttons.West);

        CheckButon();
    }

    private void CheckButon()
    {
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
                        _direction = Test2Directions.UpRight;
                        break;
                    case Buttons.South:
                        _direction = Test2Directions.UpDown;
                        break;
                    case Buttons.West:
                        _direction = Test2Directions.UpLeft;
                        break;
                    default: 
                        _direction = Test2Directions.None;
                        _buttonsPressed.Clear();
                        break;
                }
                break;
            case Buttons.East:
                switch (_buttonsPressed[1])
                {
                    case Buttons.North:
                        _direction = Test2Directions.RightUp;
                        break;
                    case Buttons.South:
                        _direction = Test2Directions.RightDown;
                        break;
                    case Buttons.West:
                        _direction = Test2Directions.RightLeft;
                        break;
                    default:
                        _direction = Test2Directions.None;
                        _buttonsPressed.Clear();
                        break;
                }
                break;
            case Buttons.South:
                switch (_buttonsPressed[1])
                {
                    case Buttons.East:
                        _direction = Test2Directions.DownRight;
                        break;
                    case Buttons.North:
                        _direction = Test2Directions.DownUp;
                        break;
                    case Buttons.West:
                        _direction = Test2Directions.DownLeft;
                        break;
                    default:
                        _direction = Test2Directions.None;
                        _buttonsPressed.Clear();
                        break;
                }
                break;
            case Buttons.West:
                switch (_buttonsPressed[1])
                {
                    case Buttons.East:
                        _direction = Test2Directions.LeftRight;
                        break;
                    case Buttons.South:
                        _direction = Test2Directions.LeftDown;
                        break;
                    case Buttons.North:
                        _direction = Test2Directions.LeftUp;
                        break;
                    default:
                        _direction = Test2Directions.None;
                        _buttonsPressed.Clear();
                        break;
                }
                break;
        }

        StopCoroutine(DoTimer(0.2f));
        _buttonsPressed.Clear();
        Debug.Log($"Slash to {_direction} with a power of {_power * _powerMultiplier.action.ReadValue<float>()}");
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
