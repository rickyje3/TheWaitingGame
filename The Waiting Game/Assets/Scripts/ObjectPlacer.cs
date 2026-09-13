using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<GameObject> placedGameObjects = new();
    public PreviewSystem previewSystem;

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (gameObjectIndex < 0 || gameObjectIndex >= placedGameObjects.Count)
        {
            return;
        }

        if (placedGameObjects[gameObjectIndex] != null)
        {
            Destroy(placedGameObjects[gameObjectIndex]);
        }

        placedGameObjects[gameObjectIndex] = null;
    }

    public int PlaceObject(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation)
    {
        GameObject newObject =
            Instantiate(prefab, position, rotation);

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    public void ClearPlacedObjects()
    {
        foreach (GameObject obj in placedGameObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        placedGameObjects.Clear();
    }
}
