using System.Collections;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField]
    private GameObject _slashVisual;
    [SerializeField]
    private GameObject _stabVisual;
    private SlashDirection _attackDirection;
    private AttackStance _attackStance;

    public void GetHitPos(SlashDirection direction, AttackStance stance, bool isStab) 
    {
        _attackDirection = direction;
        _attackStance = stance;
        Vector2 hitPos = Vector2.zero;
        float hitDistance = 1f;
        float angle = 0f;
        switch (_attackStance)
        {
            case AttackStance.Head:
                hitPos.y = 3;
                break;
            case AttackStance.Torso:
                break;
            case AttackStance.Legs:
                hitPos.y = -3;
                break;
        }
        switch (_attackDirection)
        {
            case SlashDirection.Upper:
                if (isStab) 
                {
                    hitPos.y += hitDistance;
                    break;
                }
                hitPos.y -= hitDistance;
                angle = 0;
                break;
            case SlashDirection.RightUp:
                if (isStab) 
                {
                    hitPos.x -= hitDistance / 2;
                    hitPos.y += hitDistance / 2;
                    break;
                }
                hitPos.y -= hitDistance / 2;
                hitPos.x += hitDistance / 2;
                angle = 45;
                break;
            case SlashDirection.RightToLeft:
                if (isStab) 
                {
                    hitPos.x -= hitDistance;
                    break;
                }
                hitPos.x += hitDistance;
                angle = 90;
                break;
            case SlashDirection.RightDown:
                if (isStab)
                {
                    hitPos.y -= hitDistance / 2;
                    hitPos.x -= hitDistance / 2;
                    break;
                }
                hitPos.y += hitDistance / 2;
                hitPos.x += hitDistance / 2;
                angle = 135;
                break;
            case SlashDirection.StraightDown:
                if (isStab)
                {
                    hitPos.y -= hitDistance;
                    break;
                } 
                hitPos.y += hitDistance;
                angle = 180;
                break;
            case SlashDirection.LeftDown:
                if (isStab) 
                {
                    hitPos.y -= hitDistance / 2;
                    hitPos.x += hitDistance / 2;
                    break;
                }
                hitPos.y += hitDistance / 2;
                hitPos.x -= hitDistance / 2;
                angle = -135;
                break;
            case SlashDirection.LeftToRight:
                if (isStab)
                {
                    hitPos.x += hitDistance;
                    break;
                }
                hitPos.x -= hitDistance;
                angle = -90;
                break;
            case SlashDirection.LeftUp:
                if (isStab) 
                {
                    hitPos.y += hitDistance / 2;
                    hitPos.x += hitDistance / 2;
                    break;
                }
                hitPos.y -= hitDistance / 2;
                hitPos.x -= hitDistance / 2;
                angle = -45;
                break;
            case SlashDirection.Neutral:
                break;
        }

        DoAttack(angle, hitPos, isStab);
    }

    private void DoAttack(float angle, Vector2 startpos, bool isStab)
    {
        if (isStab)
        {
            _stabVisual.transform.position = startpos;
            _stabVisual.SetActive(true);
            StartCoroutine(DoResetVisual(1, _stabVisual));
            return;
        }

        _slashVisual.transform.position = startpos;
        _slashVisual.transform.rotation = Quaternion.Euler(0, 0, angle);
        _slashVisual.SetActive(true);
        StartCoroutine(DoResetVisual(1, _slashVisual));
        return;
    }

    private IEnumerator DoResetVisual(float timer, GameObject visual)
    {
        yield return new WaitForSeconds(timer);
        visual.SetActive(false);
    }

}
