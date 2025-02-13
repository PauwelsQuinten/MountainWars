using System.Collections;
using UnityEngine;

public class HitDetection2 : MonoBehaviour
{
    [SerializeField]
    private GameObject _slashVisual;
    [SerializeField]
    private GameObject _stabVisual;
    private Test2Directions _attackDirection;
    private AttackStance _attackStance;

    public void GetHitPos(Test2Directions direction, AttackStance stance, bool isStab) 
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
            case Test2Directions.UpDown:
                hitPos.y += hitDistance;
                angle = 180;
                break;
            case Test2Directions.DownUp:
                hitPos.y -= hitDistance;
                angle = 0;
                break;
            case Test2Directions.LeftRight:
                hitPos.x -= hitDistance;
                angle = -90;
                break;
            case Test2Directions.RightLeft:
                hitPos.x += hitDistance;
                angle = 90;
                break;
            case Test2Directions.LeftUp:
                hitPos.x -= hitDistance;
                angle = -45;
                break;
            case Test2Directions.UpLeft:
                hitPos.y += hitDistance;
                angle = 135;
                break;
            case Test2Directions.RightUp:
                hitPos.x += hitDistance;
                angle = 45;
                break;
            case Test2Directions.UpRight:
                hitPos.y += hitDistance;
                angle = -135;
                break;
            case Test2Directions.RightDown:
                hitPos.x += hitDistance;
                angle = 135;
                break;
            case Test2Directions.DownRight:
                hitPos.y -= hitDistance;
                angle = -45;
                break;
            case Test2Directions.LeftDown:
                hitPos.x -= hitDistance;
                angle = -135;
                break;
            case Test2Directions.DownLeft:
                hitPos.y -= hitDistance;
                angle = 45;
                break;
            case Test2Directions.None:
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
