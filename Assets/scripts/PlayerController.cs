using System;
using UnityEngine;
using UnityEngine.InputSystem;

//---------------------------------------------------
//PROTOTYPE NOTES
//-first: use RTrigger to lock the guard if held previously.
//-second: use R analog button to lock guard if held previously

public enum FightStyle
{
    Sword,
    Shield,
    Combo,
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _moveAction;
    private InputAction _aimAction;
    private InputAction _guardAction;
    private InputAction _attackGuard;
    private InputAction _aimHeadAction;
    private InputAction _aimFeetAction;
    private InputAction _aimTorsoAction;
    private InputAction _slashUpAction;
    private InputAction _slashDownAction;
    private InputAction _pickupAction;
    private InputAction _dodgeAction;
    private InputAction _cancelShieldHoldAction;

    private CharacterMovement _characterMovement;
    private Blocking _shield;
    private AimingInput2 _Sword;
    private SwordParry _SwordParry;

    private FightStyle _fightStyle = FightStyle.Sword;
    private Vector2 _storedInput = Vector2.zero;
    public bool IsJumping = false;

    #region Initialising
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        //_shield = GetComponent<Shield>();
        _shield = GetComponent<Blocking>();
        _Sword = GetComponent<AimingInput2>();
        _SwordParry = GetComponent<SwordParry>();
    }

    private void OnEnable()
    {
        InitInputActions();
        _guardAction.performed += _gaurdAction_IsInProgress;
        _guardAction.canceled += _gaurdAction_Canceled;
        _attackGuard.performed += _attackGuard_performed;
        _attackGuard.canceled += _attackGuard_Canceled;

        _moveAction.performed += _moveAction_performed;
        _moveAction.canceled += _moveAction_Canceled;
        _aimAction.performed += _aimAction_performed;
        _aimAction.canceled += _aimAction_Canceled;

        _aimFeetAction.performed += _aimFeet_performed;
        _aimHeadAction.performed += _aimHead_performed;
        //_slashDownAction.performed += _slashDown_performed;
        //_slashDownAction.canceled += _slashDown_canceled;
        //_slashUpAction.performed += _slashUp_performed;
        //_slashUpAction.canceled += _slashUp_canceled;

        _pickupAction.performed += _pickup_performed;
        _dodgeAction.performed += _dodge_performed;
        _cancelShieldHoldAction.performed += _CancelShield_Performed;
    }
    private void OnDisable()
    {
        _guardAction.performed -= _gaurdAction_IsInProgress;
        _guardAction.canceled -= _gaurdAction_Canceled;
        _attackGuard.performed -= _attackGuard_performed;
        _attackGuard.canceled -= _attackGuard_Canceled;

        _moveAction.performed -= _moveAction_performed;
        _moveAction.canceled -= _moveAction_Canceled;
        _aimAction.performed -= _aimAction_performed;
        _aimAction.canceled -= _aimAction_Canceled;

        _aimFeetAction.performed -= _aimFeet_performed;
        _aimHeadAction.performed -= _aimHead_performed;
        _slashDownAction.performed -= _slashDown_performed;
        _slashDownAction.canceled -= _slashDown_canceled;
        _slashUpAction.performed -= _slashUp_performed;
        _slashUpAction.canceled -= _slashUp_canceled;

        _pickupAction.performed -= _pickup_performed;
        _dodgeAction.performed -= _dodge_performed;

    }

    private void InitInputActions()
    {
        _moveAction = inputActionAsset.FindAction("Player/Move");
        _aimAction = inputActionAsset.FindAction("Player/Aiming");
        _guardAction = inputActionAsset.FindAction("Player/UseBlock");
        _attackGuard = inputActionAsset.FindAction("Player/AttackOnBlock");
        _aimHeadAction = inputActionAsset.FindAction("Player/SelectUpper");
        _aimFeetAction = inputActionAsset.FindAction("Player/SelectLower");
        _aimTorsoAction = inputActionAsset.FindAction("Player/SelectMiddle");
        _slashUpAction = inputActionAsset.FindAction("Player/SlashUp");
        _slashDownAction = inputActionAsset.FindAction("Player/SlashDown");
        _pickupAction = inputActionAsset.FindAction("Player/Pickup");
        _dodgeAction = inputActionAsset.FindAction("Player/Dodge");
        _cancelShieldHoldAction = inputActionAsset.FindAction("Player/CancelShield");
    }

    #endregion Initialising


    private void _pickup_performed(InputAction.CallbackContext obj)
    {
        GetComponent<HeldEquipment>().SetLookForPickup();
    }
    
    private void _dodge_performed(InputAction.CallbackContext obj)
    {
        if (!GetComponent<Dodge>().CanJump) return;
        IsJumping = true;
        Vector2 jumpInput = _moveAction.ReadValue<Vector2>();
        GetComponent<Dodge>().StartJump(jumpInput);
    }


    private void _aimAction_performed(InputAction.CallbackContext obj)
    {
        if (IsJumping)
            return;

        if (_fightStyle == FightStyle.Shield)
        {
            _shield.SetInputDirection(_aimAction.ReadValue<Vector2>());
        }
        else 
        {
            _storedInput = _aimAction.ReadValue<Vector2>();
            _Sword.Direction = _storedInput;
            _SwordParry.SetSwordMovent(_storedInput);
        }

    }

    private void _aimAction_Canceled(InputAction.CallbackContext obj)
    {
        if (_fightStyle == FightStyle.Shield)
        {
            _shield.SetInputDirection(Vector2.zero);
        }
        else 
        {
            _storedInput = Vector2.zero;
            _Sword.Direction = Vector2.zero;
            _SwordParry.SetSwordMovent(Vector2.zero);
        }
    }


    #region FightStyle (prototype 1)

    private void _attackGuard_performed(InputAction.CallbackContext context)
    {
        if (_fightStyle == FightStyle.Sword)
        {
            _Sword.IsParrying = true;
            _SwordParry.StartParryMode(true);
        }
    }

    private void _attackGuard_Canceled(InputAction.CallbackContext context)
    {
        if (_fightStyle == FightStyle.Sword)
        {
            _Sword.IsParrying = false;
            _SwordParry.StartParryMode(false);
        }
    }
    
    private void _gaurdAction_IsInProgress(InputAction.CallbackContext context)
    {
        if (_fightStyle != FightStyle.Combo)
        {
            _fightStyle = FightStyle.Shield;
            _shield.ActivateBlock(true);
        }
    }

    private void _gaurdAction_Canceled(InputAction.CallbackContext context)
    {
        if(_fightStyle != FightStyle.Combo)
        {
            _fightStyle = FightStyle.Sword;
            _shield.ActivateBlock(false);
            _shield.SetInputDirection(Vector2.zero);
        }
    }

    private void _CancelShield_Performed(InputAction.CallbackContext context)
    {
        if (_fightStyle == FightStyle.Combo)
        {
            AttackGuardMode(false);
            _Sword.ChangeStance(AttackStance.Head);
            _shield.SetInputDirection(Vector2.zero);
        }
    }


    private void AttackGuardMode(bool start)
    {
        if (start)
        {
            if (!GetComponent<HeldEquipment>().HoldSwordAndShield())
                return;

            if (_fightStyle == FightStyle.Shield)
            {
                _shield.HoldBlock(true);
                _fightStyle = FightStyle.Combo;
            }
        }
        else
        {
            _shield.HoldBlock(false);

            if (_guardAction.IsPressed())
            {
                _fightStyle = FightStyle.Shield;
                _shield.ActivateBlock(true);
            }

            else
            {
                _fightStyle = FightStyle.Sword;
                _shield.ActivateBlock(false);

            }

        }

    }

    #endregion FightStyle

    //---------------------------------------------------------------
    //SIMPLE EVENT CALL FUNCTIONS
    #region AnalogClicks (prototype2)

    private void _aimHead_performed(InputAction.CallbackContext obj)
    {
        if (_fightStyle ==FightStyle.Shield)
        {
            if (!GetComponent<HeldEquipment>().HoldSwordAndShield())
                return;

            _fightStyle = FightStyle.Combo;
            _shield.HoldBlock(true);
        }
        else
        {
            AttackGuardMode(false);
            _Sword.ChangeStance(AttackStance.Head);
            _shield.SetInputDirection(Vector2.zero);
        }
    }
 
    private void _aimFeet_performed(InputAction.CallbackContext obj)
    {
        if (_fightStyle == FightStyle.Shield)
        {
            if (!GetComponent<HeldEquipment>().HoldSwordAndShield())
                return;

            _fightStyle = FightStyle.Combo;
            _shield.HoldBlock(true);
        }
    }

    #endregion AnalogClicks

    #region MyRegion
    private void _slashUp_canceled(InputAction.CallbackContext obj)
    {
        _Sword.SlashUp = false;
    }

    private void _slashUp_performed(InputAction.CallbackContext obj)
    {
        _Sword.SlashUp = true;
    }

    private void _slashDown_canceled(InputAction.CallbackContext obj)
    {
        _Sword.SlashDown = false;
    }

    private void _slashDown_performed(InputAction.CallbackContext obj)
    {
        _Sword.SlashDown = true;
    }
    private void _moveAction_performed(InputAction.CallbackContext obj)
    {
        if (IsJumping)
            return;

        float speed = 1.0f;
        switch(_fightStyle)
        {
            case FightStyle.Shield:
                speed = 0.4f;
                break;
            case FightStyle.Combo:
                speed = 0.4f;
                //speed = (_storedInput.magnitude > 0.2f)? 0f : 0.4f;
                break;
            case FightStyle.Sword:
                break;
            default:
                break;
        }

        
        _characterMovement.SetInputDirection(_moveAction.ReadValue<Vector2>() * speed);
    }

    private void _moveAction_Canceled(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(Vector2.zero);
    }

#endregion MyRegion

    //---------------------------------------------------------------
    //Public FUNCTIONS

    public void ShieldBroke()
    {
        if (_fightStyle == FightStyle.Combo)
        {
            AttackGuardMode(false);
        }

        GetComponent<AimingInput2>().SwordBroke();
    }
}
