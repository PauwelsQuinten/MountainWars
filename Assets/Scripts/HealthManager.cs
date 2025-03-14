using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float _damageDropOff;
    [SerializeField]
    private SpriteRenderer _healthBar;
    [SerializeField]
    private SpriteRenderer _bloodBar;
    [Range(0.0f, 10.0f)]
    public int Physique;
    [SerializeField]
    private int _baseLimbHealth;
    [SerializeField]
    private float _maxBlood;
    [SerializeField]
    private float _bleedOutSpeed;
    [SerializeField]
    private GameObject _headBones;
    [SerializeField]
    private GameObject _torsoBones;
    [SerializeField]
    private GameObject _leftArmBones;
    [SerializeField]
    private GameObject _rightArmBones;
    [SerializeField]
    private GameObject _leftLegBones;
    [SerializeField]
    private GameObject _rightLegBones;

    private Dictionary<BodyParts, float> _bodyPartMaxHealth = new Dictionary<BodyParts, float>();
    private Dictionary<BodyParts, float> _bodyPartCurrentHealth = new Dictionary<BodyParts, float>();


    private float _currentHealth;
    private float _maxHealth;
    private float _bleedOutRate;
    private float _currentBlood;

    private void Start()
    {
        InitBodyParts();

        foreach (var bodypart in _bodyPartCurrentHealth)
        {
            _maxHealth += bodypart.Value;
        }

        _currentHealth = _maxHealth;
        _currentBlood = _maxBlood;
    }

    private void Update()
    {
        _currentBlood -= _bleedOutRate * Time.deltaTime;

        Vector2 healthBarSize = new Vector2(_currentHealth / _maxHealth, 1);
        _healthBar.size = healthBarSize;
        _healthBar.gameObject.transform.localPosition = new Vector3(0 - ((1 - healthBarSize.x) / 2), 0, 0);

        Vector2 bloodBarSize = new Vector2(_currentBlood / _maxBlood, 1);
        _bloodBar.size = bloodBarSize;
        _bloodBar.gameObject.transform.localPosition = new Vector3(0 - ((1 - bloodBarSize.x) / 2), 0, 0);

        if (_currentHealth <= 0 || _currentBlood <= 0) Destroy(gameObject);
    }
    public void GotHit(List<BodyParts> partsHit, float damage)
    {
        int index = 0;
        int damageTaken = (int)damage;
        foreach (BodyParts part in partsHit)
        {
            if (_bodyPartCurrentHealth[part] > 0)
            {
                damageTaken -= (int)(index * _damageDropOff);
                _bodyPartCurrentHealth[part] -= damageTaken;
                _currentHealth -= damage;
                index++;

                if(_bodyPartCurrentHealth[part] <= 0)
                {
                    if (part == BodyParts.Head)
                        _currentHealth = 0;
                    else if(part == BodyParts.Torso)
                        _bleedOutRate += _bleedOutSpeed * 1.5f;
                    else 
                        _bleedOutRate += _bleedOutSpeed;

                    if (_bleedOutRate > 0)
                    {
                        GetComponent<WorldState>().IsBleeding = true;
                    }
                }
            }
            else
            {
                Debug.Log($"{part} has taken too much damage");
                ShowBones( part );
            }
        }
    }

    private void ShowBones(BodyParts part)
    {
        switch (part)
        {
            case BodyParts.Torso:
                _torsoBones.SetActive(true);
                break;
            case BodyParts.LeftArm:
                _leftArmBones.SetActive(true);
                break;
            case BodyParts.RightArm:
                _rightArmBones.SetActive(true);
                break;
            case BodyParts.LeftLeg:
                _leftLegBones.SetActive(true);
                break;
            case BodyParts.RightLeg:
                _rightLegBones.SetActive(true);
                break;
            case BodyParts.Head:
                _headBones.SetActive(true);
                break;
        }
    }

    private void InitBodyParts()
    {
        _bodyPartMaxHealth.Add(BodyParts.Head, (int)((_baseLimbHealth * 0.75f) + (((_baseLimbHealth * 0.75f) / 100) * Physique)));
        _bodyPartMaxHealth.Add(BodyParts.Torso, (int)((_baseLimbHealth * 2) + (((_baseLimbHealth * 2) / 100) * Physique))); 
        _bodyPartMaxHealth.Add(BodyParts.LeftLeg, (int)(_baseLimbHealth + ((_baseLimbHealth / 100) * Physique)));
        _bodyPartMaxHealth.Add(BodyParts.RightLeg, (int)(_baseLimbHealth + ((_baseLimbHealth / 100) * Physique)));
        _bodyPartMaxHealth.Add(BodyParts.LeftArm, (int)(_baseLimbHealth + ((_baseLimbHealth / 100) * Physique)));
        _bodyPartMaxHealth.Add(BodyParts.RightArm, (int)(_baseLimbHealth + ((_baseLimbHealth / 100) * Physique)));

        _bodyPartCurrentHealth = _bodyPartMaxHealth;
    }

    public float GetHealth()
    {
        return _currentHealth / _maxHealth;
    }

    public void PatchUpBleeding()
    {
        _bleedOutRate = 0f;
        GetComponent<WorldState>().IsBleeding = false;

    }

}
