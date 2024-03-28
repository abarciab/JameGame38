using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private GameObject _prompt;
    [SerializeField] private SpriteRenderer _sprite;

    private ItemData _itemData;
    private Transform _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _player) _prompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == _player) _prompt.SetActive(false);
    }

    private void Start()
    {
        _player = GameManager.i.Player.transform;
    }

    public void Initialize(ItemData item)
    {
        _itemData = item;
        gameObject.name = _itemData.Name;
        _sprite.sprite = _itemData.Sprite;
    }

    private void Update()
    {
        if (_prompt.activeInHierarchy && Input.GetKeyDown(KeyCode.E)) {
            InventoryManager.i.AddItem(_itemData);
            Destroy(gameObject);
        }
    }
}
