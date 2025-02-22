using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class AIController : MonoBehaviour
{
    [SerializeField] InputActionReference _testButton;
    [SerializeField] AttackStance _height = AttackStance.Torso;
    [SerializeField] AttackType _attackType = AttackType.HorizontalSlashRight;
    [SerializeField] bool _useRandomAttacks = false;
    private SwordSwing _swordSwing;
    [SerializeField] private bool _fromRightSide = true;
    private bool _aiActivated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _testButton.action.performed += Action_performed;
        _swordSwing = GetComponent<SwordSwing>();
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (_aiActivated)
            return;
      
        _swordSwing.StartSwing(_attackType, _height, -1);
        Invoke("Action_performed", 3.0f);
        _aiActivated = true;
    }
     private void Action_performed()
    {

        if (_useRandomAttacks)
        {
            _height = (AttackStance) Random.Range(-1, 2);
            _fromRightSide = Random.Range(0, 2) == 1;
        }
        
        int direction = _fromRightSide ? -1 : 1;
        _swordSwing.StartSwing(_attackType, _height, direction);

        float randomFloat = Random.Range(2f, 4f);
        Invoke("Action_performed", randomFloat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
