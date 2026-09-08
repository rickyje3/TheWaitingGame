using System;
using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;

    [SerializeField] private GridInputManager gridInputManager;

    // ---------------------------------------------------------
    // GRIDS
    // ---------------------------------------------------------

    public Grid floorGrid;
    public Grid wallGrid;
    public Grid wall2Grid;

    // ---------------------------------------------------------
    // COLLIDERS
    // ---------------------------------------------------------

    public Collider floorCollider;
    public Collider wallCollider;
    public Collider wall2Collider;

    // ---------------------------------------------------------
    // LAYERS
    // ---------------------------------------------------------

    public LayerMask floorLayer;
    public LayerMask wallLayer;
    public LayerMask wall2Layer;

    // ---------------------------------------------------------
    // GRID VISUALIZATIONS
    // ---------------------------------------------------------

    public GameObject gridVisualizationFloor;
    public GameObject gridVisualizationWall;
    public GameObject gridVisualizationWall2;

    [SerializeField] private GameAssets gameAssets;

    // ---------------------------------------------------------
    // ITEM
    // ---------------------------------------------------------

    [HideInInspector] public Item selectedItem;

    // The grid the mouse is CURRENTLY hovering over.
    [HideInInspector] public Grid hoveredGrid;

    // ---------------------------------------------------------
    // GRID DATA
    // ---------------------------------------------------------

    private GridData floorData;
    private GridData furnitureData;

    private GridData wallData;
    private GridData wallFurnitureData;

    private GridData wall2Data;
    private GridData wall2FurnitureData;

    // ---------------------------------------------------------
    // SYSTEMS
    // ---------------------------------------------------------

    [SerializeField] private PreviewSystem preview;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private SoundFeedback soundFeedback;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    private Grid lastDetectedGrid;

    private IBuildingState buildingState;

    private Vector3 currentPlacementPosition;

    public Vector3 CurrentPlacementPosition => currentPlacementPosition;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (gameAssets == null)
            gameAssets = FindAnyObjectByType<GameAssets>();

        // -----------------------------------------------------
        // FLOOR
        // -----------------------------------------------------

        floorData = new GridData(
            floorGrid,
            floorCollider,
            floorLayer);

        furnitureData = new GridData(
            floorGrid,
            floorCollider,
            floorLayer);

        // -----------------------------------------------------
        // WALL 1
        // -----------------------------------------------------

        wallData = new GridData(
            wallGrid,
            wallCollider,
            wallLayer);

        wallFurnitureData = new GridData(
            wallGrid,
            wallCollider,
            wallLayer);

        // -----------------------------------------------------
        // WALL 2
        // -----------------------------------------------------

        wall2Data = new GridData(
            wall2Grid,
            wall2Collider,
            wall2Layer);

        wall2FurnitureData = new GridData(
            wall2Grid,
            wall2Collider,
            wall2Layer);

        // -----------------------------------------------------
        // INITIAL STATE
        // -----------------------------------------------------

        StopPlacement();
    }


    // =========================================================
    // GET MOUSE POSITION + GRID
    // =========================================================

    private bool TryGetHoveredGrid(
    out Vector3 mousePosition,
    out Grid grid)
    {
        mousePosition = Vector3.zero;
        grid = null;

        Ray ray =
            Camera.main.ScreenPointToRay(Input.mousePosition);

        int combinedMask;

        if (selectedItem != null && selectedItem.isWallObject)
        {
            combinedMask =
                wallLayer |
                wall2Layer;
        }
        else
        {
            combinedMask =
                floorLayer;
        }

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            Mathf.Infinity,
            combinedMask))
        {
            return false;
        }

        mousePosition = hit.point;

        if (hit.collider == floorCollider)
            grid = floorGrid;

        else if (hit.collider == wallCollider)
            grid = wallGrid;

        else if (hit.collider == wall2Collider)
            grid = wall2Grid;

        return grid != null;
    }


    // =========================================================
    // GET DATA FOR CURRENT GRID
    // =========================================================

    public GridData GetSelectedData()
    {
        if (selectedItem == null || hoveredGrid == null)
            return null;

        // -----------------------------------------------------
        // FLOOR
        // -----------------------------------------------------

        if (hoveredGrid == floorGrid)
        {
            return selectedItem.isFloorObject
                ? floorData
                : furnitureData;
        }

        // -----------------------------------------------------
        // WALL 1
        // -----------------------------------------------------

        if (hoveredGrid == wallGrid)
        {
            return selectedItem.isFloorObject
                ? wallData
                : wallFurnitureData;
        }

        // -----------------------------------------------------
        // WALL 2
        // -----------------------------------------------------

        if (hoveredGrid == wall2Grid)
        {
            return selectedItem.isFloorObject
                ? wall2Data
                : wall2FurnitureData;
        }

        return null;
    }

    public GridData GetStructureData(Grid grid)
    {
        if (grid == floorGrid)
            return floorData;

        if (grid == wallGrid)
            return wallData;

        if (grid == wall2Grid)
            return wall2Data;

        return null;
    }


    public GridData GetFurnitureData(Grid grid)
    {
        if (grid == floorGrid)
            return furnitureData;

        if (grid == wallGrid)
            return wallFurnitureData;

        if (grid == wall2Grid)
            return wall2FurnitureData;

        return null;
    }


    // =========================================================
    // PLACE STRUCTURE
    // =========================================================

    private void PlaceStructure()
    {
        Debug.Log("Starting place structure");

        if (buildingState == null)
            return;

        if (buildingState is PlacementState &&
            selectedItem == null)
        {
            Debug.Log("No selected item");
            return;
        }

        if (!TryGetHoveredGrid(
            out Vector3 mousePosition,
            out Grid grid))
        {
            Debug.Log("Mouse is not over a placement surface");
            return;
        }

        // This is now the grid we are actually placing on.
        hoveredGrid = grid;

        Vector3Int gridPosition =
            hoveredGrid.WorldToCell(mousePosition);

        Debug.Log(
            $"Mouse: {mousePosition:F2} " +
            $"Cell: {gridPosition} " +
            $"Grid: {hoveredGrid.name}");

        Debug.DrawLine(
            mousePosition,
            hoveredGrid.GetCellCenterWorld(gridPosition),
            Color.red,
            0.1f);

        buildingState.OnAction(gridPosition);
    }


    // =========================================================
    // STOP PLACEMENT
    // =========================================================

    public void StopPlacement()
    {
        Debug.Log("Stopping placement");

        selectedItem = null;
        hoveredGrid = null;

        gridVisualizationFloor.SetActive(false);
        gridVisualizationWall.SetActive(false);
        gridVisualizationWall2.SetActive(false);

        if (buildingState != null)
            buildingState.EndState();

        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;
        lastDetectedGrid = null;

        buildingState = null;
    }


    // =========================================================
    // START PLACEMENT
    // =========================================================

    public void StartPlacement(Item item)
    {
        Debug.Log(
            "Subscribed to " +
            gridInputManager.GetEntityId());

        StopPlacement();

        selectedItem = item;

        if (!selectedItem.isCosmetic)
        {
            ShowPlacementGrids();

            buildingState = new PlacementState(
                selectedItem,
                preview,
                this,
                objectPlacer,
                soundFeedback);

            mouseIndicator.SetActive(true);

            gridInputManager.OnClicked -= PlaceStructure;
            gridInputManager.OnClicked += PlaceStructure;

            gridInputManager.OnExit -= StopPlacement;
            gridInputManager.OnExit += StopPlacement;
        }
    }


    // =========================================================
    // START REMOVING
    // =========================================================

    public void StartRemoving()
    {
        StopPlacement();

        gridVisualizationFloor.SetActive(true);
        gridVisualizationWall.SetActive(true);
        gridVisualizationWall2.SetActive(true);

        buildingState = new RemovingState(
            this,
            preview,
            objectPlacer,
            soundFeedback);

        Debug.Log("Starting Remove Mode");

        gridInputManager.OnClicked -= PlaceStructure;
        gridInputManager.OnClicked += PlaceStructure;

        gridInputManager.OnExit -= StopPlacement;
        gridInputManager.OnExit += StopPlacement;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (buildingState == null)
            return;

        if (!TryGetHoveredGrid(out Vector3 mousePosition, out Grid grid))
            return;

        hoveredGrid = grid;

        Vector3Int gridPosition =
            hoveredGrid.WorldToCell(mousePosition);

        Vector3 cellCenter =
            hoveredGrid.GetCellCenterWorld(gridPosition);

        // ---------------------------------------------------------
        // POSITION
        // ---------------------------------------------------------

        mouseIndicator.transform.position = mousePosition;

        cellIndicator.transform.position = cellCenter;


        // ---------------------------------------------------------
        // ROTATION
        // ---------------------------------------------------------

        Quaternion hoveredRotation = GetHoveredRotation();

        cellIndicator.transform.rotation = hoveredRotation;


        // ---------------------------------------------------------
        // UPDATE PREVIEW
        // ---------------------------------------------------------

        if (lastDetectedGrid != hoveredGrid ||
            lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(
                gridPosition,
                mousePosition);

            lastDetectedGrid = hoveredGrid;
            lastDetectedPosition = gridPosition;
        }


        // ---------------------------------------------------------
        // RIGHT CLICK
        // ---------------------------------------------------------

        if (Input.GetKeyDown(KeyCode.Mouse1))
            StopPlacement();
    }


    // =========================================================
    // GRID VISUALIZATION
    // =========================================================

    public void ShowFloorGrid()
    {
        gridVisualizationFloor.SetActive(true);
    }

    public void ShowWallGrid()
    {
        gridVisualizationWall.SetActive(true);
    }

    public void ShowWall2Grid()
    {
        gridVisualizationWall2.SetActive(true);
    }

    public void ShowPlacementGrids()
    {
        if (selectedItem == null)
            return;

        if (selectedItem.isWallObject)
        {
            gridVisualizationFloor.SetActive(false);
            gridVisualizationWall.SetActive(true);
            gridVisualizationWall2.SetActive(true);
        }
        else
        {
            gridVisualizationFloor.SetActive(true);
            gridVisualizationWall.SetActive(false);
            gridVisualizationWall2.SetActive(false);
        }
    }


    public Quaternion GetHoveredRotation()
    {
        if (hoveredGrid == wallGrid)
            return Quaternion.Euler(-45f, 45f, -90f);

        if (hoveredGrid == wall2Grid)
            return Quaternion.Euler(-45f, 130f, -90f);

        return Quaternion.identity;
    }
}