using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootTable
{
    [SerializeField] List<LootTableEntry> entries;
    [HideInInspector]
    private float totalWeight = -1;

    void Init()
    {
        totalWeight = 0;
        foreach (LootTableEntry entry in entries)
        {
            totalWeight += entry.weight;
        }
    }

    public ItemData GetRandomItem()
    {
        if (totalWeight == -1)
        {
            Init();
        }

        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;

        foreach (LootTableEntry entry in entries)
        {
            weightSum += entry.weight;
            if (randomValue <= weightSum)
            {
                return entry.item;
            }
        }
        return null;
    }
}

[System.Serializable]
public class LootTableEntry
{
    public ItemData item;
    public float weight;
}