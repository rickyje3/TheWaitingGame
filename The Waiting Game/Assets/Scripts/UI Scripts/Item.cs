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
        Lighting
    }

    public ItemType itemType;

    /*public static int GetCost(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Hat:          return 20;
            case ItemType.Shirt:        return 25;
            case ItemType.Pants:        return 25;
            case ItemType.Shoes:        return 75;
            case ItemType.Accessories:  return 20;
            case ItemType.Chair:        return 50;
            case ItemType.Shelf:        return 50;
            case ItemType.Table:        return 100;
            case ItemType.Decorations:  return 50;
            case ItemType.Lighting:     return 75;
        }
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.Hat:          return GameAssets.Instance.s_hat; // This should be set to return an image stored in a folder but we don't have images yet lol
            case ItemType.Shirt:        return GameAssets.Instance.s_shirt;
            case ItemType.Pants:        return GameAssets.Instance.s_pants;
            case ItemType.Shoes:        return GameAssets.Instance.s_shoes;
            case ItemType.Accessories:  return GameAssets.Instance.s_accessories;
            case ItemType.Chair:        return GameAssets.Instance.s_chair;
            case ItemType.Shelf:        return GameAssets.Instance.s_shelf;
            case ItemType.Table:        return GameAssets.Instance.s_table;
            case ItemType.Decorations:  return GameAssets.Instance.s_decorations;
            case ItemType.Lighting:     return GameAssets.Instance.s_lighting;
        }
    }*/
}
