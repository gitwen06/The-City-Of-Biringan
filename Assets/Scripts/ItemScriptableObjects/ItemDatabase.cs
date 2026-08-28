using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemScriptableObject> items;
    public Dictionary<string, ItemScriptableObject> itemsDictionary;

    public void OnEnable()
    {
        itemsDictionary = new Dictionary<string, ItemScriptableObject>();
        foreach (var item in items)
        {
            itemsDictionary[item.id] = item;
        }
    }
    public ItemScriptableObject GetItemById(string id)
    {
        if (itemsDictionary.TryGetValue(id, out ItemScriptableObject item))
        {
            return item;
        }
        else
        {
            return null;
        }
    }
}
