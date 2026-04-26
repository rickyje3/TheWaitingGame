using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Button ConjureButton;
    public SpriteRenderer Renderer;
    public TextMeshProUGUI QuantityText;
    [HideInInspector] public Item item;
}
