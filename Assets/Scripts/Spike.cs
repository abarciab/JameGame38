using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private bool _resetPlayer;
    [SerializeField] private bool _vertical;
    [SerializeField] private float _damage = 20;
    [SerializeField] private float _damagaTickTime = 0.3f;
    private float _damageCooldown;
    private PlayerCombat _player;
    [SerializeField] private Sound _deflectSound;

    private void Start()
    {
        _player = GameManager.i.Player.GetComponent<PlayerCombat>();
        _deflectSound = Instantiate(_deflectSound);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_damageCooldown > 0) return;
        Check(collision.GetComponent<PlayerStats>());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_damageCooldown > 0) return;
        Check(collision.GetComponent<PlayerStats>());
    }

    private void Check(PlayerStats player)
    {
        if (!player) return;
        if (Mathf.Abs(transform.localEulerAngles.y) < 0.1f && _player.BlockingDown) {
            FloorSpikeBlock();
            return;
        }

        player.Damage(_damage);
        if (_resetPlayer) _player.GetComponent<PlayerMovement>().ResetToSafePosition();
        _damageCooldown = _damagaTickTime;
    }

    private bool PlayerBlockingOnCorrectSide()
    {
        bool playerLeft = _player.transform.position.x < transform.position.x;
        if (playerLeft) return _player.BlockingRight;
        return _player.BlockingLeft;
    }

    private void FloorSpikeBlock()
    {
        var rb = _player.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x * 1.1f, Mathf.Abs(rb.velocity.y) * 1.2f);
        _deflectSound.Play();
        _player.Deflect();
    }

    private void Update()
    {
        _damageCooldown -= Time.deltaTime;
    }
}
