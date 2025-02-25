using System.Collections;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private Sprite _sprite;
    private float _spriteHeight;
    private float _zoneHeight;

    private AttackStance _attackStance;
    private AttackType _attackType;


    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
        _spriteHeight = _sprite.rect.height;
        _zoneHeight = _spriteHeight / 3;
    }

    public void HitDetected(GameObject sender)
    {
        if (sender == null) return;

        AimingInput2 aimInput = sender.GetComponent<AimingInput2>();
        _attackStance = aimInput.CurrentStanceState;
        _attackType = aimInput.CurrentAttackType;
    }
}
