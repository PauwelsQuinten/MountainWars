using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField]
    private GameObject _hitDetector;
    private SlashDirection _AttackDirection;

    public void GetHitPos() 
    {
        Vector2 hitPos = Vector2.zero;
        float hitDistance = 1;
        switch (_AttackDirection)
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

    private void Update()
    {

    }
}
