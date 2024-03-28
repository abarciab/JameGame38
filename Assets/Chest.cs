using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject _prompt;
    [SerializeField] private SpriteRenderer _chestSprite;
    [SerializeField] private Sprite _openChest;
    [SerializeField] private Sound _openSound;
    private Transform _player;
    [SerializeField] private UnityEvent _onOpen;

    [SerializeField] private LootTable lootTable;

    private void Start()
    {
        if (_openChest) _openSound = Instantiate(_openSound);
        _player = GameManager.i.Player.transform;
        _prompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_prompt.activeInHierarchy && collision.transform == _player) _prompt.SetActive(true);
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
        _onOpen.Invoke();
        if (_openChest) _openSound.Play();
        _prompt.SetActive(false);
        if (_chestSprite) _chestSprite.sprite = _openChest;
        enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (lootTable != null) InventoryManager.i.AddItem(lootTable.GetRandomItem());
    }
}
