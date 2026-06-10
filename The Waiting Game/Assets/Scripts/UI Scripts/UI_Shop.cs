using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;

    [Header("Grid Settings")] // the way the items are displayed in the shop
    public float itemWidth = 100f;
    public float itemHeight = 100f;
    public int itemsPerRow = 3;

    public MoneyManager moneyManager;
    [HideInInspector] public Button buyButton;

    private void Awake()
    {
        container = transform.Find("Container");
        shopItemTemplate = container.Find("ShopItemTemplate");

        if(moneyManager == null)
        moneyManager = FindAnyObjectByType<MoneyManager>();
    }

    private void Start()
    {
        for (int i = 0; i < GameAssets.instance.shopItems.Length; i++)
        {
            CreateItemButton(GameAssets.instance.shopItems[i], i);
        }
    }

    public void Purchase(Item item)
    {
        item.isPurchased = PlayerPrefs.GetInt(item.itemName, 0) == 1; // Load purchase state 

        if (!item.isPurchased && moneyManager.money >= item.price)
        {
            moneyManager.money -= item.price;
            item.isPurchased = true;
            PlayerPrefs.SetInt(item.itemName, 1); // Save purchase state
            Debug.Log(item.itemName + " was purchased for $" + item.price);
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
            .SetText("$" + item.price.ToString());

        shopItemTransform
            .Find("itemImage")
            .GetComponent<Image>()
            .sprite = item.icon;

        // Grab the Buy button from this specific shop item
        buyButton = shopItemTransform
            .Find("BuyButton")
            .GetComponentInChildren<Button>();

        // Remove old listeners just in case
        buyButton.onClick.RemoveAllListeners();

        // Pass this specific Item SO when clicked
        buyButton.onClick.AddListener(() => Purchase(item));

        /* shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 120f;

        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().SetText(itemName); // finds the name in the parent object then sets the value
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(itemPrice.ToString());
        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemSprite;*/
    }
}
