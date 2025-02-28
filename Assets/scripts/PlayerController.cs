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
    Combo
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _moveAction;
    private InputAction _aimAction;
    private InputAction _guardAction;
    private InputAction _attackGuard;
    private InputAction _aimHead;
    private InputAction _aimFeet;
    private InputAction _aimTorso;
    private InputAction _slashUp;
    private InputAction _slashDown;
    private InputAction _pickup;

    private CharacterMovement _characterMovement;
    private Blocking _shield;
    private AimingInput2 _Sword;
    private SwordParry _SwordParry;

    private FightStyle _fightStyle = FightStyle.Sword;
    private Vector2 _storedInput = Vector2.zero;

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

        _aimFeet.performed += _aimFeet_performed;
        _aimHead.performed += _aimHead_performed;
        _slashDown.performed += _slashDown_performed;
        _slashDown.canceled += _slashDown_canceled;
        _slashUp.performed += _slashUp_performed;
        _slashUp.canceled += _slashUp_canceled;

        _pickup.performed += _pickup_performed;
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

        _aimFeet.performed -= _aimFeet_performed;
        _aimHead.performed -= _aimHead_performed;
        _slashDown.performed -= _slashDown_performed;
        _slashDown.canceled -= _slashDown_canceled;
        _slashUp.performed -= _slashUp_performed;
        _slashUp.canceled -= _slashUp_canceled;

        _pickup.performed -= _pickup_performed;

    }

    private void InitInputActions()
    {
        _moveAction = inputActionAsset.FindAction("Player/Move");
        _aimAction = inputActionAsset.FindAction("Player/Aiming");
        _guardAction = inputActionAsset.FindAction("Player/UseBlock");
        _attackGuard = inputActionAsset.FindAction("Player/AttackOnBlock");
        _aimHead = inputActionAsset.FindAction("Player/SelectUpper");
        _aimFeet = inputActionAsset.FindAction("Player/SelectLower");
        _aimTorso = inputActionAsset.FindAction("Player/SelectMiddle");
        _slashUp = inputActionAsset.FindAction("Player/SlashUp");
        _slashDown = inputActionAsset.FindAction("Player/SlashDown");
        _pickup = inputActionAsset.FindAction("Player/Pickup");
    }

    #endregion Initialising


    private void _pickup_performed(InputAction.CallbackContext obj)
    {
        GetComponent<HeldEquipment>().SetLookForPickup();
    }

    private void _aimAction_performed(InputAction.CallbackContext obj)
    {
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
            _SwordParry.StartParryMode(true);
        else
            AttackGuardMode(true);
    }

    private void _attackGuard_Canceled(InputAction.CallbackContext context)
    {
        if (_fightStyle == FightStyle.Sword)
            _SwordParry.StartParryMode(false);
        else
            AttackGuardMode(false);
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
        else
        {
            AttackGuardMode(false);
            _Sword.ChangeStance(AttackStance.Legs);
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
        float speed = 1.0f;
        switch(_fightStyle)
        {
            case FightStyle.Shield:
                speed = 0.4f;
                break;
            case FightStyle.Combo:
                speed = (_storedInput.magnitude > 0.2f)? 0f : 0.4f;
                break;
            case FightStyle.Sword:
                speed = (_storedInput.magnitude > 0.2f)? 0f : 1f;
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
