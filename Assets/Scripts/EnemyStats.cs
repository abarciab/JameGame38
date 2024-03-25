using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : MonoBehaviour, IDamagable, IHealth
{
    public float Health { get; private set; }
    public float MaxHealth;

    [HideInInspector] public UnityEvent<float> OnHealthChange;
    public UnityEvent OnDie;

    private void Start()
    {
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
