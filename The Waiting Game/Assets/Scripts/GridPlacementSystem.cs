using System;
using System.Collections.Generic;
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

    public Item selectedItem;
    private Grid activeGrid;
    private GameObject activeGridVisualization;

    private GridData floorData, furnitureData;

    private Renderer previewRenderer;

    private List<GameObject> placedGameObjects = new();


    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        StopPlacement();
        floorData = new();
        furnitureData = new();
        previewRenderer = cursorIndicator.GetComponentInChildren<Renderer>();
    }


    private void PlaceStructure()
    {
        if (selectedItem == null)
        {
            Debug.Log("No selected item");
            return;
        }

        /*if (gridInputManager.IsPointerOverUI())
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
            activeGrid.WorldToCell(mousePosition);

        Vector3 spawnPosition =
            activeGrid.CellToWorld(gridPosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);

        if (placementValidity == false)
            return;

        Debug.Log("Prefab is: " + selectedItem.Prefab);

        GameObject obj = Instantiate(
            selectedItem.Prefab,
            spawnPosition,
            Quaternion.identity
        );

        placedGameObjects.Add(obj);

        GridData selectedData = selectedItem.isFloorObject ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            selectedItem.Size,
            selectedItem,
            placedGameObjects.Count - 1);

        Debug.Log($"ADDING {selectedData.GetHashCode()}");
        Debug.Log("Placing " + selectedItem.ToString());
    }

    private GridData GetSelectedData()
    {
        return selectedItem.isFloorObject
            ? floorData
            : furnitureData;
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Item selectedItem)
    {
        //GridData selectedData = selectedItem.isFloorObject ? floorData : furnitureData;
        //Debug.Log($"CHECKING {selectedData.GetHashCode()}");
        return GetSelectedData().CanPlaceObjectAt(
            gridPosition,
            selectedItem.Size);
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

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);
        previewRenderer.material.color = placementValidity ? Color.green : Color.red;

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
