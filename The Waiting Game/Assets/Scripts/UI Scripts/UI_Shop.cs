using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Shop : MonoBehaviour
{
    [SerializeField] private Transform container;
    public Transform shopItemTemplate;

    public MoneyManager moneyManager;
    [HideInInspector] public Button buyButton;

    [Header("Grid Settings")]
    public float itemWidth = 120f;
    public float itemHeight = 120f;
    public int itemsPerRow = 2;

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image itemImage;

    public ShopItemUI shopItemUI;

    private void Awake()
    {
        //container = transform.Find("Container");
        if(shopItemTemplate == null) shopItemTemplate = container.Find("ShopItemTemplate");

        if(moneyManager == null)
        moneyManager = FindAnyObjectByType<MoneyManager>();
    }

    private void Start()
    {
        for (int i = 0; i < GameAssets.instance.shopItems.Length; i++)
        {
            CreateItemButton(GameAssets.instance.shopItems[i], i);
        }

        Debug.Log("Container is: " + container?.name);
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
            moneyManager.UpdateMoneyText();
        }
        else if (!item.isPurchased && moneyManager.money < item.price)
        {
            Debug.Log("Insufficient funds");
        }
        else if (item.isPurchased)
        {
            Debug.Log("You already own this item");
        }
    }


    public void DisplayItemInfo(Item item)
    {
        itemNameText.text = item.itemName;
        priceText.text = "$" + item.price;
        descriptionText.text = item.description;
        itemImage.sprite = item.icon;
    }

    private void CreateItemButton(Item item,  /*Sprite itemSprite, string itemName, int itemPrice*/ int positionIndex)
    {
        Transform shopItemTransform =
                Instantiate(shopItemTemplate, container);

        shopItemTransform.gameObject.SetActive(true);

        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        // Calculate row and column
        int column = positionIndex % itemsPerRow;
        int row = positionIndex / itemsPerRow;

        // Position item in grid
        shopItemRectTransform.anchoredPosition =
          new Vector2(
                column * itemWidth,
                -row * itemHeight
          );

        ShopItemUI shopItemUI =
            shopItemTransform.GetComponentInChildren<ShopItemUI>();

        shopItemUI.item = item;
        shopItemUI.shop = this;

        shopItemTransform
            .Find("itemImage")
            .GetComponent<Image>()
            .sprite = item.icon;

        Button buyButton =
            shopItemTransform
                .Find("BuyButton")
                .GetComponent<Button>();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => Purchase(item));
    

        /*shopItemTransform = Instantiate(shopItemTemplate, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 120f;

        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().SetText(itemName); // finds the name in the parent object then sets the value
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(itemPrice.ToString());
        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemSprite;*/
    }
}
