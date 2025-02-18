using UnityEngine;

public class AttackTimer : MonoBehaviour
{
    private float _currentTime = 0.0f;
    [SerializeField] private float TimeToAttack = 5.0f;
    Animator _animator;
    [Range(0, 3)]
    [SerializeField] private int _maxAttackTypes = 3;
    private int _nextAttackindexer = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTime < TimeToAttack)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _currentTime = 0.0f;
            switch(_nextAttackindexer)
            {
                case 0:
                    _animator.SetTrigger("Attack");
                    break;
                case 1:
                    _animator.SetTrigger("AttackUp");
                    break;
                case 2:
                    _animator.SetTrigger("AttackDown");
                    break;
                default:break;

            }
            _nextAttackindexer = Random.Range(0, _maxAttackTypes);
            
        }

    }
}
