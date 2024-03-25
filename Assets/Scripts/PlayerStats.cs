using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamagable
{
    [HideInInspector] public UnityEvent<float> OnHealthChange;
    [SerializeField] private Vector2 _knockbackforce;
    [SerializeField] private float _knockbackTime = 0.2f;

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

    public async void Damage(float amount, Transform source)
    {
        Damage(amount);

        bool left = transform.position.x > source.position.x;
        var kbForce = new Vector2(_knockbackforce.x * (left ? -1 : 1), _knockbackforce.y);

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        await Task.Yield();
        GetComponent<Rigidbody2D>().AddForce(kbForce);

        var move = GetComponent<PlayerMovement>();
        move.Stunned = true;

        await Task.Delay(Mathf.RoundToInt(_knockbackTime * 1000));

        move.Stunned = false;
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
