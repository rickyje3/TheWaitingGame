using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(
            prefab,
            position,
            Quaternion.identity
            );

        placedGameObjects.Add(obj);
        return placedGameObjects.Count - 1;
    }
}
