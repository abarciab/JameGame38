using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _player) _player.GetComponent<PlayerMovement>().InDangerZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == _player) _player.GetComponent<PlayerMovement>().InDangerZone = false;
    }
}
