using System;
using System.Collections.Generic;

public enum SavedGridType
{
    Floor,
    Wall,
    Wall2
}


[Serializable]
public class PlacedObjectSaveData
{
    public string itemID;

    public SavedGridType gridType;

    public int gridX;
    public int gridY;
    public int gridZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;
    public float rotationW;
}


[Serializable]
public class PlacementSaveData
{
    public List<PlacedObjectSaveData> placedObjects =
        new List<PlacedObjectSaveData>();
}