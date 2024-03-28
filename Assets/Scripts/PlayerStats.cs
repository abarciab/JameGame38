using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour, IDamagable
{
    [HideInInspector] public UnityEvent<float> OnHealthChange;
    [SerializeField] private Vector2 _knockbackforce;
    [SerializeField] private float _knockbackTime = 0.2f;
    [SerializeField] private Sound _hurtSound;
    [SerializeField] private Animator _animator;
    public float Health { get; private set; }
    public float MaxHealth;

    private void Start()
    {
        _hurtSound = Instantiate(_hurtSound);
        Health = MaxHealth;
        Invoke(nameof(CallEvent), 0.01f);
    }

    public void Damage(float amount)
    {
        FindObjectOfType<CameraShake>().ShakeFixed();
        Health = Mathf.Max(0, Health - amount);
        CallEvent();
        if (Health <= 0) Die();
        else _hurtSound.Play();
    }

    public void Damage(float amount, Transform source)
    {
        Damage(amount);
        bool left = transform.position.x > source.position.x;
        var kbForce = new Vector2(_knockbackforce.x * (left ? 1 : -1), _knockbackforce.y);
        StartCoroutine(DamageCoroutine(kbForce));
        _animator.SetTrigger("hurt");
    }

    private IEnumerator DamageCoroutine(Vector2 kbForce)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForEndOfFrame();
        GetComponent<Rigidbody2D>().velocity = kbForce;
        print("added force: " + kbForce);

        var move = GetComponent<PlayerMovement>();
        move.Stunned = true;

        yield return new WaitForSeconds(_knockbackTime);

        move.Stunned = false;
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
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
