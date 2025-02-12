using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitDetector;
    private SlashDirection _attackDirection;
    private AttackStance _attackStance;

    public void GetHitPos(SlashDirection direction, AttackStance stance, bool IsStab) 
    {
        _attackDirection = direction;
        _attackStance = stance;
        Vector2 hitPos = Vector2.zero;
        float hitDistance = 1f;
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
                hitPos.y += hitDistance;
                break;
            case SlashDirection.RightUp:
                hitPos.y += hitDistance / 2;
                hitPos.x += hitDistance / 2;
                break;
            case SlashDirection.RightToLeft:
                hitPos.x -= hitDistance;
                break;
            case SlashDirection.RightDown:
                hitPos.y -= hitDistance / 2;
                hitPos.x += hitDistance / 2;
                break;
            case SlashDirection.StraightDown:
                hitPos.y -= hitDistance;
                break;
            case SlashDirection.LeftDown:
                hitPos.y -= hitDistance / 2;
                hitPos.x -= hitDistance / 2;
                break;
            case SlashDirection.LeftToRight:
                hitPos.x += hitDistance;
                break;
            case SlashDirection.LeftUp:
                hitPos.y += hitDistance / 2;
                hitPos.x -= hitDistance / 2;
                break;
            case SlashDirection.Neutral:
                break;
        }

        _hitDetector.transform.position = hitPos;
    }

    private void DoSlash(float angle, Vector2 startpos, float length)
    {

    }

}
