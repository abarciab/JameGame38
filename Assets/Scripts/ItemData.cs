using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { HealthPot, DodgeCrystal, Spear }

[CreateAssetMenu(fileName = "NewItemType", menuName = "ItemType")]
public class ItemData : ScriptableObject
{
    [HideInInspector] public bool Used;
    public string Name;
    public string Description;
    public Sprite Sprite;
    public ItemType Type;
    [ConditionalField(nameof(Type), false, ItemType.HealthPot), SerializeField] private float _healAmount; 

    public void Use()
    {
        if (Used) return;

        if (Type == ItemType.HealthPot) GameManager.i.Player.Heal(_healAmount);
        // if (Type == ItemType.DodgeCrystal) GameManager.i.Player.Heal(_healAmount);
    }
}
