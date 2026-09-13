using UnityEngine;

public class PlacementState : IBuildingState
{
    private Item selectedItem;

    private readonly GridPlacementSystem placementSystem;
    private readonly PreviewSystem previewSystem;
    private readonly ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;


    public PlacementState(
        Item selectedItem,
        PreviewSystem previewSystem,
        GridPlacementSystem placementSystem,
        ObjectPlacer objectPlacer,
        SoundFeedback soundFeedback)
    {
        this.selectedItem = selectedItem;
        this.previewSystem = previewSystem;
        this.placementSystem = placementSystem;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        previewSystem.StartShowingPlacementPreview(
            selectedItem.Prefab,
            selectedItem.Size);
    }


    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }


    private bool CheckPlacementValidity(
        Vector3Int gridPosition,
        Item selectedItem)
    {
        GridData selectedData =
            placementSystem.GetSelectedData();

        if (selectedData == null)
        {
            Debug.Log("Selected data is null in this spot " + gridPosition);
            return false;
        }

        return selectedData.CanPlaceObjectAt(
            gridPosition,
            selectedItem.Size);
    }


    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log(
            $"OnAction called at {gridPosition} " +
            $"Frame: {Time.frameCount}");

        // Make sure we're actually over one of the grids.
        if (placementSystem.hoveredGrid == null)
        {
            Debug.Log("No grid under mouse.");
            return;
        }


        bool placementValidity =
            CheckPlacementValidity(
                gridPosition,
                selectedItem);

        if (!placementValidity)
        {
            soundFeedback.PlaySound(
                SoundType.WrongPlacement);

            return;
        }


        soundFeedback.PlaySound(
            SoundType.Place);

        Debug.Log(
            "Prefab is: " +
            selectedItem.Prefab);


        // Use whichever grid the mouse is currently over.
        Vector3 placementPosition =
            placementSystem.hoveredGrid
                .GetCellCenterWorld(gridPosition);


        Quaternion rotation =
            placementSystem.GetHoveredRotation();

        int index = objectPlacer.PlaceObject(
            selectedItem.Prefab,
            placementPosition,
            rotation);


        // Get the data belonging to the current grid.
        GridData selectedData =
            placementSystem.GetSelectedData();

        if (selectedData == null)
            return;


        Vector3Int origin = gridPosition;


        // Handle even-sized objects.
        if (selectedItem.Size.x % 2 == 0)
            origin.x--;

        if (selectedItem.Size.y % 2 == 0)
            origin.y--;


        selectedData.AddObjectAt(
            origin,
            selectedItem.Size,
            selectedItem,
            index);

        placementSystem.SaveGame();

        Debug.Log(
            $"ADDING {selectedData.GetHashCode()}");

        Debug.Log(
            "Placing " +
            selectedItem.ToString());


        previewSystem.UpdatePosition(
            placementSystem.hoveredGrid.GetCellCenterWorld(gridPosition),
            false,
            placementSystem.GetHoveredRotation());
    }


    public void UpdateState(Vector3Int gridPosition, Vector3 mousePosition)
    {
        if (placementSystem.hoveredGrid == null)
            return;

        bool placementValidity =
            CheckPlacementValidity(gridPosition, selectedItem);

        Vector3 position =
            placementSystem.hoveredGrid.GetCellCenterWorld(gridPosition);

        Quaternion rotation =
            placementSystem.GetHoveredRotation();

        previewSystem.UpdatePosition(
            position,
            placementValidity,
            rotation);
    }
}