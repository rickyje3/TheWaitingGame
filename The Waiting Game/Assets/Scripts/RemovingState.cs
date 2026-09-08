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
            placementSystem.GetStructureData(hoveredGrid);

        GridData furnitureData =
            placementSystem.GetFurnitureData(hoveredGrid);


        if (structureData == null ||
            furnitureData == null)
        {
            return;
        }


        GridData selectedData = null;


        // Check furniture first.
        if (!furnitureData.CanPlaceObjectAt(
            gridPosition,
            Vector2Int.one))
        {
            selectedData = furnitureData;
        }
        // Then check structure.
        else if (!structureData.CanPlaceObjectAt(
            gridPosition,
            Vector2Int.one))
        {
            selectedData = structureData;
        }


        if (selectedData == null)
            return;


        int gameObjectIndex =
            selectedData.GetRepresentationIndex(
                gridPosition);


        if (gameObjectIndex == -1)
            return;


        selectedData.RemoveObjectAt(
            gridPosition);

        objectPlacer.RemoveObjectAt(
            gameObjectIndex);

        soundFeedback.PlaySound(
            SoundType.Remove);


        Debug.Log(
            "Removing object at " +
            gridPosition +
            " from " +
            hoveredGrid.name);


        previewSystem.UpdatePosition(
            hoveredGrid.GetCellCenterWorld(gridPosition),
            false,
            placementSystem.GetHoveredRotation());
    }


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


        bool canRemove =
            !furnitureData.CanPlaceObjectAt(
                gridPosition,
                Vector2Int.one)
            ||
            !structureData.CanPlaceObjectAt(
                gridPosition,
                Vector2Int.one);


        Vector3 position =
            hoveredGrid.GetCellCenterWorld(
                gridPosition);


        previewSystem.UpdatePosition(
            position,
            canRemove,
            placementSystem.GetHoveredRotation());
    }
}