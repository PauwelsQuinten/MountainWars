using System.Collections;
using UnityEngine;

public class Dodge : MonoBehaviour
{
    [SerializeField]private float _jumpDistanceMultiplier = 2f;
    [SerializeField]private float _jumpSpeed = 1f;
    [SerializeField]private float _cooldownTime = 1f;
    private Vector3 _startLocation;
    private Coroutine _Cooldown;
    private Coroutine _jumping;
    public bool CanJump = true;

    public void StartJump(Vector3 direction )
    {
        if (!CanJump) return;
        CanJump = false;
        _startLocation = transform.position;
        float distance = direction.magnitude * _jumpDistanceMultiplier;
        direction = direction.normalized;

        _jumping = StartCoroutine(Jumping(direction, distance));
    }

    IEnumerator Jumping(Vector3 direction, float distance )
    {
        while(Vector3.Distance(_startLocation, transform.position) < distance)
        {
            transform.position += _jumpSpeed * Time.deltaTime * direction;
            yield return null;
        }
        GetComponent<PlayerController>().IsJumping = false;
        _Cooldown = StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_cooldownTime);
        CanJump = true;
    }
}
