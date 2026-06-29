using Unity.AppUI.Core;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cursorIndicator; 
    [SerializeField] private GridInputManager gridInputManager;
    [SerializeField] private Grid floorGrid;
    [SerializeField] private Grid loftGrid;

    [SerializeField] private GameObject gridVisualizationFloor;
    [SerializeField] private GameObject gridVisualizationLoft;
    [SerializeField] private GameAssets gameAssets;

    private Item selectedItem;
    private Grid activeGrid;
    private GameObject activeGridVisualization;


    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        StopPlacement();
    }


    private void PlaceStructure()
    {
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

        Vector3 mousePosition =
            gridInputManager.GetSelectedMousePosition();

        if (mousePosition == Vector3.zero)
        {
            Debug.Log("Invalid mouse position (raycast miss)");
            return;
        }

        if (activeGrid == null)
        {
            Debug.Log("No active grid set");
            return;
        }

        Vector3Int gridPosition =
            floorGrid.WorldToCell(mousePosition);

        Vector3 spawnPosition =
            floorGrid.CellToWorld(gridPosition);

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

        gridVisualizationFloor.SetActive(false);
        gridVisualizationLoft.SetActive(false);
        cursorIndicator.SetActive(false);
        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnExit -= StopPlacement;
    }

    public void StartPlacement(Item item)
    {
        Debug.Log("Started placement of " + item.itemName);
        Debug.Log("Subscribed to " + gridInputManager.GetEntityId());

        selectedItem = item;

        SetActiveGrid(floorGrid, gridVisualizationFloor);

        cursorIndicator.SetActive(true);
        mouseIndicator.SetActive(true);

        gridInputManager.OnClicked += PlaceStructure;
        gridInputManager.OnExit += StopPlacement;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItem == null || activeGrid == null)
            return;

        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();

        if (mousePosition == Vector3.zero)
            return;

        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);
        mouseIndicator.transform.position =
            mousePosition + Vector3.up * 0.5f;

        cursorIndicator.transform.position = activeGrid.CellToWorld(gridPosition);
    }

    private void SetActiveGrid(Grid grid, GameObject visualization)
    {
        activeGrid = grid;

        gridVisualizationFloor.SetActive(false);
        gridVisualizationLoft.SetActive(false);

        activeGridVisualization = visualization;
        activeGridVisualization.SetActive(true);
    }

    // Optional manual switch (you can call this from a button or keybind)
    public void SwitchToLoft()
    {
        SetActiveGrid(loftGrid, gridVisualizationLoft);
    }

    public void SwitchToFloor()
    {
        SetActiveGrid(floorGrid, gridVisualizationFloor);
    }
}
