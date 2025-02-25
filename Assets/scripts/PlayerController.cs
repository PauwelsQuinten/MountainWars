using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private CharacterMovement _characterMovement;
    private Blocking _shield;
    private AimingInput2 _Sword;

    private FightStyle _fightStyle = FightStyle.Sword;


    #region Initialising
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        //_shield = GetComponent<Shield>();
        _shield = GetComponent<Blocking>();
        _Sword = GetComponent<AimingInput2>();
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
    }

    #endregion Initialising

    private void _aimAction_performed(InputAction.CallbackContext obj)
    {
        if (_fightStyle == FightStyle.Shield)
        {
            _shield.SetInputDirection(_aimAction.ReadValue<Vector2>());
        }
        else 
        {
            _Sword.Direction = _aimAction.ReadValue<Vector2>();
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
            _Sword.Direction = Vector2.zero;
        }
    }

    #region FightStyle
    private void _attackGuard_performed(InputAction.CallbackContext context)
    {
        if (_fightStyle == FightStyle.Shield)
        {
            _shield.HoldBlock(true);
            _fightStyle = FightStyle.Combo;
        }
    }

    private void _attackGuard_Canceled(InputAction.CallbackContext context)
    {
        _shield.ActivateBlock(false);

        if (_guardAction.IsPressed())
            _fightStyle = FightStyle.Shield;
        else
        {
            _fightStyle = FightStyle.Sword;
            _shield.HoldBlock(false);

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

    private void _gaurdAction_IsInProgress(InputAction.CallbackContext context)
    {
        if (_fightStyle != FightStyle.Combo)
        {
            _fightStyle = FightStyle.Shield;
            _shield.ActivateBlock(true);
        }

    }

    #endregion FightStyle

    //---------------------------------------------------------------
    //SIMPLE EVENT CALL FUNCTIONS

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

    private void _aimHead_performed(InputAction.CallbackContext obj)
    {
        _Sword.ChangeStance(AttackStance.Head);
    }

    private void _aimFeet_performed(InputAction.CallbackContext obj)
    {
        _Sword.ChangeStance(AttackStance.Legs);
    }

    private void _moveAction_performed(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(_moveAction.ReadValue<Vector2>());
    }

    private void _moveAction_Canceled(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(Vector2.zero);
    }

#endregion MyRegion

    //---------------------------------------------------------------
    //PRIVATE FUNCTIONS

    
}
