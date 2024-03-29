using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { HealthPot, MaxHealthPot, FullHealPot }

[CreateAssetMenu(fileName = "NewItemType", menuName = "ItemType")]
public class ItemData : ScriptableObject
{
    [HideInInspector] public bool Used;
    public string Name;
    [TextArea (3, 10)]public string Description;
    public string Quote;
    public Sprite Sprite;
    public ItemType Type;
    [ConditionalField(nameof(Type), false, ItemType.HealthPot, ItemType.MaxHealthPot), SerializeField] private float _healAmount; 

    public void Use()
    {
        var player = GameManager.i.Player;
        if (Type == ItemType.HealthPot) player.Heal(_healAmount);
        if (Type == ItemType.MaxHealthPot) player.IncreaseMaxHealth(_healAmount);
        if (Type == ItemType.FullHealPot) player.Heal(player.MaxHealth);
        InventoryManager.i.PlayUseSound();
    }
}
