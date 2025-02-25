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

    public AimingInput2 AimingScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _aimFeet.action.performed += AimFeet_performed;
        _aimHead.action.performed += AimHead_performed;
    }

    private void Update()
    {
        if (AimingScript == null) return;
            
        AimingScript.Direction = _aimAction.action.ReadValue<Vector2>();

        if (_slashDown.action.IsPressed()) AimingScript.SlashDown = true;
        else AimingScript.SlashDown = false;
        if (_slashUp.action.IsPressed()) AimingScript.SlashUp = true;
        else AimingScript.SlashUp = false;
    }
    private void AimHead_performed(InputAction.CallbackContext obj)
    {
        AimingScript.ChangeStance(AttackStance.Head);
    }

    private void AimFeet_performed(InputAction.CallbackContext obj)
    {
        AimingScript.ChangeStance(AttackStance.Legs);
    }
}
