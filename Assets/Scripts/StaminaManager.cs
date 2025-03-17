using System.Collections;
using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [SerializeField] int _maxStamina;
    [SerializeField] float _waitForRegen;
    [SerializeField] GameObject _staminaBarObj;

    SpriteRenderer _staminaBar;

    public float BaseStaminaRegenRate;
    private float _currentStaminaRegenRate;
    [HideInInspector]
    public float CurrentStamina;
    [HideInInspector]
    public bool IsAttacking;
    [HideInInspector]
    public bool IsBlocking;

    private bool _canRegenStamina = true;

    private Coroutine _counter;

    private void Start()
    {
        if (_staminaBarObj)
            _staminaBar = _staminaBarObj.GetComponent<SpriteRenderer>();
        else
            _staminaBar = GameObject.Find("Stamina").GetComponent<SpriteRenderer>();
        CurrentStamina = _maxStamina;
        _currentStaminaRegenRate = BaseStaminaRegenRate;
    }
    private void Update()
    {
        if (_staminaBar.enabled == false) return;
        SetStaminaBar();
        RegenStamina();
    }

    public void DepleteStamina(int stamina)
    {
        _canRegenStamina = false;
        CurrentStamina -= stamina;

        if (_counter != null) StopCoroutine(_counter);
        _counter = StartCoroutine(DoCooldown());
    }

    private void SetStaminaBar()
    {
        Vector2 staminaBarSize = new Vector2(CurrentStamina / _maxStamina, 1);
        _staminaBar.size = staminaBarSize;
        _staminaBar.gameObject.transform.localPosition = new Vector3(0 - ((1f - staminaBarSize.x) / 2f), 0, 0);
    }

    private void RegenStamina()
    {
        if (IsAttacking || IsBlocking) return;
        if (!_canRegenStamina || CurrentStamina >= _maxStamina) return;
        CurrentStamina = CurrentStamina + _currentStaminaRegenRate * Time.deltaTime;
    }

    private IEnumerator DoCooldown()
    {
        yield return new WaitForSeconds(_waitForRegen);
        _canRegenStamina = true;
    }

    public float GetStamina()
    {
        return CurrentStamina / _maxStamina; 
    }

}
