using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler
{
    [HideInInspector] public Item item;
    public UI_Shop shop;

    public void OnPointerEnter(PointerEventData eventData)
    {
        shop.DisplayItemInfo(item);
        Debug.Log("Pointer entered item: " + item.itemName);
    }
}
