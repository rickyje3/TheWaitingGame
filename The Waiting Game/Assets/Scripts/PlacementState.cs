using UnityEngine;
using static UnityEditor.Progress;

public class PlacementState : IBuildingState
{
    private Item selectedItem;
    private readonly GridPlacementSystem placementSystem;
    private readonly PreviewSystem previewSystem;
    private Grid grid;
    private GridData selectedData;
    private readonly ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public PlacementState(Item selectedItem, Grid grid, PreviewSystem previewSystem, GridPlacementSystem placementSystem, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer, SoundFeedback soundFeedback)
    {
        this.selectedItem = selectedItem;
        this.previewSystem = previewSystem;
        this.placementSystem = placementSystem;
        this.grid = grid;
        this.selectedData = placementSystem.GetSelectedData();
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;

        selectedItem = placementSystem.selectedItem;

        if (selectedItem != null)
        {
            placementSystem.SetActiveGrid(
            placementSystem.floorGrid,
            placementSystem.gridVisualizationFloor);

            previewSystem.StartShowingPlacementPreview(placementSystem.selectedItem.Prefab, placementSystem.selectedItem.Size);
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, Item selectedItem)
    {
        //GridData selectedData = selectedItem.isFloorObject ? floorData : furnitureData;
        //Debug.Log($"CHECKING {selectedData.GetHashCode()}");
        return placementSystem.GetSelectedData().CanPlaceObjectAt(
            gridPosition,
            selectedItem.Size);
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
