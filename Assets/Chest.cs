using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject _prompt;
    [SerializeField] private SpriteRenderer _chestSprite;
    [SerializeField] private Sprite _openChest;
    [SerializeField] private Sound _openSound;
    private Transform _player;

    private void Start()
    {
        _openSound = Instantiate(_openSound);
        _player = GameManager.i.Player.transform;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform == _player) _prompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerStats>()) _prompt.SetActive(false);
    }

    private void Update()
    {
        if (!_prompt.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.E)) Open();
    }

    private void Open()
    {
        _openSound.Play();
        _prompt.SetActive(false);
        _chestSprite.sprite = _openChest;
        enabled = false;
        GetComponent<Collider2D>().enabled = false;   
    }
}
