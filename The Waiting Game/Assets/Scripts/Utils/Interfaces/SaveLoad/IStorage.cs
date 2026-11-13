using UnityEngine;

public interface IStorage<TItemType> where TItemType : IHasUniqueID
{
    /// <summary>
    /// Loads an item from storage based on its unique ID.
    /// </summary>
    /// <param name="uniqueID">The unique ID to identify the object.</param>
    /// <returns>The item with the specified ID (if successful).</returns>
    public Optional<TItemType> LoadItem(GUID uniqueID);

    /// <summary>
    /// Saves an item to storage.
    /// </summary>
    /// <param name="item">The item to be saved.</param>
    /// <returns>True if the item was successfully saved; otherwise, false.</returns>
    public bool SaveItem(TItemType item);
}