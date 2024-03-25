using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamagable
{
    [HideInInspector] public UnityEvent<float> OnHealthChange;

    public float Health { get; private set;}
    public float MaxHealth;

    private async void Start()
    {
        Health = MaxHealth;
        await Task.Yield();
        CallEvent();
    }

    public void Damage(float amount)
    {
        FindObjectOfType<CameraShake>().ShakeFixed();
        Health = Mathf.Max(0, Health - amount);
        CallEvent();
        if (Health <= 0) Die();
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(1, Health + amount);
        CallEvent();
    }

    private void CallEvent() => OnHealthChange.Invoke(Health / MaxHealth);

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool AcceptDirectHits()
    {
        return true;
    }
}
