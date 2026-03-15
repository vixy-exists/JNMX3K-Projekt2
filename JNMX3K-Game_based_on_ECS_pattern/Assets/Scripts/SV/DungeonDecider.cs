using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections;

[System.Serializable]
public struct StartPositionEntry
{
    public int x;
    public int y;
}

public class DungeonDecider : MonoBehaviour
{
    [Header("Assign dungeon prefabs (each prefab contains both background and walls)")]
    [Tooltip("Prefabs for full dungeons.")]
    public GameObject[] dungeonPrefabs;

    [Header("Player start position")]
    [Tooltip("If true, look for a DungeonStartPoint component inside the instantiated prefab to set player start position. Otherwise use the manual Start Positions array.")]
    public bool usePrefabStartPoint = true;

    [Tooltip("Manual start positions per dungeon prefab. Used when usePrefabStartPoint is false. Length should match or exceed dungeonPrefabs length.")]
    public StartPositionEntry[] startPositions;

    [Tooltip("If true, manual start positions are relative to the dungeon prefab origin. Otherwise they are absolute grid coordinates.")]
    public bool startPositionsAreRelative = true;

    [Header("Retry settings")]
    [Tooltip("How many times to retry setting the player GridPosition if the player entity is not yet available.")]
    public int retryAttempts = 10;

    [Tooltip("Delay in seconds between retry attempts.")]
    public float retryDelay = 0.1f;

    [Tooltip("If true, instantiated prefabs will be parented under this GameObject.")]
    public bool parentToThis = true;

    [Tooltip("If true, clear previously instantiated dungeon when loading a new one.")]
    public bool clearPrevious = true;

    // Keep reference to the currently instantiated dungeon so we can remove it later
    GameObject currentDungeonInstance;
    // Keep reference to the currently running coroutine so we can cancel when loading a new dungeon
    Coroutine setPositionCoroutine;

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

        int clampedMax = Mathf.Clamp(dungeonPrefabs.Length, 1, available);
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

        var scriptTEST = currentDungeonInstance.GetComponentInChildren<DungeonStartPoint>();

        // After instantiating the dungeon, set the player's GridPosition according to configuration
        int absoluteX = 0;
        int absoluteY = 0;
        bool haveStart = false;

        if (usePrefabStartPoint)
        {
            Debug.LogWarning("TEST usePrefabStartPoint.");
            // If prefab contains multiple start points, prefer one that matches the dungeon index/name, then primary, then first
            var startComps = currentDungeonInstance.GetComponentsInChildren<DungeonStartPoint>(true);
            DungeonStartPoint startComp = null;
            if (startComps.Length > 0)
            {
                Debug.LogWarning("TEST.");
                // 1) prefer by explicit dungeonIndex
                foreach (var s in startComps)
                {
                    Debug.LogWarning("TEST explicit dungeonIndex.");
                    Debug.LogWarning("TEST Index: " + index);
                    Debug.LogWarning("TEST dungeon index: " + s.dungeonIndex);
                    if (s != null && s.dungeonIndex == index)
                    {
                        Debug.LogWarning("TEST startcomp-ban van cucc");
                        startComp = s;
                        break;
                    }
                }
                /*
                // 2) prefer by dungeonName match with prefab name
                if (startComp == null)
                {
                    Debug.LogWarning("dungeonName match with prefab name.");
                    foreach (var s in startComps)
                    {
                        if (s != null && !string.IsNullOrEmpty(s.dungeonName) && s.dungeonName == prefab.name)
                        {
                            startComp = s;
                            break;
                        }
                    }
                }

                // 3) prefer explicitly marked primary
                if (startComp == null)
                {
                    Debug.LogWarning("explicitly marked primary.");
                    foreach (var s in startComps)
                    {
                        if (s != null && s.isPrimary)
                        {
                            startComp = s;
                            break;
                        }
                    }
                }

                // 4) fallback to first
                if (startComp == null)
                { Debug.LogWarning("fallback."); startComp = startComps[0]; }*/
            }

            if (startComp != null)
            {
                Debug.LogWarning("TEST ha startComp-ba rakott valamit");

                // If start is relative, compute absolute by adding dungeon origin (rounded) to offset
                if (startComp.isRelative)
                {
                    Debug.LogWarning("TEST startComp is relative.");
                    var origin = currentDungeonInstance.transform.position;
                    absoluteX = Mathf.RoundToInt(origin.x) + startComp.startX;
                    absoluteY = Mathf.RoundToInt(origin.y) + startComp.startY;
                }
                else
                {
                    Debug.LogWarning("TEST startcomp not relative");
                    absoluteX = startComp.startX;
                    absoluteY = startComp.startY;
                }
                Debug.LogWarning("x and y: " + absoluteX + " + " + absoluteY);
                Debug.LogWarning("TEST haveStart az már igaz");
                haveStart = true;
            }
            else
            {
                Debug.LogWarning("DungeonDecider: usePrefabStartPoint is true but no DungeonStartPoint found in prefab.");
            }
        }
        else
        {
            if (startPositions != null && index < startPositions.Length)
            {
                Debug.LogWarning("bruh else");
                var sp = startPositions[index];
                if (startPositionsAreRelative)
                {
                    var origin = currentDungeonInstance.transform.position;
                    absoluteX = Mathf.RoundToInt(origin.x) + sp.x;
                    absoluteY = Mathf.RoundToInt(origin.y) + sp.y;
                }
                else
                {
                    absoluteX = sp.x;
                    absoluteY = sp.y;
                }
                haveStart = true;
            }
            else
            {
                Debug.LogWarning("DungeonDecider: No manual start position found for this dungeon index.");
            }
        }

        SetPlayerGridPositionImmediate(absoluteX, absoluteY);

        if (haveStart)
        {
            // Ensure a player entity exists and set its GridPosition (create entity if necessary)
            //EnsurePlayerEntityAt(absoluteX, absoluteY);
        }
    }

    string GetInstanceName(string baseName, int index)
    {
        return $"{baseName}#{index}-Dungeon";
    }

    void SetPlayerGridPositionImmediate(int x, int y)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("DungeonDecider: No Default World available to set player GridPosition.");
            return;
        }

        var em = world.EntityManager;
        var query = em.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(GridPosition), typeof(Player) }
        });

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            if (entities.Length == 0)
            {
                Debug.LogWarning("DungeonDecider: No player entity found to set GridPosition.");
            }
            else
            {
                foreach (var e in entities)
                {
                    em.SetComponentData(e, new GridPosition { x = x, y = y });
                }
            }
        }
    }

    // Returns true if player entity was found and position set
    bool SetPlayerGridPosition(int x, int y)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
            return false;

        var em = world.EntityManager;
        var query = em.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(GridPosition), typeof(Player) }
        });

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            if (entities.Length == 0)
            {
                // No ECS player entity found — try GameObject fallbacks
                var goByTag = GameObject.FindWithTag("Player");
                if (goByTag != null)
                {
                    goByTag.transform.position = new Vector3(x, y, goByTag.transform.position.z);
                    Debug.Log($"DungeonDecider: Set GameObject Player transform to ({x},{y}).");
                    return true;
                }

    /// <summary>
    /// Ensure a player entity exists and set its GridPosition. If no entity exists, create one.
    /// </summary>
    void EnsurePlayerEntityAt(int x, int y)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("DungeonDecider: No Default World available to ensure player entity.");
            return;
        }

        var em = world.EntityManager;
        var query = em.CreateEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Player) }
        });

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            if (entities.Length == 0)
            {
                // Create a new player entity with GridPosition and MoveIntent
                var entity = em.CreateEntity(typeof(Player), typeof(GridPosition), typeof(MoveIntent));
                em.SetComponentData(entity, new GridPosition { x = x, y = y });
                em.SetComponentData(entity, new MoveIntent { DirectionX = 0, DirectionY = 0 });
                Debug.Log($"DungeonDecider: Created player entity at ({x},{y}).");
                return;
            }

            // Set GridPosition for existing player entities
            foreach (var e in entities)
            {
                if (em.HasComponent<GridPosition>(e))
                    em.SetComponentData(e, new GridPosition { x = x, y = y });
                else
                    em.AddComponentData(e, new GridPosition { x = x, y = y });

                if (!em.HasComponent<MoveIntent>(e))
                    em.AddComponentData(e, new MoveIntent { DirectionX = 0, DirectionY = 0 });
            }

            Debug.Log($"DungeonDecider: Set existing player entity(ies) GridPosition to ({x},{y}).");
        }
    }

                var bakerAuthoring = FindObjectOfType<PlayerAuthoring>();
                if (bakerAuthoring != null)
                {
                    bakerAuthoring.transform.position = new Vector3(x, y, bakerAuthoring.transform.position.z);
                    Debug.Log($"DungeonDecider: Set PlayerBaker GameObject transform to ({x},{y}).");
                    return true;
                }

                return false;
            }

            foreach (var e in entities)
            {
                em.SetComponentData(e, new GridPosition { x = x, y = y });
            }
        }

        Debug.Log($"DungeonDecider: Set ECS Player GridPosition to ({x},{y}).");

        return true;
    }

    IEnumerator TrySetPlayerGridPositionWithRetry(int x, int y)
    {
        int attempts = 0;
        while (attempts < retryAttempts)
        {
            if (SetPlayerGridPosition(x, y))
                yield break;

            attempts++;
            yield return new WaitForSeconds(retryDelay);
        }

        Debug.LogWarning("DungeonDecider: Failed to set player GridPosition after retries.");
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
