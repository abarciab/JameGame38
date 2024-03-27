using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform _shieldParent;
    public bool ShieldUp { get; private set; }
    private PlayerMovement _playerMovement;
    [SerializeField] private float _shieldStunTime = 2f;
    [SerializeField] private float _deflectTime = 0.2f;
    private float _shieldCooldown;
    private float _deflectRemaining;
    [SerializeField] private BounceData _shield;
    [HideInInspector] public bool BlockingDown => ShieldUp && AimingDown();
    [HideInInspector] public bool BlockingRight => ShieldUp && AimingRight();
    [HideInInspector] public bool BlockingLeft => ShieldUp && AimingLeft();

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _shield.OnBounced.AddListener(Deflect);
    }

    private void Update()
    {
        _shieldParent.gameObject.SetActive(ShieldUp);

        if (_shieldCooldown > 0) {
            _shieldCooldown -= Time.deltaTime;
            if (_shieldCooldown <= 0) UIManager.i.HideShieldbar();
            else UIManager.i.DisplayShieldCooldown(1 - _shieldCooldown / _shieldStunTime);
            return;
        }

        if (_playerMovement.Running) ShieldUp = false;
        else if (Input.GetMouseButtonDown(0)) {
            _deflectRemaining = _deflectTime;
            _shield.enabled = true;
            UIManager.i.StartParryColor();
        }
        ShieldUp = Input.GetMouseButton(0);
        
        if (ShieldUp) {
            _deflectRemaining -= Time.deltaTime;
            if (_deflectRemaining <= 0) {
                UIManager.i.StopParryColor();
                _shield.enabled = false;
            }
            TrackMouse();
        }
    }

    private bool AimingDown()
    {
        var down = Quaternion.Euler(0, 0, -90);
        float angle = Quaternion.Angle(_shieldParent.rotation, down);
        return angle < 50;
    }

    private bool AimingRight()
    {
        var down = Quaternion.Euler(0, 0, 0);
        float angle = Quaternion.Angle(_shieldParent.rotation, down);
        return angle < 35;
    }

    private bool AimingLeft()
    {
        var down = Quaternion.Euler(0, 0, 180);
        float angle = Quaternion.Angle(_shieldParent.rotation, down);
        return angle < 35;
    }

    private void TrackMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(Camera.main.transform.position.z)));

        Vector3 direction = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _shieldParent.rotation = targetRotation;
    }

    public void Deflect()
    {
        ShieldUp = false;
        _shieldCooldown = _shieldStunTime;
    }
}
