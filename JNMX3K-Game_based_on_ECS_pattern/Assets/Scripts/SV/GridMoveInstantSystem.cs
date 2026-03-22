using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Tilemaps;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

public partial class GridMoveInstantSystem : SystemBase
{
    Tilemap _wallTilemap;
    float _tileSize = 1f;

    protected override void OnCreate()
    {
        base.OnCreate();
        // Try to find a Tilemap named with "wall" or the first available Tilemap
        var maps = UnityEngine.Object.FindObjectsOfType<Tilemap>();
        foreach (var m in maps)
        {
            Debug.Log("TTEST maps entered");
            if (m != null && m.gameObject != null && m.gameObject.name.ToLower().Contains("wall"))
            {
                _wallTilemap = m;
                Debug.Log("TTest tilemap found");
                break;
            }
        }

        if (_wallTilemap == null && maps.Length > 0)
            _wallTilemap = maps[0];

        if (_wallTilemap == null)
            Debug.LogWarning("GridMoveInstantSystem: No Tilemap found for wall checks. CanStep will always allow movement.");
    }

    bool CanStep(Vector2 dir, float3 currentPos)
    {
        if (_wallTilemap == null)
            return true;

        Vector3 worldPos = new Vector3(currentPos.x, currentPos.y, currentPos.z);
        Vector3 checkPos = worldPos + (Vector3)(dir * _tileSize);
        Vector3Int cellPos = _wallTilemap.WorldToCell(checkPos);
        TileBase tile = _wallTilemap.GetTile(cellPos);
        return tile == null; // if no tile, it's walkable
    }

    protected override void OnUpdate()
    {
        // Process movers on the main thread and prevent stepping into walls using Tilemap checks
        Entities.ForEach((ref LocalTransform transform, ref GridMoveRe gridMove) =>
        {
            if (!gridMove.IsMoving)
                return;

            var dirf = gridMove.TargetPosition - transform.Position;
            Vector2 dir2 = new Vector2(dirf.x, dirf.y);
            if (dir2.sqrMagnitude < 0.0001f)
            {
                gridMove.IsMoving = false;
                return;
            }

            dir2 = dir2.normalized;

            bool canStep = CanStep(dir2, transform.Position);

            if (canStep)
            {
                transform.Position = gridMove.TargetPosition;
            }

            gridMove.IsMoving = false;
        }).WithoutBurst().Run();
    }
}


/*
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class GridMoveInstantSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref LocalTransform transform, ref GridMoveRe gridMove) =>
        {
            if (gridMove.IsMoving)
            {
                transform.Position = gridMove.TargetPosition;
                gridMove.IsMoving = false;
            }
        }).Schedule();
    }
}
 */