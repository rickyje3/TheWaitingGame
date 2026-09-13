using UnityEngine;

public class RemovingState : IBuildingState
{
    private readonly GridPlacementSystem placementSystem;
    private readonly PreviewSystem previewSystem;
    private readonly ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;


    public RemovingState(
        GridPlacementSystem placementSystem,
        PreviewSystem previewSystem,
        ObjectPlacer objectPlacer,
        SoundFeedback soundFeedback)
    {
        this.placementSystem = placementSystem;
        this.previewSystem = previewSystem;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        previewSystem.StartShowingRemovePreview();
    }


    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }


    // ---------------------------------------------------------
    // REMOVE OBJECT
    // ---------------------------------------------------------

    public void OnAction(Vector3Int gridPosition)
    {
        Grid hoveredGrid =
            placementSystem.hoveredGrid;

        if (hoveredGrid == null)
        {
            Debug.Log("No grid under mouse.");
            return;
        }


        // Get the data belonging to the grid
        // we're currently hovering.
        GridData structureData =
            placementSystem.GetStructureData(
                hoveredGrid);

        GridData furnitureData =
            placementSystem.GetFurnitureData(
                hoveredGrid);


        if (structureData == null ||
            furnitureData == null)
        {
            return;
        }


        // -----------------------------------------------------
        // Find the object occupying this cell.
        // -----------------------------------------------------

        PlacementData placementData =
            furnitureData.GetPlacementDataAt(
                gridPosition);


        GridData selectedData =
            furnitureData;


        // If there is no furniture here, check structure.
        if (placementData == null)
        {
            placementData =
                structureData.GetPlacementDataAt(
                    gridPosition);

            selectedData =
                structureData;
        }


        // Nothing is occupying this cell.
        if (placementData == null)
            return;


        // -----------------------------------------------------
        // Get the actual placed object.
        // -----------------------------------------------------

        Item placedItem =
            placementData.placedItem;


        if (placedItem == null)
        {
            Debug.LogWarning(
                "PlacementData has no Item.");

            return;
        }


        // Blockers cannot be removed.
        if (placedItem.isBlocker)
        {
            Debug.Log(
                "This object is a blocker and cannot be removed.");

            return;
        }


        int gameObjectIndex =
            placementData.PlacedObjectIndex;


        if (gameObjectIndex < 0)
        {
            Debug.LogWarning(
                "PlacementData has an invalid GameObject index.");

            return;
        }


        // -----------------------------------------------------
        // Remove from GridData.
        // -----------------------------------------------------

        selectedData.RemoveObjectAt(
            gridPosition);


        // -----------------------------------------------------
        // Remove actual GameObject.
        // -----------------------------------------------------

        objectPlacer.RemoveObjectAt(
            gameObjectIndex);


        // -----------------------------------------------------
        // SAVE AFTER SUCCESSFUL REMOVAL
        // -----------------------------------------------------

        placementSystem.SaveGame();


        soundFeedback.PlaySound(
            SoundType.Remove);


        Debug.Log(
            "Removing object at " +
            gridPosition +
            " from " +
            hoveredGrid.name);


        previewSystem.UpdatePosition(
            hoveredGrid.GetCellCenterWorld(
                gridPosition),
            false,
            placementSystem.GetHoveredRotation());
    }


    // ---------------------------------------------------------
    // UPDATE REMOVE PREVIEW
    // ---------------------------------------------------------

    public void UpdateState(
        Vector3Int gridPosition,
        Vector3 mousePosition)
    {
        Grid hoveredGrid =
            placementSystem.hoveredGrid;

        if (hoveredGrid == null)
            return;


        GridData structureData =
            placementSystem.GetStructureData(
                hoveredGrid);

        GridData furnitureData =
            placementSystem.GetFurnitureData(
                hoveredGrid);


        if (structureData == null ||
            furnitureData == null)
        {
            return;
        }


        // -----------------------------------------------------
        // Check whether either type of object occupies
        // this cell.
        // -----------------------------------------------------

        PlacementData furnitureDataAtCell =
            furnitureData.GetPlacementDataAt(
                gridPosition);


        PlacementData structureDataAtCell =
            structureData.GetPlacementDataAt(
                gridPosition);


        bool canRemove = false;


        if (furnitureDataAtCell != null)
        {
            if (furnitureDataAtCell.placedItem != null &&
                !furnitureDataAtCell.placedItem.isBlocker)
            {
                canRemove = true;
            }
        }
        else if (structureDataAtCell != null)
        {
            if (structureDataAtCell.placedItem != null &&
                !structureDataAtCell.placedItem.isBlocker)
            {
                canRemove = true;
            }
        }


        Vector3 position =
            hoveredGrid.GetCellCenterWorld(
                gridPosition);


        previewSystem.UpdatePosition(
            position,
            canRemove,
            placementSystem.GetHoveredRotation());
    }
}