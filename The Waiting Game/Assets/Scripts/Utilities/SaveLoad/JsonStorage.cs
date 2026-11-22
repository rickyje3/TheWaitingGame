using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Alchemy.Serialization;
using UnityEngine;
using Utilities.Monads;

namespace Utilities.SaveLoad
{
    [Serializable, AlchemySerialize]
    public abstract partial class JsonStorage<TItemType, TItemArrayType> : IStorage<TItemType>
        where TItemType : IHasUniqueID
        where TItemArrayType : IEnumerable<TItemType>
    {
        /// <summary>
        /// The file name where the data will be stored in JSON format.
        /// </summary>
        [SerializeField] private string storageFileName = "";

        /// <summary>
        /// The backing dictionary that stores the data.
        /// This is here to avoid constantly reading and writing to the JSON file, which can be inefficient.
        /// </summary>
        [AlchemySerializeField, NonSerialized, ReadOnly]
        private Dictionary<GUID, TItemType> _dataDictionary = new();

        /// <summary>
        /// A flag that indicates whether the  data has been modified since the last save.
        /// This is used to determine whether the data needs to be saved to the JSON file.
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// A flag that indicates whether the data needs to be loaded from the JSON file.
        /// This is used to determine whether the data needs to be loaded from the JSON file when the application starts or when the data is explicitly loaded.
        /// </summary>
        private bool _needsToLoad = true;

        private string StorageFilePath => System.IO.Path.Combine(Application.persistentDataPath, storageFileName);

        #region IStorage Implementation

        /// <summary>
        /// Loads a data item from the storage by its unique ID.
        /// </summary>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Optional<TItemType> LoadItem(GUID uniqueID)
        {
            // If the data needs to be loaded from the JSON file, do so before attempting to load the item.
            if (_needsToLoad)
                LoadDataFromBackEnd();

            // If the item exists in the dictionary, return it wrapped in an Optional.
            if (_dataDictionary.TryGetValue(uniqueID, out var data))
                return data;

            // If the item does not exist, return an empty Optional.
            return Optional<TItemType>.None;
        }

        public bool SaveItem(TItemType item)
        {
            // If the data needs to be loaded from the JSON file, do so before attempting to save the item.
            if (_needsToLoad)
                LoadDataFromBackEnd();

            // Set the information in the dictionary to the new item.
            // If the item already exists, it will be updated; if it does not exist, it will be added.
            _dataDictionary[item.UniqueID] = item;

            // Mark the data as dirty to indicate that it needs to be saved to the JSON file.
            _isDirty = true;

            // Save the data to the JSON file after saving the item.
            SaveDataToBackEnd();

            return true;
        }

        public Optional<TItemType> RemoveItem(GUID uniqueID)
        {
            // If the data needs to be loaded from the JSON file, do so before attempting to remove the item.
            if (_needsToLoad)
                LoadDataFromBackEnd();

            // If the item exists in the dictionary, remove it and return it wrapped in an Optional.
            if (_dataDictionary.Remove(uniqueID, out var data))
            {
                // Mark the data as dirty to indicate that it needs to be saved to the JSON file.
                _isDirty = true;

                // Save the data to the JSON file after removing the item.
                SaveDataToBackEnd();

                return data;
            }

            // If the item does not exist, return an empty Optional.
            return Optional<TItemType>.None;
        }

        public List<GUID> GetAllStoredItemIDs()
        {
            // If the data needs to be loaded from the JSON file, do so before attempting to get the item IDs.
            if (_needsToLoad)
                LoadDataFromBackEnd();

            // Return a list of all the unique IDs of the stored items.
            return new List<GUID>(_dataDictionary.Keys);
        }

        public List<TItemType> GetAllStoredItems()
        {
            // If the data needs to be loaded from the JSON file, do so before attempting to get the stored items.
            if (_needsToLoad)
                LoadDataFromBackEnd();

            // Return a list of all the stored items.
            return new List<TItemType>(_dataDictionary.Values);
        }

        #endregion


        private bool LoadDataFromBackEnd()
        {
            string fullFilePath = StorageFilePath;

            // Log the loading process for debugging purposes.
            Debug.Log($"Loading data from JSON file: {fullFilePath}");

            // Check if the JSON file exists. If it does not, create an empty dictionary and return true.
            if (!System.IO.File.Exists(fullFilePath))
                _dataDictionary = new Dictionary<GUID, TItemType>();

            // Otherwise, read the JSON file and deserialize it into the dictionary.
            else
            {
                var jsonString = System.IO.File.ReadAllText(fullFilePath);
                var dataArray = JsonUtility.FromJson<TItemArrayType>(jsonString);

                // Populate the dictionary with the deserialized data.
                foreach (var data in dataArray)
                    _dataDictionary[data.UniqueID] = data;
            }

            _needsToLoad = false;

            // If successful, return true.
            return true;
        }

        private bool SaveDataToBackEnd()
        {
            // Skip saving if the data has not been modified since the last save.
            if (!_isDirty)
                return true;

            string fullFilePath = StorageFilePath;

            // Log the saving process for debugging purposes.
            Debug.Log($"Saving data to JSON file: {fullFilePath}");

            // Create a list to store the JSON strings of each data.
            var dataList = CreateItemArray(_dataDictionary.Values);
            var jsonString = JsonUtility.ToJson(dataList);

            // Save the JSON array string to the JSON file.
            System.IO.File.WriteAllText(fullFilePath, jsonString);

            // Reset the dirty flag to indicate that the data has been saved and is no longer dirty.
            _isDirty = false;

            // If successful, return true.
            return true;
        }

        protected abstract TItemArrayType CreateItemArray(IEnumerable<TItemType> items);
    }
}