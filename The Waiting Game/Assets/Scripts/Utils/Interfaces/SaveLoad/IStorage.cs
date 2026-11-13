using System.Collections.Generic;
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

    /// <summary>
    /// Removes an item from storage based on its unique ID.
    /// </summary>
    /// <param name="uniqueID">The unique ID to identify the object.</param>
    /// <returns>An Optional containing the removed item if successful; otherwise, an empty Optional.</returns>
    public Optional<TItemType> RemoveItem(GUID uniqueID);

    /// <summary>
    /// Retrieves a list of all stored item IDs.
    /// </summary>
    /// <returns>A list of all stored item IDs.</returns>
    public List<GUID> GetAllStoredItemIDs();

    /// <summary>
    /// Retrieves a list of all stored items.
    /// </summary>
    /// <note>Use sparingly, as this may be a costly operation depending on the storage implementation.</note>
    /// <note>Changes made to the returned list will not affect the storage.</note>
    /// <returns></returns>
    public List<TItemType> GetAllStoredItems();
}