using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    public List<Item> shopItems;


    private void Awake()
    {
        instance = this;
    }


    public Item GetItemByID(string itemID)
    {
        foreach (Item item in shopItems)
        {
            if (item != null && item.ItemID == itemID)
            {
                return item;
            }
        }

        Debug.LogWarning(
            $"Could not find Item with ID '{itemID}'.");

        return null;
    }
}