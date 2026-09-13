using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string PLACED_OBJECTS_KEY = "PlacedObjects";

    [SerializeField] private GridPlacementSystem placementSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private GameAssets gameAssets;


    // ---------------------------------------------------------
    // SAVE DATA
    // ---------------------------------------------------------

    [Serializable]
    public class PlacedObjectSaveData
    {
        public string itemID;

        public SavedGridType gridType;

        public Vector3Int gridPosition;

        public Vector3 worldPosition;

        public Quaternion rotation;
    }


    [Serializable]
    private class PlacedObjectsSaveData
    {
        public List<PlacedObjectSaveData> objects =
            new List<PlacedObjectSaveData>();
    }


    // ---------------------------------------------------------
    // SAVE
    // ---------------------------------------------------------

    public void SaveGame()
    {
        if (placementSystem == null ||
            objectPlacer == null)
        {
            Debug.LogWarning(
                "SaveManager is missing a reference.");

            return;
        }


        PlacedObjectsSaveData saveData =
            new PlacedObjectsSaveData();


        HashSet<int> savedObjectIndices =
            new HashSet<int>();


        // ---------------------------------------------------------
        // FLOOR
        // ---------------------------------------------------------

        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Floor,
                true),
            SavedGridType.Floor,
            saveData,
            savedObjectIndices);


        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Floor,
                false),
            SavedGridType.Floor,
            saveData,
            savedObjectIndices);


        // ---------------------------------------------------------
        // WALL
        // ---------------------------------------------------------

        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Wall,
                true),
            SavedGridType.Wall,
            saveData,
            savedObjectIndices);


        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Wall,
                false),
            SavedGridType.Wall,
            saveData,
            savedObjectIndices);


        // ---------------------------------------------------------
        // WALL 2
        // ---------------------------------------------------------

        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Wall2,
                true),
            SavedGridType.Wall2,
            saveData,
            savedObjectIndices);


        SaveGrid(
            placementSystem.GetGridData(
                SavedGridType.Wall2,
                false),
            SavedGridType.Wall2,
            saveData,
            savedObjectIndices);


        // ---------------------------------------------------------
        // WRITE SAVE
        // ---------------------------------------------------------

        string json =
            JsonUtility.ToJson(saveData);


        PlayerPrefs.SetString(
            PLACED_OBJECTS_KEY,
            json);


        PlayerPrefs.Save();


        Debug.Log(
            $"Game saved. " +
            $"{saveData.objects.Count} placed objects.");
    }


    private void SaveGrid(
    GridData gridData,
    SavedGridType gridType,
    PlacedObjectsSaveData saveData,
    HashSet<int> savedObjectIndices)
    {
        if (gridData == null)
            return;


        foreach (PlacementData placementData
                 in gridData.GetPlacedObjects())
        {
            if (placementData == null)
                continue;


            int objectIndex =
                placementData.PlacedObjectIndex;


            // -----------------------------------------------------
            // Prevent the same GameObject from being saved twice.
            // -----------------------------------------------------

            if (savedObjectIndices.Contains(objectIndex))
                continue;


            if (objectIndex < 0 ||
                objectIndex >= objectPlacer.placedGameObjects.Count)
            {
                continue;
            }


            GameObject placedObject =
                objectPlacer.placedGameObjects[
                    objectIndex];


            // Removed objects remain null in the list.
            if (placedObject == null)
                continue;


            if (placementData.placedItem == null)
                continue;


            if (placementData.occupiedPositions == null ||
                placementData.occupiedPositions.Count == 0)
            {
                continue;
            }


            // -----------------------------------------------------
            // Create save entry
            // -----------------------------------------------------

            PlacedObjectSaveData objectData =
                new PlacedObjectSaveData();


            objectData.itemID =
                placementData.placedItem.ItemID;


            objectData.gridType =
                gridType;


            // The first occupied position is the original
            // placement origin.
            objectData.gridPosition =
                placementData.occupiedPositions[0];


            objectData.worldPosition =
                placedObject.transform.position;


            objectData.rotation =
                placedObject.transform.rotation;


            saveData.objects.Add(
                objectData);


            savedObjectIndices.Add(
                objectIndex);
        }
    }


    // ---------------------------------------------------------
    // LOAD
    // ---------------------------------------------------------

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(PLACED_OBJECTS_KEY))
        {
            Debug.Log("No placed-object save found.");
            return;
        }


        if (placementSystem == null ||
            objectPlacer == null ||
            gameAssets == null)
        {
            Debug.LogWarning(
                "SaveManager is missing references.");
            return;
        }


        string json =
            PlayerPrefs.GetString(
                PLACED_OBJECTS_KEY);


        if (string.IsNullOrEmpty(json))
            return;


        PlacedObjectsSaveData saveData =
            JsonUtility.FromJson<PlacedObjectsSaveData>(
                json);


        if (saveData == null ||
            saveData.objects == null)
        {
            Debug.LogWarning(
                "Could not read placed-object save.");
            return;
        }


        foreach (PlacedObjectSaveData objectData
                 in saveData.objects)
        {
            LoadPlacedObject(objectData);
        }


        Debug.Log(
            $"Game loaded. " +
            $"{saveData.objects.Count} placed objects.");
    }


    private void LoadPlacedObject(
        PlacedObjectSaveData objectData)
    {
        Item item =
            gameAssets.GetItemByID(
                objectData.itemID);


        if (item == null)
        {
            Debug.LogWarning(
                $"Could not load item with ID: " +
                objectData.itemID);

            return;
        }


        Grid grid =
            placementSystem.GetGrid(
                objectData.gridType);


        if (grid == null)
        {
            Debug.LogWarning(
                $"Could not find grid for " +
                objectData.gridType);

            return;
        }


        GridData gridData =
            item.isFloorObject
                ? placementSystem.GetStructureData(grid)
                : placementSystem.GetFurnitureData(grid);


        if (gridData == null)
        {
            Debug.LogWarning(
                $"Could not find GridData for " +
                $"{item.itemName}");

            return;
        }


        int index =
            objectPlacer.PlaceObject(
                item.Prefab,
                objectData.worldPosition,
                objectData.rotation);


        gridData.AddObjectAt(
            objectData.gridPosition,
            item.Size,
            item,
            index);
    }


    // ---------------------------------------------------------
    // DELETE SAVE
    // ---------------------------------------------------------

    public void DeletePlacedObjectsSave()
    {
        PlayerPrefs.DeleteKey(
            PLACED_OBJECTS_KEY);

        PlayerPrefs.Save();

        Debug.Log(
            "Placed-object save deleted.");
    }
}

