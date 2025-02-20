using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;


public class AIController : MonoBehaviour
{
    [SerializeField] InputActionReference _testButton;
    private SwordSwing _swordSwing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _testButton.action.performed += Action_performed;
        _swordSwing = GetComponent<SwordSwing>();
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        _swordSwing.StartSwing(AttackType.HorizontalSlashLeft, AttackStance.Head, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
