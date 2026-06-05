using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;

    [Header("Grid Settings")]
    public float itemWidth = 160f;
    public float itemHeight = 180f;
    public int itemsPerRow = 3;

    private void Awake()
    {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");
        //shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        for (int i = 0; i < GameAssets.instance.shopItems.Length; i++)
        {
            CreateItemButton(GameAssets.instance.shopItems[i], i);
        }
    }

    private void CreateItemButton(Item item,  /*Sprite itemSprite, string itemName, int itemPrice*/ int positionIndex)
    {
        Transform shopItemTransform =
            Instantiate(shopItemTemplate, container);

        shopItemTransform.gameObject.SetActive(true);

        RectTransform shopItemRectTransform =
            shopItemTransform.GetComponent<RectTransform>();


        // Calculate row and column
        int column = positionIndex % itemsPerRow;
        int row = positionIndex / itemsPerRow;

        // Position item in grid
        shopItemRectTransform.anchoredPosition =
            new Vector2(
                column * itemWidth,
                -row * itemHeight
            );

        // Set item information
        shopItemTransform
            .Find("itemName")
            .GetComponent<TextMeshProUGUI>()
            .SetText(item.itemName);

        shopItemTransform
            .Find("priceText")
            .GetComponent<TextMeshProUGUI>()
            .SetText(item.price.ToString());

        shopItemTransform
            .Find("itemImage")
            .GetComponent<Image>()
            .sprite = item.icon;


        /* shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 120f;

        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().SetText(itemName); // finds the name in the parent object then sets the value
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(itemPrice.ToString());
        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemSprite;*/
    }
}
