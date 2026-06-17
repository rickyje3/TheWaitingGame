using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
    public bool isPurchased = false;

    [TextArea] public string description;

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
        Flooring
    }

    public ItemType itemType;
}
