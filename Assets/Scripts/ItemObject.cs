using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;

    // Start is called before the first frame update
    void Start()
    {
        if (itemData != null)
        {
            gameObject.name = itemData.itemName;
            gameObject.GetComponent<SpriteRenderer>().sprite = itemData.itemImage;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
