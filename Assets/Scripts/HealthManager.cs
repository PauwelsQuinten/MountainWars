using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float _damageDropOff;
    [SerializeField]
    private SpriteRenderer _health;
    [SerializeField, Range(0.0f, 10.0f)]
    private int _physique;

    private Dictionary<BodyParts, float> _bodyPartMaxHealth = new Dictionary<BodyParts, float>();
    private Dictionary<BodyParts, float> _bodyPartCurrentHealth = new Dictionary<BodyParts, float>();


    private float _currentHealth;
    private float _maxHealth;
    private float _bleedOutRate;

    private void Start()
    {
        InitBodyParts();

        _bodyPartCurrentHealth = _bodyPartMaxHealth;

        foreach (var bodypart in _bodyPartCurrentHealth)
        {
            _maxHealth += bodypart.Value;
        }

        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        _currentHealth -= _bleedOutRate * Time.deltaTime;

        Vector2 healthBarSize = new Vector2(_currentHealth / _maxHealth, 1);
        _health.size = healthBarSize;
        _health.gameObject.transform.localPosition = new Vector3(0 - ((1 - healthBarSize.x) / 2), 0, 0);
    }
    public void GotHit(List<BodyParts> partsHit, float damage)
    {
        int index = 0;
        foreach (BodyParts part in partsHit)
        {
            if (_bodyPartCurrentHealth[part] > 0)
            {
                _bodyPartCurrentHealth[part] -= damage - (index * _damageDropOff);
                _currentHealth -= damage;
            }
            else
            {
                Debug.Log($"{part} has taken too much damage");
                if (part != BodyParts.Head || part != BodyParts.Torso) _bleedOutRate = 15;
                else _currentHealth = 0;
            }
        }
    }

    private void InitBodyParts()
    {
        _bodyPartMaxHealth.Add(BodyParts.Head, 60 + ((60 / 100) * _physique));
        _bodyPartMaxHealth.Add(BodyParts.Torso, 100 + ((100 / 100) * _physique));
        _bodyPartMaxHealth.Add(BodyParts.LeftLeg, 80 + ((80 / 100) * _physique));
        _bodyPartMaxHealth.Add(BodyParts.RightLeg, 80 + ((80 / 100) * _physique));
        _bodyPartMaxHealth.Add(BodyParts.LeftArm, 80 + ((80 / 100) * _physique));
        _bodyPartMaxHealth.Add(BodyParts.RightArm, 80 + ((80 / 100) * _physique));
    }
}
