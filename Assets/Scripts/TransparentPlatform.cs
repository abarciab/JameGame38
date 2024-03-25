using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentPlatform : MonoBehaviour
{
    private Transform _player;
    private Collider2D _collider;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        _collider.enabled = _player.position.y > transform.position.y && !Input.GetKey(KeyCode.S);
    }
}
