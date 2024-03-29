using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearTrap : MonoBehaviour
{
    [SerializeField] private Transform _spears;
    [SerializeField, ReadOnly] private Vector2 _outPos;
    [SerializeField, ReadOnly] private Vector2 _inPos;
    [SerializeField] private AnimationCurve _moveCurve;
    [SerializeField] private float _animateTime;
    [SerializeField] private float _interval;
    [SerializeField] private float _startTime;
    private float _cooldown;

    [SerializeField] private Sound _raise;

    public void Retract()
    {
        StopAllCoroutines();
        _spears.position = _inPos;
    }

    [ButtonMethod]
    private void SetOutPosition()
    {
        _outPos = _spears.localPosition;
    }

    private void Start()
    {
        _raise = Instantiate(_raise);
        _inPos = _spears.localPosition;
        _cooldown = _startTime;
    }

    private void Update()
    {
        _cooldown -= Time.deltaTime;
        if (_cooldown <= 0) StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        _raise.Play(transform);
        _spears.localPosition = _inPos;
        _cooldown = _interval + _animateTime;
        float timePassed = 0;
        while (timePassed < _animateTime) {
            timePassed += Time.deltaTime;
            var progress = timePassed / _animateTime;
            progress = _moveCurve.Evaluate(progress);
            _spears.localPosition = Vector3.Lerp(_inPos, _outPos, progress);
            yield return new WaitForEndOfFrame();
        }
        _spears.localPosition = _inPos;
    }
}
