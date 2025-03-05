using UnityEngine;

public enum ObjectTarget
{
    Player,
    Weapon,
    Shield
}

public class MoveToAction : GoapAction
{
    [SerializeField] private ObjectTarget _MoveTo = ObjectTarget.Player;


}
