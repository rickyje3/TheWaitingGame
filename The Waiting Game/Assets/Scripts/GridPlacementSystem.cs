using Unity.AppUI.Core;
using UnityEngine;
using static UnityEditor.Progress;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cursorIndicator; // attach the parent here
    [SerializeField] private GridInputManager gridInputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private GameAssets gameAssets;

    public Item selectedItem;


    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        StopPlacement();
    }


    private void PlaceStructure()
    {
        Debug.Log("Calling placestructure");

        if (selectedItem == null)
        {
            Debug.Log("No selected item");
            return;
        }

       /* if (gridInputManager.IsPointerOverUI())
        {
            Debug.Log("Pointer is over ui, blocking placement");
            return;
        }*/

        Debug.Log("Passed UI check");

        Vector3 mousePosition =
            gridInputManager.GetSelectedMousePosition();

        Vector3Int gridPosition =
            grid.WorldToCell(mousePosition);

        Vector3 spawnPosition =
            grid.CellToWorld(gridPosition);

        Debug.Log("Prefab is: " + selectedItem.Prefab);

        Instantiate(
            selectedItem.Prefab,
            spawnPosition,
            Quaternion.identity
        );
        Debug.Log("Placing " + selectedItem.ToString());
    }

    public void StopPlacement()
    {
        Debug.Log("Stopping placement");

        selectedItem = null;

        gridVisualization.SetActive(false);
        cursorIndicator.SetActive(false);
        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnExit -= StopPlacement;
    }

    public void StartPlacement(Item item)
    {
        Debug.Log("Started placement of " + item.itemName);
        Debug.Log("Subscribed to " + gridInputManager.GetEntityId());

        selectedItem = item;

        gridVisualization.SetActive(true);
        cursorIndicator.SetActive(true);

        gridInputManager.OnClicked += PlaceStructure;
        Debug.Log("Subscribed");
        gridInputManager.OnExit += StopPlacement;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItem == null)
            return;

        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position =
            mousePosition + Vector3.up * 0.5f;

        cursorIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
