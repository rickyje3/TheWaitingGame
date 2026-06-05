using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;

    private void Awake()
    {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");
        //shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(Item.GetSprite(Item.ItemType.Hat), "Sprite_hat", Item.GetCost(Item.ItemType.Hat), 0);
        CreateItemButton(Item.GetSprite(Item.ItemType.Shirt), "Sprite_shirt", Item.GetCost(Item.ItemType.Shirt), 1);
    }

    private void CreateItemButton(Sprite itemSprite, string itemName, int itemPrice, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 120f;

        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().SetText(itemName); // finds the name in the parent object then sets the value
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(itemPrice.ToString());
        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemSprite;
    }
}
