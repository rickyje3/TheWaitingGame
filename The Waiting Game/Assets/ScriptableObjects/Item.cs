using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string Id;
    public bool CanConjure;
    public Sprite Icon;
    public bool IsLocked;
    public int Quantity;
    public int PerSecond;
}
