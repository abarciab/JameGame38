using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    [SerializeField] private Transform _axePivot;
    [SerializeField] private float _maxSpeed = 150;
    [SerializeField] private bool _goingRight;
    [SerializeField] private float _startAngle = 0;
    [SerializeField] private Vector2 _minMaxZ = new Vector2(-75, 75);
    [SerializeField] private AnimationCurve _speedCurve;
    private float _currentValue;

    private void Start()
    {
        _axePivot.transform.localEulerAngles = Vector3.forward * _startAngle;
        _currentValue = _startAngle;
    }

    private void Update()
    {
        var progress = (_currentValue - _minMaxZ.x) / (_minMaxZ.y - _minMaxZ.x);
        if (!_goingRight) progress = 1 - progress;
        float speed = _speedCurve.Evaluate(progress) * _maxSpeed;

        var delta = speed * Time.deltaTime * (_goingRight ? 1 : -1);
        _currentValue += delta;

        _axePivot.transform.localEulerAngles += Vector3.forward * delta; 
        if (_currentValue < _minMaxZ.x || _currentValue > _minMaxZ.y) _goingRight = !_goingRight;
    }
}
