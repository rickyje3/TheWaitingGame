using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator; 
    [SerializeField] private GridInputManager gridInputManager;
    public Grid floorGrid;
    public Grid loftGrid;

    public GameObject gridVisualizationFloor;
    public GameObject gridVisualizationLoft;
    [SerializeField] private GameAssets gameAssets;

    [HideInInspector] public Item selectedItem;
    [HideInInspector] public Grid activeGrid;
    private GameObject activeGridVisualization;

    private GridData floorData, furnitureData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private SoundFeedback soundFeedback;

    IBuildingState buildingState;


    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        floorData = new();
        furnitureData = new();
        StopPlacement();
    }


    private void PlaceStructure()
    {
        Debug.Log("Placing structure");

        if (buildingState == null)
            return;

        // Only placement requires a selected item
        if (buildingState is PlacementState && selectedItem == null)
        {
            Debug.Log("No selected item");
            return;
        }

        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();

        if (mousePosition == Vector3.zero)
            return;

        if (activeGrid == null)
            return;

        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);


        //Moved to PlacementState class
        /*
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);

        if (placementValidity == false)
            return;

        Debug.Log("Prefab is: " + selectedItem.Prefab);


        int index = objectPlacer.PlaceObject(selectedItem.Prefab, activeGrid.CellToWorld(gridPosition));

        GridData selectedData = selectedItem.isFloorObject ? floorData : furnitureData;
        selectedData.AddObjectAt(
            gridPosition,
            selectedItem.Size,
            selectedItem,
            objectPlacer.placedGameObjects.Count - 1);

        Debug.Log($"ADDING {selectedData.GetHashCode()}");
        Debug.Log("Placing " + selectedItem.ToString());

        preview.UpdatePosition(activeGrid.CellToWorld(gridPosition), false);*/
    }

    public GridData GetSelectedData()
    {
        if (selectedItem == null)
            return null;

        return selectedItem.isFloorObject
            ? floorData
            : furnitureData;
    }

    public void StopPlacement()
    {
        Debug.Log("Stopping placement");

        selectedItem = null;

        gridVisualizationFloor.SetActive(false);
        gridVisualizationLoft.SetActive(false);

        if(buildingState != null)
            buildingState.EndState();

        //preview.StopShowingPreview();
        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    public void StartPlacement(Item item)
    {
        Debug.Log("Started placement of " + item.itemName);
        Debug.Log("Subscribed to " + gridInputManager.GetEntityId());

        selectedItem = item;

        buildingState = new PlacementState(selectedItem, activeGrid, preview, this, floorData, furnitureData, objectPlacer, soundFeedback);

        mouseIndicator.SetActive(true);

        gridInputManager.OnClicked += PlaceStructure;
        gridInputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        activeGridVisualization.SetActive(true);
        buildingState = new RemovingState(this, preview, activeGrid, floorData, furnitureData, objectPlacer, soundFeedback);

        Debug.Log("Starting Remove Mode");
        Debug.Log(buildingState);

        gridInputManager.OnClicked += PlaceStructure;
        gridInputManager.OnExit += StopPlacement;
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingState == null || activeGrid == null)
            return;

        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();

        if (mousePosition == Vector3.zero)
            return;

        Vector3Int gridPosition = activeGrid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            /*bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);

            mouseIndicator.transform.position =
                mousePosition + Vector3.up * 0.5f;

            //cursorIndicator.transform.position = activeGrid.CellToWorld(gridPosition);
            preview.UpdatePosition(activeGrid.CellToWorld(gridPosition), placementValidity);*/
            lastDetectedPosition = gridPosition;
        }
    }

    public void SetActiveGrid(Grid grid, GameObject visualization)
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
