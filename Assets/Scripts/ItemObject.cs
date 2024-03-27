using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;
    private SpriteRenderer highlight;

    // Start is called before the first frame update
    void Start()
    {
        highlight = gameObject.transform.Find("Highlight").GetComponent<SpriteRenderer>();
        highlight.enabled = false;
        if (itemData != null)
        {
            gameObject.name = itemData.itemName;
            gameObject.GetComponent<SpriteRenderer>().sprite = itemData.itemImage;
        }
    }

    public void Highlight(bool state)
    {
        highlight.enabled = state;

        if (state == true)
        {
            InventoryManager.Instance.pickupPrompt.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
        }
        else
        {
            InventoryManager.Instance.pickupPrompt.transform.position = new Vector2(9000, 9000);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            InventoryManager.Instance.NearbyItems.Add(this);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            InventoryManager.Instance.NearbyItems.Remove(this);
            InventoryManager.Instance.highlightedItem = null;
            Highlight(false);
        }
    }
}
