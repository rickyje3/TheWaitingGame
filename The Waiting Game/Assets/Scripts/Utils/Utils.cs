using UnityEngine;

public class Utils
{
    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("ScriptableObjects");
    }
}
