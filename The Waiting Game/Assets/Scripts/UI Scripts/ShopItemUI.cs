using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler
{
    [HideInInspector] public Item item;
    public UI_Shop shop;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
        {
            Debug.LogError("ShopItemUI: Item is NULL on " + gameObject.name);
            return;
        }

        if (shop == null)
        {
            Debug.LogError("ShopItemUI: Shop reference is NULL on " + gameObject.name);
            return;
        }

        Debug.Log("Pointer entered item: " + item.itemName);

        shop.DisplayItemInfo(item);
    }
}
