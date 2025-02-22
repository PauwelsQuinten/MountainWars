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
    private Shield _shield;
    private AimingInput2 _Sword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _shield = GetComponent<Shield>();
        _Sword = GetComponent<AimingInput2>();

        InitInputActions();
    }

    // Update is called once per frame
    void Update()
    {
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
