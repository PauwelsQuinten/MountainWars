using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private Sprite _sprite;
    private float _spriteHeight;
    private float _zoneHeight;

    private AttackStance _attackStance;
    private AttackType _attackType;

    private BodyParts _legPartInFront;
    private BodyParts _headPartInFront;
    private BodyParts _torsoPartInFront;


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
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and head") ;
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit head and right arm");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
            case AttackType.UpperSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit head and left arm");
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and head");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit torso and left arm");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }

                break;
            case AttackType.DownSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and head");
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit head and right arm");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit torso and right arm");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
            case AttackType.DownSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit head and lef arm");
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and head");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit torso and left arm");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
            case AttackType.HorizontalSlashLeft:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit head and left arm");
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and head");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit torso and left arm");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit right arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
            case AttackType.HorizontalSlashRight:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        if (_headPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and torso");
                        else if (_headPartInFront == BodyParts.RightArm) Debug.Log("hit head and right arm");
                        else if (_headPartInFront == BodyParts.Head) Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit torso and right arm");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit left arm and torso");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
            case AttackType.Stab:
                switch (_attackStance)
                {
                    case AttackStance.Head:
                        Debug.Log("hit head");
                        break;
                    case AttackStance.Torso:
                        if (_torsoPartInFront == BodyParts.LeftArm) Debug.Log("hit left arm");
                        else if (_torsoPartInFront == BodyParts.RightArm) Debug.Log("hit right");
                        else if (_torsoPartInFront == BodyParts.Torso) Debug.Log("hit right");
                        else if (_torsoPartInFront == BodyParts.Weapon) Debug.Log("hit weapon");
                        else if (_torsoPartInFront == BodyParts.Shield) Debug.Log("hit shield");
                        break;
                    case AttackStance.Legs:
                        if (_legPartInFront == BodyParts.LeftLeg) Debug.Log("hit left leg");
                        else if (_legPartInFront == BodyParts.RightLeg) Debug.Log("hit right leg");
                        break;
                }
                break;
        }
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

public enum BodyParts
{
    Torso,
    LeftArm,
    RightArm,
    LeftLeg,
    RightLeg,
    Head,
    Shield,
    Weapon,
    None
}