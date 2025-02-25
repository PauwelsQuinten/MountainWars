using System.Collections.Generic;
using UnityEngine;

public class FindPossibleAttacks : MonoBehaviour
{
    private List<AttackType> _possibleAttacks = new List<AttackType>();

    public List<AttackType> GetpossibleAtack(AttackStance stance, AttackType previous)
    {
        _possibleAttacks.Clear();
        switch (previous)
        {
            case AttackType.UpperSlashRight:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.UpperSlashLeft:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.DownSlashRight:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.DownSlashLeft:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.HorizontalSlashLeft:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.HorizontalSlashRight:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.StraightUp:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightDown);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);

                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.StraightDown:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
                break;
            case AttackType.Stab:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.None);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.None);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.None);
                        break;
                }
                break;
            case AttackType.None:
                switch (stance)
                {
                    case AttackStance.Head:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Torso:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                    case AttackStance.Legs:
                        _possibleAttacks.Add(AttackType.StraightUp);
                        _possibleAttacks.Add(AttackType.UpperSlashLeft);
                        _possibleAttacks.Add(AttackType.UpperSlashRight);
                        _possibleAttacks.Add(AttackType.HorizontalSlashLeft);
                        _possibleAttacks.Add(AttackType.HorizontalSlashRight);
                        _possibleAttacks.Add(AttackType.DownSlashLeft);
                        _possibleAttacks.Add(AttackType.DownSlashRight);
                        _possibleAttacks.Add(AttackType.Stab);
                        break;
                }
            break;
        }

        return _possibleAttacks;
    }
}
