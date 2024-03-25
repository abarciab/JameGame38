using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private float _lookAheadFactor = 1;
    [SerializeField] private float _lookAheadlerpFactor = 15;
    [SerializeField] private float _mouseFactor;
    [SerializeField] private float _lerpFactor;
    [SerializeField] Vector2 _offset;
    [SerializeField] private Vector2 _walkRunFOV = new Vector2(60, 65);
    [SerializeField] private float _fovLerpFactor = 5;

    private Camera _cam;
    private float _fovTarget;
    private Transform _arenaTarget;
    private float _arenaFOVTarget;
    private Vector2 _lookAheadDelta;

    private void Start()
    {
        _cam = Camera.main;
        _fovTarget = _walkRunFOV.x;
        _playerMovement.OnRunStart.AddListener(() => _fovTarget = _walkRunFOV.y);
        _playerMovement.OnRunEnd.AddListener(() => _fovTarget = _walkRunFOV.x);
    }

    private void Update()
    {
        float target = _arenaTarget == null ? _fovTarget : _arenaFOVTarget;
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, target, _fovLerpFactor * Time.deltaTime);

        _lookAheadDelta = Vector2.Lerp(_lookAheadDelta, _player.GetComponent<Rigidbody2D>().velocity.normalized * _lookAheadFactor, _lookAheadlerpFactor * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        var initialTarget = _arenaTarget == null ? _player.position : _arenaTarget.position;

        var mousePosition = Input.mousePosition;
        var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        var mouseOffset = mousePosition - screenCenter;
        var normalizedPosition = new Vector2(mouseOffset.x / screenCenter.x, mouseOffset.y / screenCenter.y);
        
        var target = initialTarget + (Vector3) (normalizedPosition * _mouseFactor);

        _offset.x = Mathf.Abs(_offset.x) * (_playerMovement.movingRight ? 1 : -1);
        var offset = new Vector3(_offset.x, _offset.y, transform.position.z) + (Vector3) _lookAheadDelta;

        if (_arenaTarget == null) target += offset;
        target.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, target, _lerpFactor * Time.deltaTime);
    }

    public void EnterArenaMode(Transform arenaTarget, float FOV)
    {
        if (_arenaTarget == arenaTarget) return;
        _arenaTarget = arenaTarget;
        _arenaFOVTarget = FOV;
    }

    public void ExitArenaMode(Transform arenaTarget)
    {
        if (_arenaTarget != arenaTarget) return;
        _arenaTarget = null;
    }

}
