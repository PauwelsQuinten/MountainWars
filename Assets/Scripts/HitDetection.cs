using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    private TextMeshPro _AttackMessage;


    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _AttackMessage = GameObject.Find("action power").GetComponent<TextMeshPro>();
        _spriteHeight = _sprite.bounds.size.y / 2;
        _zoneHeight = _spriteHeight / 3;
        _hitbox.SetActive(false);
        _arrow.SetActive(false);
        _AttackMessage.gameObject.SetActive(false);
    }

    public void HitDetected(GameObject sender)
    {
        if (sender == null) return;

        AimingInput2 aimInput = sender.GetComponent<AimingInput2>();
        _attackStance = aimInput.CurrentStanceState;
        _attackType = aimInput.CurrentAttackType;

        DetermineAtackType();
    }

    private void DetermineAtackType()
    {
        switch (_attackType)
        {
            case AttackType.UpperSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) { Debug.Log("hit left arm and head"); _AttackMessage.text = "hit left arm and head"; }
                        else if (_headPartInFront == BodyParts.RightArm) { Debug.Log("hit head and right arm"); _AttackMessage.text = "hit head and right arm"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) { Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) { Debug.Log("hit torso and right arm"); _AttackMessage.text = "hit torso and right arm"; }
                        else if (_torsoPartInFront == BodyParts.Torso) { Debug.Log("hit Right arm and torso"); _AttackMessage.text = "hit Right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
            case AttackType.UpperSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) { Debug.Log("hit head and left arm"); _AttackMessage.text = "hit head and left arm"; }
                        else if (_headPartInFront == BodyParts.RightArm) { Debug.Log("hit right arm and head"); _AttackMessage.text = "hit right arm and head"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit torso and left arm"); _AttackMessage.text = "hit torso and left arm"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm and torso"); _AttackMessage.text = "hit right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }

                break;
            case AttackType.DownSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) {Debug.Log("hit left arm and head"); _AttackMessage.text = "hit left arm and head"; }
                        else if (_headPartInFront == BodyParts.RightArm) {Debug.Log("hit head and right arm"); _AttackMessage.text = "hit head and right arm"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit torso and right arm"); _AttackMessage.text = "hit torso and right arm"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit right arm and torso"); _AttackMessage.text = "hit right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
            case AttackType.DownSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) {Debug.Log("hit head and lef arm"); _AttackMessage.text = "hit head and lef arm"; }
                        else if (_headPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm and head"); _AttackMessage.text = "hit right arm and head"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit torso and left arm"); _AttackMessage.text = "hit torso and left arm"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm and torso"); _AttackMessage.text = "hit right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
            case AttackType.HorizontalSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) {Debug.Log("hit head and left arm"); _AttackMessage.text = "hit head and left arm"; }
                        else if (_headPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm and head"); _AttackMessage.text = "hit right arm and head"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit torso and left arm"); _AttackMessage.text = "hit torso and left arm"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm and torso"); _AttackMessage.text = "hit right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
            case AttackType.HorizontalSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_headPartInFront == BodyParts.RightArm) {Debug.Log("hit head and right arm"); _AttackMessage.text = "hit head and right arm"; }
                        else if (_headPartInFront == BodyParts.Head) { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit left arm and torso"); _AttackMessage.text = "hit left arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit torso and right arm"); _AttackMessage.text = "hit torso and right arm"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit right arm and torso"); _AttackMessage.text = "hit right arm and torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
            case AttackType.Stab:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        { Debug.Log("hit head"); _AttackMessage.text = "hit head"; }
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) {Debug.Log("hit left arm"); _AttackMessage.text = "hit left arm"; }
                        else if (_torsoPartInFront == BodyParts.RightArm) {Debug.Log("hit right arm"); _AttackMessage.text = "hit right arm"; }
                        else if (_torsoPartInFront == BodyParts.Torso) {Debug.Log("hit torso"); _AttackMessage.text = "hit torso"; }
                        else if (_torsoPartInFront == BodyParts.Weapon) { Debug.Log("hit weapon"); _AttackMessage.text = "hit weapon"; }
                        else if (_torsoPartInFront == BodyParts.Shield) { Debug.Log("hit shield"); _AttackMessage.text = "hit shield"; }
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) { Debug.Log("hit left leg"); _AttackMessage.text = "hit left leg"; }
                        else if (_legPartInFront == BodyParts.RightLeg) { Debug.Log("hit right leg"); _AttackMessage.text = "hit right leg"; }
                        break;
                }
                break;
        }

        SetHitbox();
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
        _AttackMessage.gameObject.SetActive(true);

        if(_resetHitbox != null) StopCoroutine(_resetHitbox);
        _resetHitbox = StartCoroutine(ResetHitBox(0.5f));
    }

    private IEnumerator ResetHitBox(float timer)
    {
        yield return new WaitForSeconds(timer);
        _hitbox.SetActive(false);
        _arrow.SetActive(false);
        _AttackMessage.gameObject.SetActive(false);
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
