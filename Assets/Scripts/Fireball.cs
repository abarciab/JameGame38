using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fireball : MonoBehaviour
{
    public bool Homing;
    public bool Bouncable;
    public float Damage;
    private Transform _target;
    private Rigidbody2D _rb;
    [SerializeField] private float _homingStopRadius = 3;
    private bool _bounced;
    [SerializeField] private Sound _deflectSound;
    [SerializeField] private ParticleSystem _trail;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_deflectSound) _deflectSound = Instantiate(_deflectSound);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        Homing = true;
    }

    private void Update()
    {
        if (!Homing || !_target) return;
        var dir = (Vector2)(_target.position - transform.position);
        float dist = dir.magnitude;
        if (dist < _homingStopRadius) {
            Homing = false;
            return;
        }

        dir.Normalize();
        var speed = _rb.velocity.magnitude;
        _rb.velocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var damagable = collider.GetComponent<IDamagable>();
        var bounceData = collider.GetComponent<BounceData>();

        if (bounceData && _deflectSound) _deflectSound.Play();

        if (bounceData && Bouncable) {
            FindObjectOfType<CameraShake>().ShakeFixed();
            bounceData.OnBounced.Invoke();

            if (!bounceData.enabled) {
                Kill();
                return;
            }

            var rb = GetComponent<Rigidbody2D>();
            var dir = (collider.transform.position - bounceData.Root.position).normalized;

            float speed = rb.velocity.magnitude;
            rb.velocity = 1.5f * speed * dir;

            _bounced = true;
            Homing = false;
        }
        else if (damagable != null) {
            if (damagable.AcceptDirectHits() && !_bounced || _bounced) damagable.Damage(Damage, transform);
            Kill();
        }
        else {
            Kill();
        }
    }

    private void Kill()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        var light = GetComponentInChildren<Light2D>();
        if (light) light.enabled = false;
        if (_trail) _trail.Stop();

        Invoke(nameof(Destroy), 5);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
