using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _moveAction;
    private InputAction _aimAction;
    private InputAction _guardAction;
    private InputAction _aimHead;
    private InputAction _aimFeet;
    private InputAction _aimTorso;
    private InputAction _slashUp;
    private InputAction _slashDown;

    private CharacterMovement _characterMovement;
    private Blocking _shield;
    private AimingInput2 _Sword;

    private bool _useShield = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        _moveAction.performed += _moveAction_performed;
        _moveAction.canceled += _moveAction_Canceled;
        _aimAction.performed += _aimAction_performed;
        _aimAction.canceled += _aimAction_Canceled;
    }

    private void OnDisable()
    {
        _guardAction.performed -= _gaurdAction_IsInProgress;
        _guardAction.canceled -= _gaurdAction_Canceled;
        _moveAction.performed -= _moveAction_performed;
        _moveAction.canceled -= _moveAction_Canceled;
        _aimAction.performed -= _aimAction_performed;
        _aimAction.canceled -= _aimAction_Canceled;
    }
    private void _aimAction_performed(InputAction.CallbackContext obj)
    {
        if (_useShield)
        {
            _shield.SetInputDirection(_aimAction.ReadValue<Vector2>());
        }
        else
        {
            _Sword.SetInputDirection(_aimAction.ReadValue<Vector2>());
        }

    }

    private void _aimAction_Canceled(InputAction.CallbackContext obj)
    {
        if (_useShield)
        {
            _shield.SetInputDirection(Vector2.zero);
        }
        else
        {
            _Sword.SetInputDirection(Vector2.zero);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void _moveAction_performed(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(_moveAction.ReadValue<Vector2>());
    }

    private void _moveAction_Canceled(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(Vector2.zero);
    }

    private void _gaurdAction_Canceled(InputAction.CallbackContext context)
    {
        _useShield = false;
        _shield.ActivateBlock(_useShield);
    }

    private void _gaurdAction_IsInProgress(InputAction.CallbackContext context)
    {
        _useShield = true;
        _shield.ActivateBlock(_useShield);
    }

    //---------------------------------------------------------------
    //PRIVATE FUNCTIONS

    private void InitInputActions()
    {
        _moveAction = inputActionAsset.FindAction("Player/Move");
        _aimAction = inputActionAsset.FindAction("Player/Aiming");
        _guardAction = inputActionAsset.FindAction("Player/UseBlock");
        _aimHead = inputActionAsset.FindAction("Player/SelectUpper");
        _aimFeet = inputActionAsset.FindAction("Player/SelectLower");
        _aimTorso = inputActionAsset.FindAction("Player/SelectMiddle");
        _slashUp = inputActionAsset.FindAction("Player/SlashUp");
        _slashDown = inputActionAsset.FindAction("Player/SlashDown");
    }

    
}
