using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float _parallaxAmount = 0.05f;
    private Transform _player;
    private Vector3 _oldPos;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
        _oldPos = _player.position;
    }

    private void Update()
    {
        var difference = _player.position - _oldPos;
        var delta = Vector2.Lerp(Vector2.zero, (Vector2) difference, _parallaxAmount);
        transform.position += (Vector3)delta;
        _oldPos = _player.position;
    }

}
