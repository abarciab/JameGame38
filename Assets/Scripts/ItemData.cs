using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    HealthPot,
    DodgeCrystal,
    Spear
}

[CreateAssetMenu(fileName = "NewItemType", menuName = "ItemType")]
public class ItemData : ScriptableObject
{
    public string itemName, itemDescription;
    public Sprite itemImage;
    public ItemType itemType;
}
