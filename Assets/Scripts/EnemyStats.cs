using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour, IDamagable, IHealth
{
    public float Health { get; private set; }
    public float MaxHealth;
    [SerializeField] private bool _endGameOnDeath;

    [HideInInspector] public UnityEvent<float> OnHealthChange;
    public UnityEvent OnDie;
    [SerializeField] private Sound _deathSound;

    private void Start()
    {
        _deathSound = Instantiate(_deathSound);
        Health = MaxHealth;
        CallEvent();
    }

    public void Damage(float amount)
    {
        Health = Mathf.Max(0, Health - amount);
        CallEvent();
        if (Health <= 0) Die();
    }

    private void Die()
    {
        OnDie.Invoke();
        UIManager.i.HideBossBar();
        Destroy(gameObject);
        if (_deathSound) _deathSound.Play();
        if (_endGameOnDeath) GameManager.i.EndGame(4);
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(1, Health + amount);
        CallEvent();
    }

    private void CallEvent() => OnHealthChange.Invoke(HealthPercent());

    public float HealthPercent()
    {
        return Health / MaxHealth;
    }

    public UnityEvent<float> GetOnHealthChange()
    {
        return OnHealthChange;
    }

    public bool AcceptDirectHits()
    {
        return false;
    }

    public void Damage(float amount, Transform source)
    {
        Damage(amount);
    }
}
