using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private HashSet<string> keys = new HashSet<string>(); // Stores the player's keys

    private void Awake()
    {
        // Ensure only one instance of InventoryManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Make this GameObject persistent
    }

    /// <summary>
    /// Adds a key to the inventory.
    /// </summary>
    /// <param name="keyID">The unique ID of the key.</param>
    public void AddKey(string keyID)
    {
        keys.Add(keyID);
        Debug.Log($"Key added to inventory: {keyID}");
    }

    /// <summary>
    /// Checks if the inventory contains a specific key.
    /// </summary>
    /// <param name="keyID">The unique ID of the key.</param>
    /// <returns>True if the key exists in the inventory, false otherwise.</returns>
    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }
}
