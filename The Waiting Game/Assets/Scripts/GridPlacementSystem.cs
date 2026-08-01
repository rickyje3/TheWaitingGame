using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UI;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator; 
    [SerializeField] private GridInputManager gridInputManager;
    public Grid floorGrid;
    public Grid loftGrid;
    public Collider floorCollider; //grid
    public Collider loftCollider; // grid
    public LayerMask floorLayer;
    public LayerMask loftLayer;

    public GameObject gridVisualizationFloor;
    public GameObject gridVisualizationLoft;
    [SerializeField] private GameAssets gameAssets;

    [HideInInspector] public Item selectedItem;
    [HideInInspector] public Grid activeGrid;
    private GameObject activeGridVisualization;

    private GridData floorData;
    private GridData furnitureData;

    private GridData loftFloorData;
    private GridData loftFurnitureData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private SoundFeedback soundFeedback;

    private Vector3 currentPlacementPosition;
    public Vector3 CurrentPlacementPosition => currentPlacementPosition;

    IBuildingState buildingState;


    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        floorData = new GridData(
            floorGrid,
            floorCollider,
            floorLayer
        );

        furnitureData = new GridData(
            floorGrid,
            floorCollider,
            floorLayer
        );

        loftFloorData = new GridData(
            loftGrid,
            loftCollider,
            loftLayer
        );

        loftFurnitureData = new GridData(
            loftGrid,
            loftCollider,
            loftLayer
        );

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
        Debug.Log($"Mouse: {mousePosition:F2}  Cell: {gridPosition}");

        Debug.DrawLine(
            mousePosition,
            activeGrid.CellToWorld(gridPosition),
            Color.red,
            0.1f);

        /*Debug.Log($"Mouse: {mousePosition}");
        Debug.Log($"Cell: {gridPosition}");
        Debug.Log($"Center: {activeGrid.GetCellCenterWorld(gridPosition)}");*/

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

        bool onFloorGrid = activeGrid == floorGrid;

        if (onFloorGrid)
        {
            return selectedItem.isFloorObject
                ? floorData
                : furnitureData;
        }
        else
        {
            return selectedItem.isFloorObject
                ? loftFloorData
                : loftFurnitureData;
        }
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
        //Debug.Log("Started placement of " + item.itemName);
        Debug.Log("Subscribed to " + gridInputManager.GetEntityId());

        StopPlacement();

        selectedItem = item;

        if (!selectedItem.isCosmetic)
        {
            buildingState = new PlacementState(selectedItem, activeGrid, preview, this, objectPlacer, soundFeedback);

            mouseIndicator.SetActive(true);

            // Prevent duplicate subscriptions
            gridInputManager.OnClicked -= PlaceStructure;
            gridInputManager.OnClicked += PlaceStructure;

            gridInputManager.OnExit -= StopPlacement;
            gridInputManager.OnExit += StopPlacement;
        }
    }

    public void StartRemoving()
    {
        StopPlacement();
        activeGridVisualization.SetActive(true);
        buildingState = new RemovingState(this, preview, activeGrid, floorData, furnitureData, objectPlacer, soundFeedback);

        Debug.Log("Starting Remove Mode");
        Debug.Log(buildingState);

        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnClicked += PlaceStructure;

        gridInputManager.OnExit -= StopPlacement;
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

        mouseIndicator.transform.position = mousePosition;

        cellIndicator.transform.position = activeGrid.GetCellCenterWorld(gridPosition);

        Vector3 cellCenter = activeGrid.GetCellCenterWorld(gridPosition);

        Debug.DrawLine(
            mousePosition,
            cellCenter,
            Color.yellow
        );

        Debug.DrawLine(
            cellCenter + Vector3.up * 0.5f,
            cellCenter + Vector3.down * 0.5f,
            Color.blue
        );


        //Debug.Log($"Mouse: {mousePosition}");
        //Debug.Log($"Grid Cell: {gridPosition}");

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition, mousePosition);
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
