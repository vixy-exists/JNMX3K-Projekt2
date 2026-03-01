using UnityEngine;

public class DungeonDecider : MonoBehaviour
{
    [Header("Assign dungeon prefabs (each prefab contains both background and walls)")]
    [Tooltip("Prefabs for full dungeons (background + walls combined).")]
    public GameObject[] dungeonPrefabs;

    [Header("Selection")]
    [Tooltip("Maximum number of dungeon prefabs that can be selected from (count starts at 1). Clamped to available prefabs.")]
    public int maxSelectable = 3;

    [Tooltip("If true, instantiated prefabs will be parented under this GameObject.")]
    public bool parentToThis = true;

    [Tooltip("If true, clear previously instantiated dungeon when loading a new one.")]
    public bool clearPrevious = true;

    // Keep reference to the currently instantiated dungeon so we can remove it later
    GameObject currentDungeonInstance;

    void Start()
    {
        LoadRandomDungeon();
    }

    /// <summary>
    /// Loads a random dungeon prefab using inspector-assigned prefabs.
    /// </summary>
    public void LoadRandomDungeon()
    {
        int available = dungeonPrefabs?.Length ?? 0;
        if (available == 0)
        {
            Debug.LogWarning("DungeonDecider: No dungeon prefabs assigned.");
            return;
        }

        int clampedMax = Mathf.Clamp(maxSelectable, 1, available);
        int index = Random.Range(0, clampedMax); // zero-based
        LoadDungeon(index);
    }

    /// <summary>
    /// Loads the dungeon prefab at the specific index (0-based).
    /// </summary>
    public void LoadDungeon(int index)
    {
        int available = dungeonPrefabs?.Length ?? 0;
        if (index < 0 || index >= available)
        {
            Debug.LogWarning($"DungeonDecider: Index {index} is out of range (available: {available}).");
            return;
        }

        if (clearPrevious)
            ClearCurrent();

        var prefab = dungeonPrefabs[index];
        if (prefab == null)
        {
            Debug.LogWarning($"DungeonDecider: Dungeon prefab at index {index} is null.");
            return;
        }

        Transform parent = parentToThis ? transform : null;
        currentDungeonInstance = Instantiate(prefab, parent);
        currentDungeonInstance.name = GetInstanceName(prefab.name, index);
    }

    string GetInstanceName(string baseName, int index)
    {
        return $"{baseName}#{index}-Dungeon";
    }

    /// <summary>
    /// Destroys currently instantiated dungeon (if any).
    /// </summary>
    public void ClearCurrent()
    {
        if (currentDungeonInstance != null)
        {
            Destroy(currentDungeonInstance);
            currentDungeonInstance = null;
        }
    }

    // Handy editor/testing method: load a new random dungeon at runtime
    [ContextMenu("Load Random Dungeon")] 
    void ContextLoadRandom()
    {
        LoadRandomDungeon();
    }
}
