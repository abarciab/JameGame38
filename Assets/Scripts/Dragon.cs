using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    [Header("Phases")]
    [SerializeField] private int _currentPhase;

    [Header("Attack1 - fireball")]
    [SerializeField] private Transform _lowPos;
    [SerializeField] private string _fireballAnimTriggerString = "Fireball";
    [SerializeField] private int _numFireballs = 3;
    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private float _fireballForce;
    [SerializeField] private Vector2 _firePoint;
    [SerializeField] private float _shootResetTime;
    private float _shootCooldown;
    private int _numFireballsShot;

    [Header("Attack2 - fire rain")]
    [SerializeField] private string _fireRainAnimTriggerString = "Fire rain";
    [SerializeField] private GameObject _fireRainPrefab;
    [SerializeField] private Transform _fireRainCenter;
    [SerializeField] private Transform _topPos;
    [SerializeField] private float _fireRainCount;
    [SerializeField] private float _fireRainStep;

    [Header("Attack3 - fire breath")]
    [SerializeField] private string _fireBreathAnimTriggerString = "Fire breath";
    [SerializeField] private Transform _leftTopPerch;
    [SerializeField] private Transform _rightTopPerch;
    [SerializeField] private float _fireBreathTime = 4;
    [SerializeField] private ParticleSystem _breathParticleSystem;
    [SerializeField] private DragonFireBreathCollider _breathCollider;

    [Header("References")]
    [SerializeField] private Animator _animator;

    private PlayerStats _player;
    private bool _busy;
    private bool _aggro;

    private void Start()
    {
        _player = GameManager.i.Player;
        GetComponent<EnemyStats>().OnHealthChange.AddListener(TakeDamage);
        GetComponent<EnemyStats>().OnDie.AddListener(() => AudioManager.i.GetComponent<MusicPlayer>().playAltMusic = false);
    }

    private void TakeDamage(float value)
    {
        ChangePhase(Random.Range(2, 4));
    }

    public void Activate()
    {
        _aggro = true;
        GameManager.i.FightingDragon = true;
        UIManager.i.DisplayBossBar(GetComponent<EnemyStats>());
        GetComponent<EnemyStats>().OnHealthChange.Invoke(1);
        _shootCooldown = _shootResetTime;
        ChangePhase(1);
        AudioManager.i.GetComponent<MusicPlayer>().playAltMusic = true;
        transform.position = _lowPos.position;
        transform.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if (!_aggro || _busy) return;
        FacePlayer();

        if (_currentPhase == 1) DoPhase1Behavior();
        if (_currentPhase == 2) DoPhase2Behavior();
        if (_currentPhase == 3) DoPhase3Behavior();

    }

    private void FacePlayer()
    {
        if (_player.transform.position.x > transform.position.x) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    private void DoPhase1Behavior()
    {
        _shootCooldown -= Time.deltaTime;
        if (_shootCooldown <= 0) {
            _busy = true;
            _animator.SetTrigger(_fireballAnimTriggerString);
        }
    }

    private async void DoPhase2Behavior()
    {
        _busy = true;
        transform.position = _topPos.position;
        await Task.Delay(500);
        _animator.SetTrigger(_fireRainAnimTriggerString);
    }

    private async void DoPhase3Behavior()
    {
        _busy = true;
        bool left = Random.Range(0, 1f) > 0.5f;
        transform.position = left ? _leftTopPerch.position : _rightTopPerch.position;
        transform.localEulerAngles = new Vector3(0, left ? 0 : 180, 0);
        await Task.Delay(1200);
        _animator.SetTrigger(_fireBreathAnimTriggerString);
    }

    private void ChangePhase(int newPhase)
    {
        _busy = false;
        _currentPhase = newPhase;
        if (_currentPhase == 1) ResetPhase1();
        //if (_currentPhase == 2) ResetPhase2();
        //if (_currentPhase == 3) ResetPhase3();
    }

    public void DoAttack()
    {
        if (_currentPhase == 1) ShootFireball();
        if (_currentPhase == 2) RainFire();
        if (_currentPhase == 3) BreatheFire();
    }

    private async void BreatheFire()
    {
        _breathParticleSystem.Play();
        await Task.Delay(1200);
        _breathCollider.Checking = true;
        await Task.Delay(2200);
        _breathParticleSystem.Stop();
        await Task.Delay(100);
        _breathCollider.Checking = false;
        await Task.Delay(400);
        ChangePhase(Random.Range(1, 3));
    }

    private async void RainFire()
    {
        int originalLayer = gameObject.layer;
        gameObject.layer = 3;

        for (int i = 0; i < _fireRainCount / 2; i++) {
            var pos = _fireRainCenter.position;
            var offset = i * _fireRainStep;
            Instantiate(_fireRainPrefab, pos + Vector3.left * offset, Quaternion.identity);
            if (i > 0) Instantiate(_fireRainPrefab, pos + Vector3.right * offset, Quaternion.identity);
            await Task.Delay(150);
        }

        await Task.Delay(700);
        gameObject.layer = originalLayer;

        ChangePhase(Random.Range(1, 4));
    }

    private async void ShootFireball()
    {
        _shootCooldown = _shootResetTime;
        var pos = transform.TransformPoint((Vector3)_firePoint);

        var fireBallRb = Instantiate(_fireballPrefab, pos, Quaternion.identity).GetComponent<Rigidbody2D>();
        var fireball = fireBallRb.GetComponent<Fireball>();
        fireball.SetTarget(_player.transform);

        var playerPos = _player.transform.position + ((Vector3)_player.GetComponent<Rigidbody2D>().velocity/2);
        var dir = (playerPos - pos).normalized;
        fireBallRb.velocity = dir * _fireballForce;

        _numFireballsShot += 1;
        if (_numFireballsShot >= _numFireballs) {
            await Task.Delay(1000);
            ChangePhase(Random.Range(2, 4));
        }

        _busy = false;
    }

    private void ResetPhase1()
    {
        _numFireballsShot = 0;
        //_shootCooldown = _shootResetTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)_firePoint, 0.5f);
    }
}
