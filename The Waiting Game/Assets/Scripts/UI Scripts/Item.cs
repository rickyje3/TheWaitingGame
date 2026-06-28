using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public bool isPurchased = false;

    [TextArea] public string description;

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }


    public enum ItemType
    { 
        Hat,
        Shirt,
        Pants,
        Shoes,
        Accessories,
        Chair,
        Shelf,
        Table,
        Decorations,
        Lighting,
        Beds,
        Pets,
        Wallpaper,
        Flooring,
        Appliances
    }

    public ItemType itemType;
}
