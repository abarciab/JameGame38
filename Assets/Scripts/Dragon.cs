using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Dragon : MonoBehaviour
{
    [Header("Phases")]
    [SerializeField] private int _currentPhase;

    [Header("Attack1 - fireball")]
    [SerializeField] private Transform _lowPos;
    [SerializeField] private string _fireballAnimTriggerString = "Fireball";
    [SerializeField] private int _numFireballs = 3;
    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private float _fireballForceMin;
    [SerializeField] private float _fireballForce;
    [SerializeField] private Vector2 _firePoint;
    [SerializeField] private float _shootResetTime;
    private float _shootCooldown;
    private int _numFireballsShot;

    [Header("Attack2 - fire rain")]
    [SerializeField] private string _fireRainAnimTriggerString = "Fire rain";
    [SerializeField] private GameObject _fireRainPrefab;
    [SerializeField] private List<Transform> _fireRainCenters = new List<Transform>();
    [SerializeField] private Transform _topPos;
    [SerializeField] private float _minRainSpeed;
    [SerializeField] private float _fireRainSpeed;
    [SerializeField] private float _fireRainCount;
    [SerializeField] private float _fireRainStep;

    [Header("Attack3 - fire breath")]
    [SerializeField] private string _fireBreathAnimTriggerString = "Fire breath";
    [SerializeField] private string _fireBreathLoopAnimString = "Fire breath loop";
    [SerializeField] private Transform _leftTopPerch;
    [SerializeField] private Transform _rightTopPerch;
    [SerializeField] private float _fireBreathTime = 4;
    [SerializeField] private Light2D _fireBreathLight;
    [SerializeField] private ParticleSystem _breathParticleSystem;
    [SerializeField] private DragonFireBreathCollider _breathCollider;
    [SerializeField] private Transform _firebreathParent;
    private Vector3 _fireBreathStartRot;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _smokeParticles;

    [Header("Sounds")]
    [SerializeField] private Sound _wakeSound;
    [SerializeField] private Sound _hurtSound;
    [SerializeField] private Sound _disapearSound;
    [SerializeField] private Sound _reapearSound;
    [SerializeField] private Sound _flameLoop;
    [SerializeField] private Sound _summonSmall;
    [SerializeField] private Sound _summonBig;

    private PlayerStats _player;
    private bool _busy;
    private bool _aggro;
    private int _originalLayer;
    private int _smokeCount = 250;

    private void Start()
    {
        _fireBreathStartRot = _firebreathParent.transform.localEulerAngles;

        _originalLayer = gameObject.layer;
        _wakeSound = Instantiate(_wakeSound);
        _hurtSound = Instantiate(_hurtSound);
        _disapearSound = Instantiate(_disapearSound);
        _reapearSound = Instantiate(_reapearSound);
        _flameLoop = Instantiate(_flameLoop);
        _summonSmall = Instantiate(_summonSmall);
        _summonBig = Instantiate(_summonBig);

        _player = GameManager.i.Player;
        GetComponent<EnemyStats>().OnHealthChange.AddListener(TakeDamage);
        GetComponent<EnemyStats>().OnDie.AddListener(() => AudioManager.i.GetComponent<MusicPlayer>().playAltMusic = false);
    }

    [ButtonMethod] private void SetTopPos() => _topPos.position = transform.position;
    [ButtonMethod] private void SetLeftPos() => _leftTopPerch.position = transform.position;
    [ButtonMethod] private void SetRightPos() => _rightTopPerch.position = transform.position;
    [ButtonMethod] private void SetLowPos() => _lowPos.position = transform.position;

    private void TakeDamage(float value)
    {
        if (_busy || GetComponent<EnemyStats>().HealthPercent() > 0.99f) return;

        _hurtSound.Play();
        StopAllCoroutines();
        StartCoroutine(WaitThenChangePhase());
    }

    private IEnumerator WaitThenChangePhase()
    {
        _busy = true;
        yield return new WaitForSeconds(PhaseChangeTime());
        _busy = false;
        ChangePhase(Random.Range(2, 4), transform);
    }

    public void Activate()
    {
        _flameLoop.PlaySilent();
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
        _animator.SetTrigger("Wake");
        FindObjectOfType<CameraShake>().Shake(0.2f, 0.1f);
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
            gameObject.layer = _originalLayer;
        }
    }

    private IEnumerator DoPhase2Behavior()
    {
        _busy = true;

        bool moved = SetPosition(_topPos.position);
        if (moved) {
            SetVisible(false);
            yield return new WaitForSeconds(0.8f * GetComponent<EnemyStats>().HealthPercent());
            _smokeParticles.Emit(_smokeCount);
            yield return new WaitForSeconds(0.05f);
            SetVisible(true);
        }

        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger(_fireRainAnimTriggerString);
    }

    private IEnumerator DoPhase3Behavior()
    {
        _busy = true;
        bool left = Random.Range(0, 1f) > 0.5f;

        var pos = left ? _leftTopPerch.position : _rightTopPerch.position;

        bool moved = SetPosition(pos);
        if (moved) {
            SetVisible(false);
            yield return new WaitForSeconds(0.8f * GetComponent<EnemyStats>().HealthPercent());
            _smokeParticles.Emit(_smokeCount);
            yield return new WaitForSeconds(0.05f);
            SetVisible(true);
        }

        transform.localEulerAngles = new Vector3(0, left ? 0 : 180, 0);
        yield return new WaitForSeconds(1.2f);
        _animator.SetTrigger(_fireBreathAnimTriggerString);
        _animator.SetBool(_fireBreathLoopAnimString, true);
    }

    private void SetVisible(bool state)
    {
        GetComponent<Collider2D>().enabled = state;
        _animator.GetComponent<SpriteRenderer>().enabled = state;
        GetComponentInChildren<Light2D>().enabled = state;
        if (state) _reapearSound.Play();
        else _disapearSound.Play();
    }

    private void ChangePhase(int newPhase, bool damaged = false)
    {
        SetVisible(true);
        StopAllCoroutines();
        _busy = false;

        if (_currentPhase != 1 && !damaged) newPhase = CheckExtraFireball(newPhase);

        _currentPhase = newPhase;
        if (_currentPhase == 1) ResetPhase1();
        //if (_currentPhase == 2) ResetPhase2();
        //if (_currentPhase == 3) ResetPhase3();
    }

    private int CheckExtraFireball(int current)
    {
        var percent = GetComponent<EnemyStats>().HealthPercent();
        if (percent > .75f) return Random.Range(0, 1f) > 0.2f ? 1: current;
        if (percent > .5f) return Random.Range(0, 1f) > 0.5f ? 1 : current;
        if (percent > .25f) return Random.Range(0, 1f) > 0.9f ? 1 : current;
        return current;
    }

    public void DoAttack()
    {
        if (_currentPhase == 1) StartCoroutine(ShootFireball());
        if (_currentPhase == 2) StartCoroutine(RainFire());
        if (_currentPhase == 3) StartCoroutine(BreatheFire());
    }

    private bool SetPosition(Vector3 Pos)
    {
        var dist = Vector3.Distance(transform.position, Pos);
        if (dist <= 0.1f) return false;

        _smokeParticles.Emit(_smokeCount);
        transform.position = Pos;
        FindObjectOfType<CameraShake>().Shake(0.1f, 0.1f);
        return true;
    }

    private IEnumerator BreatheFire()
    {
        float variance = 8;
        _firebreathParent.transform.localEulerAngles = _fireBreathStartRot + Vector3.forward * Random.Range(-variance, variance);

        var shake = FindObjectOfType<CameraShake>();

        _fireBreathLight.enabled = true;
        _breathParticleSystem.Play();
        shake.Shake(0.1f, 0.2f);
        StartCoroutine(LerpFireSound(0.5f, 1, 0));
        yield return new WaitForSeconds(1.2f);
        shake.Shake(0.1f, 0.2f);
        _breathCollider.Checking = true;
        yield return new WaitForSeconds(1.5f * GetComponent<EnemyStats>().HealthPercent() + 0.2f);
        shake.Shake(0.1f, 0.2f);
        _flameLoop.PercentVolume(0);
        _breathParticleSystem.Stop();
        _fireBreathLight.enabled = false;
        yield return new WaitForSeconds(0.1f);
        _breathCollider.Checking = false;
        _animator.SetBool(_fireBreathLoopAnimString, false);

        yield return new WaitForSeconds(PhaseChangeTime());
        ChangePhase(Random.Range(1, 3));
    }

    private IEnumerator LerpFireSound(float time, float targetVol, float startVol)
    {
        float timePassed = 0;
        while (timePassed < time) {
            timePassed += Time.deltaTime;
            float progress = timePassed / time;
            _flameLoop.PercentVolume(Mathf.Lerp(startVol, targetVol, progress));
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RainFire()
    {
        _summonBig.Play();
        gameObject.layer = 3;

        var fireDir = GetFireDir();
        List<Vector3> dirs = GetOffsetDir( (int) fireDir.x);

        int count = Mathf.RoundToInt(_fireRainCount * (1 - GetComponent<EnemyStats>().HealthPercent()));
        count = Mathf.Max(2, count);

        float val = 3;
        Vector2 randomOffset = new Vector2(Random.Range(-val, val), Random.Range(-val, val));

        for (int i = 0; i < count / 2; i++) {
            var pos = _fireRainCenters[(int)fireDir.x].position + (Vector3) randomOffset;
            var offset = i * _fireRainStep;

            var fireBall = Instantiate(_fireRainPrefab, pos + dirs[0] * offset, Quaternion.identity);
            Configure(fireBall.transform, fireDir);
            if (i > 0) fireBall = Instantiate(_fireRainPrefab, pos + dirs[1] * offset, Quaternion.identity);
            Configure(fireBall.transform, fireDir);

            yield return new WaitForSeconds(.25f);
        }

        yield return new WaitForSeconds(PhaseChangeTime());
        ChangePhase(Random.Range(1, 4));
    }

    private void Configure(Transform fireBall, Vector4 fireDir)
    {
        fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(fireDir.y, fireDir.z) * GetFireRainSpeed();
        fireBall.localEulerAngles = new Vector3(0, 0, fireDir.w);
    }

    private float GetFireRainSpeed()
    {
        var percent = GetComponent<EnemyStats>().HealthPercent();
        return Mathf.Lerp(_minRainSpeed, _fireRainSpeed, 1 - percent);
    }

    private List<Vector3> GetOffsetDir(int dir)
    {
        if (dir == 0 || dir == 2) return new List<Vector3>() { Vector3.left, Vector3.right };
        else return new List<Vector3>() { Vector3.up, Vector3.down };
    }

    private Vector4 GetFireDir()
    {
        int dir = GetFireDirIndexByDifficulty();
        if (dir == 0) return new Vector4(0, 0, -1, 0);
        if (dir == 1) return new Vector4(1, -1, 0, 270);
        if (dir == 2) return new Vector4(2, 0, 1, 180);
        else return new Vector4(3, 1, 0, 90);
    }

    private float PhaseChangeTime()
    {
        var percent = GetComponent<EnemyStats>().HealthPercent();
        return percent;
    }

    private int GetFireDirIndexByDifficulty()
    {
        var percent = GetComponent<EnemyStats>().HealthPercent();
        if (percent > .75f) return 0;
        if (percent > .5f) return Random.Range(0, 2);
        if (percent > .25f) return Random.Range(0, 3);
        return Random.Range(0, 4);
    }

    private IEnumerator ShootFireball()
    {
        _summonSmall.Play();
        _shootCooldown = _shootResetTime;
        var pos = transform.TransformPoint((Vector3)_firePoint);

        var fireBallRb = Instantiate(_fireballPrefab, pos, Quaternion.identity).GetComponent<Rigidbody2D>();
        var fireball = fireBallRb.GetComponent<Fireball>();
        fireball.SetTarget(_player.transform);

        var playerPos = _player.transform.position + ((Vector3)_player.GetComponent<Rigidbody2D>().velocity/2);
        var dir = (playerPos - pos).normalized;

        var speed = Mathf.Lerp(_fireballForceMin, _fireballForce, 1 - GetComponent<EnemyStats>().HealthPercent());
        fireBallRb.velocity = dir * speed;

        _numFireballsShot += 1;
        if (_numFireballsShot >= _numFireballs) {
            yield return new WaitForSeconds(PhaseChangeTime());
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
