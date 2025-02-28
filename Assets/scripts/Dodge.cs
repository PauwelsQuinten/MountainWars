using System.Collections;
using UnityEngine;

public class Dodge : MonoBehaviour
{
    [SerializeField]private float _jumpDistanceMultiplier = 2f;
    [SerializeField]private float _jumpSpeed = 1f;
    [SerializeField]private float _cooldownTime = 1f;
    private Vector3 _startLocation;
    private bool _inCooldown = false;
    Coroutine _coroutine = null;

    public void StartJump(Vector3 direction )
    {
        if (_inCooldown )
            return;

        _inCooldown = true;
        _startLocation = transform.position;
        float distance = direction.magnitude * _jumpDistanceMultiplier;
        direction = direction.normalized;

        _coroutine = StartCoroutine(Jumping(direction, distance));
    }

    public void StartCooldown()
    {
        StartCoroutine(CoolDown());

    }

    IEnumerator Jumping(Vector3 direction, float distance )
    {
        while(Vector3.Distance(_startLocation, transform.position) < distance)
        {
            transform.position += _jumpSpeed * Time.deltaTime * direction;
            yield return null;
        }
        GetComponent<PlayerController>().JumpFinished();
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_cooldownTime);
        _inCooldown = false;
/*
        if( _coroutine != null)
            StopCoroutine(_coroutine);*/
    }

    
}
