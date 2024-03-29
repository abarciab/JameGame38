using MyBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i;
    private void Awake() { i = this; }

    public List<ItemObject> NearbyItems = new List<ItemObject>();
    public ItemObject HighlightedItem;

    [SerializeField] private List<ItemData> _items = new List<ItemData>();
    [SerializeField] private Sprite emptyImage;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject baseItemPrefab;
    private int maxItems => UIManager.i.MaxItems;

    [SerializeField] private Sound _pickupSound;
    [SerializeField] private Sound _useItemSound;

    private void Start()
    {
        _pickupSound = Instantiate(_pickupSound);
        _useItemSound = Instantiate(_useItemSound);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItemBySlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItemBySlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItemBySlot(2);
    }

    public void SpawnItem(ItemData itemData, Vector3 position)
    {
        GameObject item = Instantiate(baseItemPrefab, position, Quaternion.identity);
        item.GetComponent<ItemObject>().Initialize(itemData);
    }

    public void StartAddItem (ItemData itemData)
    {
        UIManager.i.DisplayNewItemScreen(itemData, _items);
    }

    public void ActuallyAddItem (ItemData itemData)
    {
        if (_items.Count >= maxItems) return;

        _pickupSound.Play();
        _items.Add(itemData);
        UIManager.i.UpdateInventory(_items);
    }

    void UseItemBySlot(int index)
    {
        if (index < 0 || index >= _items.Count) return;

        ItemData item = _items[index];
        item.Use();
        _items.RemoveAt(index);
        //_useItemSound.Play();

        UIManager.i.UpdateInventory(_items);
    }

    public void PlayUseSound() => _useItemSound.Play();
}