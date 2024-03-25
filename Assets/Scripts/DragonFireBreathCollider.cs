using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireBreathCollider : MonoBehaviour
{
    [SerializeField] private float _damage;
    public bool Checking;
    [SerializeField] private float _damagaTickTime = 0.3f;
    private float _damageCooldown;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Checking || _damageCooldown > 0) return;
        var player = collision.GetComponent<PlayerStats>();
        if (player) {
            player.Damage(_damage);
            _damageCooldown = _damagaTickTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Checking || _damageCooldown > 0) return;
        var player = collision.GetComponent<PlayerStats>();
        if (player) {
            player.Damage(_damage);
            _damageCooldown = _damagaTickTime;
        }
    }

    private void Update()
    {
        _damageCooldown -= Time.deltaTime;
    }
}
