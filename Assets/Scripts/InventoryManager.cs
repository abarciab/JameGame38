using System.Collections;
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
    public List<Image> itemImages = new List<Image>(maxItems);

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
            GameObject imageObject = new GameObject("Image" + i);
            imageObject.transform.SetParent(canvas.transform);

            RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.sizeDelta = new Vector2(64, 64);
            rectTransform.anchoredPosition = new Vector2(64, -i * rectTransform.rect.height - 64);

            Debug.Log(rectTransform.anchoredPosition);

            Image image = imageObject.AddComponent<Image>();

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

        if (Input.GetKeyDown(KeyCode.U))
        {
            UseItem(0);
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
                itemImages[i].sprite = null;
            }
            else
            {
                itemImages[i].sprite = curItems[i].itemImage;
            }
        }
    }
}