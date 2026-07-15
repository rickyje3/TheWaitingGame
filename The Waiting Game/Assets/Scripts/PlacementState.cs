using UnityEngine;
using static UnityEditor.Progress;

public class PlacementState : IBuildingState
{
    private Item selectedItem;
    private readonly GridPlacementSystem placementSystem;
    private readonly PreviewSystem previewSystem;
    private Grid grid;
    private readonly ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public PlacementState(Item selectedItem, Grid grid, PreviewSystem previewSystem, GridPlacementSystem placementSystem, ObjectPlacer objectPlacer, SoundFeedback soundFeedback)
    {
        this.selectedItem = selectedItem;
        this.previewSystem = previewSystem;
        this.placementSystem = placementSystem;
        this.grid = grid;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        selectedItem = placementSystem.selectedItem;

        if (selectedItem != null)
        {
            placementSystem.SetActiveGrid(
            placementSystem.floorGrid,
            placementSystem.gridVisualizationFloor);
        }

        previewSystem.StartShowingPlacementPreview(
        placementSystem.selectedItem.Prefab,
        placementSystem.selectedItem.Size);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Item selectedItem)
    {
        //GridData selectedData = selectedItem.isFloorObject ? floorData : furnitureData;
        //Debug.Log($"CHECKING {selectedData.GetHashCode()}");
        //Debug.Log($"Checking {gridPosition}");

        bool valid = placementSystem.GetSelectedData().CanPlaceObjectAt(
            gridPosition,
            selectedItem.Size);

        //Debug.Log($"Can place: {valid}");

        return valid;
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);

        if (placementValidity == false)
        {
            soundFeedback.PlaySound(SoundType.WrongPlacement);
            return;
        }

        soundFeedback.PlaySound(SoundType.Place);

        Debug.Log("Prefab is: " + selectedItem.Prefab);


        int index = objectPlacer.PlaceObject(selectedItem.Prefab, placementSystem.activeGrid.CellToWorld(gridPosition));

        GridData selectedData = placementSystem.GetSelectedData();

        selectedData.AddObjectAt(
            gridPosition,
            selectedItem.Size,
            selectedItem,
            objectPlacer.placedGameObjects.Count - 1);

        Debug.Log($"ADDING {selectedData.GetHashCode()}");
        Debug.Log("Placing " + selectedItem.ToString());

        previewSystem.UpdatePosition(placementSystem.activeGrid.CellToWorld(gridPosition), false);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedItem);

        //cursorIndicator.transform.position = activeGrid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(placementSystem.activeGrid.CellToWorld(gridPosition), placementValidity);
    }
}
