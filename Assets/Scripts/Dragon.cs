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

    [Header("Sounds")]
    [SerializeField] private Sound _wakeSound;
    [SerializeField] private Sound _hurtSound;

    private PlayerStats _player;
    private bool _busy;
    private bool _aggro;

    private void Start()
    {
        _wakeSound = Instantiate(_wakeSound);
        _hurtSound = Instantiate(_hurtSound);

        _player = GameManager.i.Player;
        GetComponent<EnemyStats>().OnHealthChange.AddListener(TakeDamage);
        GetComponent<EnemyStats>().OnDie.AddListener(() => AudioManager.i.GetComponent<MusicPlayer>().playAltMusic = false);
    }

    private void TakeDamage(float value)
    {
        if (GetComponent<EnemyStats>().HealthPercent() > 0.99f) return;

        _hurtSound.Play();
        StopAllCoroutines();
        ChangePhase(Random.Range(2, 4));
    }

    public void Activate()
    {
        print("Awake");
        _wakeSound.Play();
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
        if (_currentPhase == 2) StartCoroutine(DoPhase2Behavior());
        if (_currentPhase == 3) StartCoroutine(DoPhase3Behavior());

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

    private IEnumerator DoPhase2Behavior()
    {
        _busy = true;
        transform.position = _topPos.position;
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger(_fireRainAnimTriggerString);
    }

    private IEnumerator DoPhase3Behavior()
    {
        _busy = true;
        bool left = Random.Range(0, 1f) > 0.5f;
        transform.position = left ? _leftTopPerch.position : _rightTopPerch.position;
        transform.localEulerAngles = new Vector3(0, left ? 0 : 180, 0);
        yield return new WaitForSeconds(1.2f);
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
        if (_currentPhase == 1) StartCoroutine(ShootFireball());
        if (_currentPhase == 2) StartCoroutine(RainFire());
        if (_currentPhase == 3) StartCoroutine(BreatheFire());
    }

    private IEnumerator BreatheFire()
    {
        _breathParticleSystem.Play();
        yield return new WaitForSeconds(1.2f);
        _breathCollider.Checking = true;
        yield return new WaitForSeconds(2.2f);
        _breathParticleSystem.Stop();
        yield return new WaitForSeconds(0.1f);
        _breathCollider.Checking = false;
        yield return new WaitForSeconds(0.4f);
        ChangePhase(Random.Range(1, 3));
    }

    private IEnumerator RainFire()
    {
        int originalLayer = gameObject.layer;
        gameObject.layer = 3;

        for (int i = 0; i < _fireRainCount / 2; i++) {
            var pos = _fireRainCenter.position;
            var offset = i * _fireRainStep;
            Instantiate(_fireRainPrefab, pos + Vector3.left * offset, Quaternion.identity);
            if (i > 0) Instantiate(_fireRainPrefab, pos + Vector3.right * offset, Quaternion.identity);
            yield return new WaitForSeconds(.15f);
        }

        yield return new WaitForSeconds(0.7f);
        gameObject.layer = originalLayer;

        ChangePhase(Random.Range(1, 4));
    }

    private IEnumerator ShootFireball()
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
            yield return new WaitForSeconds(1f);
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
