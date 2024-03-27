using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public List<ItemData> AllItems = new List<ItemData>();
    [SerializeField] private ItemData _item;

    [ButtonMethod]
    public void SpawnItem()
    {
        InventoryManager.i.SpawnItem(_item, GameManager.i.Player.transform.position);
    }
}
