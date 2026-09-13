using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item")]
public class Item : ScriptableObject
{
    [SerializeField]
    private string itemID;
    public string ItemID => itemID; // name to give to item that won't change so it won't break save data if the name changes

    public string itemName;
    public Sprite icon;
    public int price;
    public bool isPurchased = false;

    public bool isBlocker; // mark as true if you want this to just block a tile and not make it so objects are placeable

    [TextArea] public string description;

    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    public bool isCosmetic;
    public bool isFloorObject;
    public bool isWallObject;


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
        Appliances,
        Miscellaneous
    }

    public ItemType itemType;
}
