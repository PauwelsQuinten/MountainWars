using UnityEngine;


public class WalkAnimate : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Walk(Vector2 direction)
    {
        float angle = (direction.y < 0) ? 1 : 0; 

        _animator.SetFloat("orientation", angle);
        _animator.SetFloat("xInput", direction.x);
        _animator.SetFloat("yInput", direction.y);

    }
}
