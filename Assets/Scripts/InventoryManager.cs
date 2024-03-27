using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;
    public static InventoryManager Instance { get { return instance; } }

    public ItemData[] itemTypes;
    public static int maxItems = 3;
    public List<ItemData> curItems = new List<ItemData>(maxItems);
    private List<Image> itemImages = new List<Image>(maxItems);
    [SerializeField] Sprite emptyImage;
    [SerializeField] GameObject slotPrefab;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItem(itemTypes[0]);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddItem(itemTypes[1]);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem(itemTypes[2]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseItem(2);
        }
    }

    void AddItem(ItemData itemData)
    {
        if (curItems.Count < maxItems)
        {
            curItems.Add(itemData);
        }

        UpdateInventoryUI();
    }

    void UseItem(int index)
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
            Debug.Log(i);
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