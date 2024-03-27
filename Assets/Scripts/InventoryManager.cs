using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Instance { get { return instance; } }

    public GameObject pickupPrompt;
    public List<ItemObject> NearbyItems = new List<ItemObject>();
    public ItemObject highlightedItem;

    public ItemData[] itemTypes;
    public static int maxItems = 3;
    public List<ItemData> curItems = new List<ItemData>(maxItems);
    private List<Image> itemImages = new List<Image>(maxItems);
    [SerializeField] Sprite emptyImage;
    [SerializeField] GameObject slotPrefab, baseItemPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject canvas = GameObject.Find("HUD");

        for (int i = 0; i < maxItems; i++)
        {
            GameObject imageObject = Instantiate(slotPrefab, canvas.transform);
            Image image = imageObject.transform.Find("ItemImage").GetComponent<Image>();
            image.enabled = false;

            Rect rect = canvas.GetComponent<RectTransform>().rect;
            float xPos = -rect.width / 2 + 16;
            float yPos = rect.height / 2 - 16 - i * image.rectTransform.rect.height;
            imageObject.transform.localPosition = new Vector2(xPos, yPos);

            itemImages.Add(image);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        CheckNearbyItems();
    }

    void CheckNearbyItems()
    {
        GameObject player = GameManager.i.Player.gameObject;
        float closestDistance = float.MaxValue;
        ItemObject nearestItem = null;

        foreach (var item in NearbyItems)
        {
            float distance = Vector3.Distance(player.transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestItem = item;
            }
        }

        if (nearestItem != null)
        {
            if (highlightedItem != null && highlightedItem != nearestItem)
            {
                highlightedItem.Highlight(false);
            }
            nearestItem.Highlight(true);
            highlightedItem = nearestItem;
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && highlightedItem != null && curItems.Count < maxItems)
        {
            AddItem(highlightedItem.itemData);
            Destroy(highlightedItem.gameObject);
            NearbyItems.Remove(highlightedItem);
            highlightedItem = null;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnItem(itemTypes[0]);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnItem(itemTypes[1]);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnItem(itemTypes[2]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItemBySlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItemBySlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItemBySlot(2);
        }
    }

    void SpawnItem(ItemData itemData)
    {
        GameObject item = Instantiate(baseItemPrefab, GameManager.i.Player.transform.position, Quaternion.identity);
        item.GetComponent<ItemObject>().itemData = itemData;
    }

    void AddItem(ItemData itemData)
    {
        if (curItems.Count < maxItems)
        {
            curItems.Add(itemData);
        }

        UpdateInventoryUI();
    }

    void UseItemBySlot(int index)
    {
        if (index < 0 || index >= curItems.Count) return;

        ItemData item = curItems[index];
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.HealthPot:
                Debug.Log("Restoring health");
                FindObjectOfType<PlayerStats>().Heal(25);
                break;
            case ItemType.DodgeCrystal:
                Debug.Log("Doing whatever a dodge crystal does");
                break;
            case ItemType.Spear:
                Debug.Log("Throwing a spear");
                break;
            default:
                Debug.Log("Item type not recognized");
                break;
        }

        curItems.RemoveAt(index);

        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < maxItems; i++)
        {
            if (i >= curItems.Count || curItems[i] == null)
            {
                itemImages[i].enabled = false;
            }
            else
            {
                itemImages[i].enabled = true;
                itemImages[i].sprite = curItems[i].itemImage;
            }
        }
    }
}