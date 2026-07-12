using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private readonly GridPlacementSystem placementSystem;
    private readonly PreviewSystem previewSystem;
    private Grid grid;
    private GridData floorData, furnitureData;
    private readonly ObjectPlacer objectPlacer;
    private SoundFeedback soundFeedback;

    public RemovingState(GridPlacementSystem placementSystem, PreviewSystem previewSystem, Grid grid, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer, SoundFeedback soundFeedback)
    {
        this.placementSystem = placementSystem;
        this.previewSystem = previewSystem;
        this.grid = grid;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
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
        GridData selectedData = null;

        if (!furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = furnitureData;
        }
        else if (!floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one))
        {
            selectedData = floorData;
        }

        if (selectedData == null)
        {
            return;
        }

        int gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);

        if (gameObjectIndex == -1)
        {
            return;
        }

        selectedData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(gameObjectIndex);

        soundFeedback.PlaySound(SoundType.Remove);

        Debug.Log("Removing object at " + gridPosition);
            
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool canRemove = !furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) || !floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), canRemove);
    }
}
