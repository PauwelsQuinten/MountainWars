using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class AIController : MonoBehaviour
{
    //[SerializeField] InputActionReference _testButton;
    //[SerializeField] AttackStance _height = AttackStance.Torso;
    //[SerializeField] AttackType _attackType = AttackType.HorizontalSlashRight;
    //[SerializeField] bool _useRandomAttackHeight = false;
    //[SerializeField] bool _useRandomDirection = false;
    //[SerializeField] bool _useRandomAttackTypes = false;
    //private SwordSwing _swordSwing;
    //[SerializeField] private bool _fromRightSide = true;
    //private bool _aiActivated = false;
    private WalkAnimate _animator;

    private CharacterMovement _characterMovement;
    private Blocking _shield;
    private AimingInput2 _Sword;
    private SwordParry _SwordParry;

    private FightStyle _fightStyle = FightStyle.Sword;
    //private Vector2 _storedInput = Vector2.zero;
    public bool IsJumping = false;

    void Start()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        //_shield = GetComponent<Shield>();
        _shield = GetComponent<Blocking>();
        _Sword = GetComponent<AimingInput2>();
        _SwordParry = GetComponent<SwordParry>();
        _animator = GetComponent<WalkAnimate>();
    }


    public void Parried()
    {
        _animator.Parried();

    }
    
    public void Disarmed()
    {
        _animator.Disarmed();
    }


    public void Dodge(Vector2 dodgeDirection)
    {
        if (!GetComponent<Dodge>().CanJump) return;
        IsJumping = true;
        GetComponent<Dodge>().StartJump(dodgeDirection);
    }

    private void AttackGuardMode(bool start, bool guarding)
    {
        if (start)
        {
            if (!GetComponent<HeldEquipment>().HoldSwordAndShield())
                return;

            if (guarding)
            {
                _shield.HoldBlock(true);
                _fightStyle = FightStyle.Combo;
            }
        }
        else
        {
            _shield.HoldBlock(false);

            if (guarding)
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


    //---------------------------------------------------------------
    //SIMPLE EVENT CALL FUNCTIONS

    private void _aimHead_performed(InputAction.CallbackContext obj)
    {
        _Sword.ChangeStance(AttackStance.Head);
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

    public void MoveAction_performed(Vector2 moveInput)
    {
        if (IsJumping)
            return;

        float speed = 1.0f;
        switch (_fightStyle)
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

        _characterMovement.SetInputDirection(moveInput * speed);
    }

    private void _moveAction_Canceled(InputAction.CallbackContext obj)
    {
        _characterMovement.SetInputDirection(Vector2.zero);
    }

    public void AimAction_performed(Vector2 input, FightStyle swordOrShield)
    {
        if (IsJumping)
            return;

        _fightStyle = swordOrShield;
        if (_fightStyle == FightStyle.Shield)
        {
            _shield.SetInputDirection(input);
        }
        else
        {
            //_storedInput = input;
            //_SwordParry.SetSwordMovent(_storedInput);
            _Sword.Direction = input;
            _SwordParry.SetSwordMovent(input);
        }

    }

    public void AimAction_Canceled()
    {
        if (_fightStyle == FightStyle.Shield)
        {
            _shield.SetInputDirection(Vector2.zero);
        }
        else
        {
            //_storedInput = Vector2.zero;
            _Sword.Direction = Vector2.zero;
            _SwordParry.SetSwordMovent(Vector2.zero);
        }
    }

    #endregion MyRegion

    //---------------------------------------------------------------
    //Public FUNCTIONS

    public void ShieldBroke()
    {
        if (_fightStyle == FightStyle.Combo)
        {
            AttackGuardMode(false, false);
        }

        GetComponent<AimingInput2>().SwordBroke();
    }


}

    //private void Action_performed(InputAction.CallbackContext obj)
    //{
    //    if (_aiActivated)
    //    {
    //        CancelInvoke("Action_performed");
    //        _aiActivated = false;
    //        return;
    //    }
    //  
    //    _swordSwing.StartSwing(_attackType, _height, -1);
    //    Invoke("Action_performed", 3.0f);
    //    _aiActivated = true;
    //}
    // private void Action_performed()
    //{
    //
    //    if (_useRandomAttackTypes)
    //    {
    //        _attackType = (Random.Range(0, 2) > 0)? AttackType.Stab : AttackType.HorizontalSlashLeft;
    //    }
    //    if (_useRandomAttackHeight)
    //    {
    //        _height = (AttackStance) Random.Range(-1, 2);
    //    }
    //    if (_useRandomDirection)
    //    {
    //        _fromRightSide = Random.Range(0, 2) == 1;
    //    }
    //    
    //    int direction = _fromRightSide ? -1 : 1;
    //    _swordSwing.StartSwing(_attackType, _height, direction);
    //
    //    float randomFloat = Random.Range(2f, 4f);
    //    Invoke("Action_performed", randomFloat);
    //}