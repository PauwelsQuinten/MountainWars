using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitbox;
    [SerializeField]
    private GameObject _arrow;
    private SpriteRenderer _sprite;
    private float _spriteHeight;
    private float _zoneHeight;

    private AttackStance _attackStance;
    private AttackType _attackType;

    private BodyParts _legPartInFront;
    private BodyParts _headPartInFront;
    private BodyParts _torsoPartInFront;

    private Coroutine _resetHitbox;

    private HealthManager _healthManager;
    private List<BodyParts> _partsHit = new List<BodyParts>();

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _healthManager = GetComponent<HealthManager>();
        _spriteHeight = _sprite.bounds.size.y / 2;
        _zoneHeight = _spriteHeight / 3;
        _hitbox.SetActive(false);
        _arrow.SetActive(false);
    }

    public void HitDetected(GameObject sender, float damage)
    {
        if (sender == null) return;

        AimingInput2 aimInput = sender.GetComponent<AimingInput2>();
        _attackStance = aimInput.CurrentStanceState;
        _attackType = aimInput.CurrentAttackType;

        DetermineAtackType();
        SetHitbox();
        _healthManager.GotHit(_partsHit, damage);
    }

    private void DetermineAtackType()
    {
        _partsHit.Clear();
        switch (_attackType)
        {
            case AttackType.UpperSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) 
                        { 
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_headPartInFront == BodyParts.Head)
                        {
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm)
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon) 
                        {
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        {
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        {
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        break;
                }
                break;
            case AttackType.UpperSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) 
                        {
                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_headPartInFront == BodyParts.RightArm)
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.Head)
                        { 
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm)
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon) 
                        { 
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        {
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        {
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        {
                            _partsHit.Add(BodyParts.RightLeg);
                        }
                        break;
                }

                break;
            case AttackType.DownSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_headPartInFront == BodyParts.Head)
                        {
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon) 
                        { 
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        { 
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        { 
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg)
                        {
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        break;
                }
                break;
            case AttackType.DownSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm)
                        {

                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_headPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.Head) 
                        { 
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) 
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon)
                        {
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        {
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg)
                        {
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        {
                            _partsHit.Add(BodyParts.RightLeg);
                        }
                        break;
                }
                break;
            case AttackType.HorizontalSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_headPartInFront == BodyParts.RightArm)
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.Head) 
                        {
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso)
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon)
                        { 
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        { 
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        {
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        {
                            _partsHit.Add(BodyParts.RightLeg);
                        }
                        break;
                }
                break;
            case AttackType.HorizontalSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) 
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Head);
                        }
                        else if (_headPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.Head);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_headPartInFront == BodyParts.Head)
                        {
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm)
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.Torso);
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {

                            _partsHit.Add(BodyParts.RightArm);
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon) 
                        { 

                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        { 

                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        { 

                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        { 

                            _partsHit.Add(BodyParts.RightLeg);
                        }
                        break;
                }
                break;
            case AttackType.Stab:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        { 
                            _partsHit.Add(BodyParts.Head);
                        }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) 
                        {
                            _partsHit.Add(BodyParts.LeftArm);
                        }
                        else if (_torsoPartInFront == BodyParts.RightArm) 
                        {
                            _partsHit.Add(BodyParts.RightArm);
                        }
                        else if (_torsoPartInFront == BodyParts.Torso) 
                        {
                            _partsHit.Add(BodyParts.Torso);
                        }
                        else if (_torsoPartInFront == BodyParts.Weapon) 
                        { 
                            _partsHit.Add(BodyParts.Weapon);
                        }
                        else if (_torsoPartInFront == BodyParts.Shield) 
                        { 
                            _partsHit.Add(BodyParts.Shield);
                        }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) 
                        {
                            _partsHit.Add(BodyParts.LeftLeg);
                        }
                        else if (_legPartInFront == BodyParts.RightLeg) 
                        {
                            _partsHit.Add(BodyParts.RightLeg);
                        }
                        break;
                }
                break;
        }
    }

    private void SetHitbox()
    {
        Vector3 newHitboxPos;
        newHitboxPos = _sprite.gameObject.transform.position;
        if (_hitbox == null) return;

        switch (_attackStance)
        {
            case AttackStance.Head:
                newHitboxPos.y += _spriteHeight / 2;
                break;
            case AttackStance.Torso:
                break;
            case AttackStance.Legs:
                newHitboxPos.y -= _spriteHeight / 2;
                break;
        }
        _hitbox.transform.position = newHitboxPos;
        SetHitboxAngle();
    }

    private void SetHitboxAngle()
    {
        switch (_attackType)
        {
            case AttackType.HorizontalSlashLeft:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f + 180f);
                break;
            case AttackType.HorizontalSlashRight:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case AttackType.DownSlashRight:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                break;
            case AttackType.UpperSlashRight:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                break;
            case AttackType.DownSlashLeft:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f + 180f);
                break;
            case AttackType.UpperSlashLeft:
                _hitbox.transform.localScale = new Vector3(1.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f - 180f);
                break;
            case AttackType.Stab:
                _hitbox.transform.localScale = new Vector3(0.4f, 0.4f, 0.0f);
                _hitbox.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            default: break;
        }
        _arrow.transform.position = _hitbox.transform.position;
        Vector3 arrowAngle = _hitbox.transform.eulerAngles;
        arrowAngle.z -= 90;
        _arrow.transform.rotation = Quaternion.Euler(arrowAngle);

        _hitbox.SetActive(true);
        _arrow.SetActive(true);

        if(_resetHitbox != null) StopCoroutine(_resetHitbox);
        _resetHitbox = StartCoroutine(ResetHitBox(0.5f));
    }

    private IEnumerator ResetHitBox(float timer)
    {
        yield return new WaitForSeconds(timer);
        _hitbox.SetActive(false);
        _arrow.SetActive(false);
    }

    public void SetPartInFrontLeg(BodyParts partInFront)
    {
        _legPartInFront = partInFront;
    }
    public void SetPartInFrontHead(BodyParts partInFront)
    {
        _headPartInFront = partInFront;
    }
    public void SetPartInFrontTorso(BodyParts partInFront)
    {
        _torsoPartInFront = partInFront;
    }
}
