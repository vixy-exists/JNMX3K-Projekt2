using UnityEngine;

// Attach this to a child GameObject inside a dungeon prefab to mark player start
public class DungeonStartPoint : MonoBehaviour
{
    public int startX = 0;
    public int startY = 0;
    [Tooltip("If true, startX/startY are treated as offsets relative to the dungeon prefab origin. If false, they are absolute grid coordinates.")]
    public bool isRelative = true;
    [Tooltip("If true this start point will be preferred when multiple start points exist in the prefab.")]
    public bool isPrimary = false;
    [Tooltip("Optional: index of the dungeon prefab this start point belongs to. Set to -1 to ignore.")]
    public int dungeonIndex = -1;
    [Tooltip("Optional: name of the dungeon prefab this start point belongs to. Leave empty to ignore.")]
    public string dungeonName = "";
}
