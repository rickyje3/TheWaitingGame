using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public Item[] items;


    private void Awake()
    {
        Instance = this;

        items = Utils.GetAllInstances<Item>();

        foreach (Item item in items)
        {
            Debug.Log(item.name);
        }
    }
}
