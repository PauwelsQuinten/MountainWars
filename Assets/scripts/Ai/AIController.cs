using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class AIController : MonoBehaviour
{
    [SerializeField] InputActionReference _testButton;
    [SerializeField] AttackStance _height = AttackStance.Torso;
    [SerializeField] AttackType _attackType = AttackType.HorizontalSlashRight;
    [SerializeField] bool _useRandomAttackHeight = false;
    [SerializeField] bool _useRandomDirection = false;
    private SwordSwing _swordSwing;
    private WalkAnimate _animator;
    [SerializeField] private bool _fromRightSide = true;
    private bool _aiActivated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _testButton.action.performed += Action_performed;
        _swordSwing = GetComponent<SwordSwing>();
        _animator = GetComponent<WalkAnimate>();
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (_aiActivated)
        {
            CancelInvoke("Action_performed");
            _aiActivated = false;
            return;
        }
      
        _swordSwing.StartSwing(_attackType, _height, -1);
        Invoke("Action_performed", 3.0f);
        _aiActivated = true;
    }
     private void Action_performed()
    {

        if (_useRandomAttackHeight)
        {
            _height = (AttackStance) Random.Range(-1, 2);
        }
        if (_useRandomDirection)
        {
            _fromRightSide = Random.Range(0, 2) == 1;
        }
        
        int direction = _fromRightSide ? -1 : 1;
        _swordSwing.StartSwing(_attackType, _height, direction);

        float randomFloat = Random.Range(2f, 4f);
        Invoke("Action_performed", randomFloat);
    }

    public void Parried()
    {
        _animator.Parried();

    }

    // Update is called once per frame
    void Update()
    {
    }
}
