using UnityEngine;

/// <summary>
/// An interface that defines an object that has a unique ID.
/// Used for saving and loading data, as well as for identifying objects in the game.
/// </summary>
public interface IHasUniqueID
{
    /// <summary>
    /// Gets the unique ID of the object.
    /// This ID should be unique across all instances of the object.
    /// </summary>
    public GUID UniqueID { get; }
}