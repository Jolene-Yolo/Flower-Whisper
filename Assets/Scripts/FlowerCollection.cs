using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager that tracks which flower species have reached full bloom
/// (growthStage == 5). The collection is stored as a JSON file in
/// Application.persistentDataPath so it survives scene reloads during
/// the same runtime session.
///
/// Data storage approach:
///   - A simple <see cref="CollectionData"/> wrapper holds a List&lt;string&gt;
///     of discovered species names.
///   - On Start, the file is loaded (if it exists) to restore previous entries.
///   - On every new discovery, the list is saved back to disk as JSON via
///     Unity's built-in JsonUtility.
///   - A HashSet is used alongside the list for O(1) duplicate checks.
///   - The JSON file lives at {Application.persistentDataPath}/flower_collection.json.
///
/// Other scripts call <see cref="FlowerCollection.Instance"/> to access the
/// singleton, then <see cref="TryAddSpecies"/> to register a species.
/// </summary>
public class FlowerCollection : MonoBehaviour
{
    /// <summary>Singleton instance accessible from any script.</summary>
    public static FlowerCollection Instance { get; private set; }

    [Header("Debug")]
    [Tooltip("Read-only list of discovered species shown in the Inspector.")]
    [SerializeField]
    private List<string> discoveredSpecies = new List<string>();

    /// <summary>Fast lookup to prevent duplicates without scanning the list.</summary>
    private HashSet<string> speciesSet = new HashSet<string>();

    /// <summary>File name used for JSON persistence.</summary>
    private const string FileName = "flower_collection.json";

    /// <summary>
    /// Serializable wrapper so JsonUtility can read/write the list.
    /// JsonUtility does not support top-level arrays or HashSets.
    /// </summary>
    [System.Serializable]
    private class CollectionData
    {
        public List<string> species = new List<string>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[FlowerWhisper] Duplicate FlowerCollection detected — destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadCollection();
    }

    /// <summary>
    /// Attempts to add a species to the collection.
    /// Returns true if the species was new, false if it was already collected.
    /// </summary>
    public bool TryAddSpecies(string species)
    {
        if (string.IsNullOrEmpty(species))
            return false;

        if (speciesSet.Contains(species))
        {
            Debug.Log($"[FlowerWhisper] Collection: '{species}' already discovered — skipping.");
            return false;
        }

        speciesSet.Add(species);
        discoveredSpecies.Add(species);
        SaveCollection();

        Debug.Log($"[FlowerWhisper] Collection: '{species}' discovered! Total: {discoveredSpecies.Count}");
        return true;
    }

    /// <summary>Returns the number of unique species collected so far.</summary>
    public int Count => discoveredSpecies.Count;

    /// <summary>Returns true if the given species has already been collected.</summary>
    public bool HasSpecies(string species)
    {
        return speciesSet.Contains(species);
    }

    /// <summary>Returns a read-only copy of all discovered species.</summary>
    public List<string> GetAllSpecies()
    {
        return new List<string>(discoveredSpecies);
    }

    /// <summary>
    /// Saves the current collection to a JSON file at Application.persistentDataPath.
    /// </summary>
    private void SaveCollection()
    {
        CollectionData data = new CollectionData { species = discoveredSpecies };
        string json = JsonUtility.ToJson(data, true);
        string path = System.IO.Path.Combine(Application.persistentDataPath, FileName);

        try
        {
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"[FlowerWhisper] Collection saved to {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FlowerWhisper] Failed to save collection: {e.Message}");
        }
    }

    /// <summary>
    /// Loads the collection from the JSON file if it exists.
    /// Rebuilds both the list and the HashSet from the saved data.
    /// </summary>
    private void LoadCollection()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, FileName);

        if (!System.IO.File.Exists(path))
        {
            Debug.Log("[FlowerWhisper] Collection: no saved data found — starting fresh.");
            return;
        }

        try
        {
            string json = System.IO.File.ReadAllText(path);
            CollectionData data = JsonUtility.FromJson<CollectionData>(json);

            if (data != null && data.species != null)
            {
                discoveredSpecies.Clear();
                speciesSet.Clear();

                foreach (string s in data.species)
                {
                    if (!string.IsNullOrEmpty(s) && speciesSet.Add(s))
                    {
                        discoveredSpecies.Add(s);
                    }
                }

                Debug.Log($"[FlowerWhisper] Collection loaded — {discoveredSpecies.Count} species restored.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FlowerWhisper] Failed to load collection: {e.Message}");
        }
    }
}
