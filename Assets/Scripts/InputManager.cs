using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _aimAction;
    [SerializeField]
    private InputActionReference _aimHead;
    [SerializeField]
    private InputActionReference _aimFeet;
    [SerializeField]
    private InputActionReference _slashUp;
    [SerializeField]
    private InputActionReference _slashDown;

    private AimingInput2 _aimingScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _aimingScript = FindObjectOfType<AimingInput2>();

        _aimFeet.action.performed += AimFeet_performed;
        _aimHead.action.performed += AimHead_performed;
    }

    private void Update()
    {
        _aimingScript.Direction = _aimAction.action.ReadValue<Vector2>();

        if (_slashDown.action.IsPressed()) _aimingScript.SlashDown = true;
        else _aimingScript.SlashDown = false;
        if (_slashUp.action.IsPressed()) _aimingScript.SlashUp = true;
        else _aimingScript.SlashUp = false;
    }
    private void AimHead_performed(InputAction.CallbackContext obj)
    {
        _aimingScript.ChangeStance(AttackStance.Head);
    }

    private void AimFeet_performed(InputAction.CallbackContext obj)
    {
        _aimingScript.ChangeStance(AttackStance.Legs);
    }
}
