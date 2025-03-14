using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParryAction : GoapAction
{
    [SerializeField] private EquipmentType _equipmentToParryWith = EquipmentType.Shield;
    [SerializeField] private bool _startFromRight = false;
    private SwordParry _swordParryComp;
    private Blocking _shieldParryComp;
    FightStyle _fightStyle;
    private AIController _aiController;
    private float _progress = 0f;
    [SerializeField] private float _parrySpeed = 0.5f;
    private Vector2 _startVector = Vector2.zero;
    private bool _swingBack = false;
    float _targetProgress = 0;

    public override void StartAction(WorldState currentWorldState)
    {
        base.StartAction(currentWorldState);
        

        if (_equipmentToParryWith == EquipmentType.Shield)
        {
            _fightStyle = FightStyle.Shield;
            _shieldParryComp = currentWorldState.GetOwner().GetComponent<Blocking>();
            _targetProgress = 0.8f;
        }

        else if (_equipmentToParryWith == EquipmentType.Weapon)
        {
            _fightStyle = FightStyle.Sword;
            _swordParryComp = currentWorldState.GetOwner().GetComponent<SwordParry>();
            _swordParryComp.StartParryMode(true);
            _targetProgress = 0.4f;
        }

        float orientation = _startFromRight ?
            currentWorldState.GetOwner().GetComponent<WalkAnimate>().GetOrientation() - Mathf.PI * _targetProgress * 0.5f
            : currentWorldState.GetOwner().GetComponent<WalkAnimate>().GetOrientation() + Mathf.PI * _targetProgress * 0.5f;
        _startVector = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));
        _aiController = currentWorldState.GetOwner().GetComponent<AIController>();
        _aiController.AimAction_performed(_startVector, _fightStyle);
    }

    public override void UpdateAction(WorldState currentWorldState)
    {
        int multiplier = _startFromRight ? 1 : -1;
        _progress += multiplier * Time.deltaTime * _parrySpeed;

        

        float angle = MathF.PI * _progress;
        float cosAngle = MathF.Cos(angle);
        float sinAngle = MathF.Sin(angle);

        Vector2 rotatedVec = new Vector2(
            _startVector.x * cosAngle - _startVector.y * sinAngle,
            _startVector.x * sinAngle + _startVector.y * cosAngle
        );

        rotatedVec = rotatedVec.normalized; // Normalize after rotation

        _aiController.AimAction_performed(rotatedVec, _fightStyle);
    }

    public override bool IsVallid(WorldState currentWorldState)
    {
        foreach(KeyValuePair<AttackType, int> att in currentWorldState._attackCountList)
        {
            if (att.Value > 0 && currentWorldState.TargetCurrentAttack == att.Key)
            {
                Cost = 0f;
                return true;
            }
            
        }
        Cost = 1f;
        

        return true;
        /*return currentWorldState.GetOwner().GetComponent<HeldEquipment>().HoldsEquipment(_equipmentToParryWith)
            && currentWorldState.TargetCurrentAttack == AttackType.Stab;*/
    }

    public override bool IsCompleted(WorldState current, WorldState activeActionDesiredState)
    {
        if (_fightStyle == FightStyle.Sword)
        {
            if (_swingBack && (_progress <= 0.15f && _progress >= -0.15f))
            {
                //_attackComp.Direction = Vector2.zero;
                _aiController.AimAction_performed(Vector2.zero, FightStyle.Sword);
                _startFromRight = !_startFromRight;
                _swordParryComp.StartParryMode(false);
                _swingBack = false;
                _progress = 0;
                ActionCompleted();
                Debug.Log("Parry compleet");
                return true;
            }
            else if (_progress >= _targetProgress || _progress <= -_targetProgress)
            {
                _swingBack = true;

                int multiplier = _startFromRight ? 1 : -1;
                _progress = multiplier * _targetProgress;

                _startFromRight = !_startFromRight;
            }
        }

        else if (_progress >= _targetProgress || _progress <= -_targetProgress)
        {
            //_attackComp.Direction = Vector2.zero;
            _aiController.AimAction_performed(Vector2.zero, FightStyle.Sword);
            _progress = 0;

            ActionCompleted();
            return true;
        }
        return false;

    }
    public override bool IsInterupted(WorldState currentWorldState)
    {
        return base.IsInterupted(currentWorldState);
    }

}
