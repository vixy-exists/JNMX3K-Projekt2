using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct NoSpawnBounds
{
    public int2 Min;
    public int2 Max;
}

public struct NoSpawnData : IComponentData
{
    public BlobAssetReference<NoSpawnBlob> Forbidden;
    public NoSpawnBounds Bounds;
}

public class NoSpawnTilemapAuthoring : MonoBehaviour
{
    public class Baker : Baker<NoSpawnTilemapAuthoring>
    {
        public override void Bake(NoSpawnTilemapAuthoring authoring)
        {
            var tilemap = authoring.GetComponent<Tilemap>();
            var bounds = tilemap.cellBounds;

            var cells = new NativeList<int2>(Allocator.Temp);

            foreach (var pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                    cells.Add(new int2(pos.x, pos.y));
            }

            // BlobAsset
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<NoSpawnBlob>();
            var array = builder.Allocate(ref root.Cells, cells.Length);

            for (int i = 0; i < cells.Length; i++)
                array[i] = cells[i];

            var blobRef = builder.CreateBlobAssetReference<NoSpawnBlob>(Allocator.Persistent);
            builder.Dispose();

            AddBlobAsset(ref blobRef, out _);

            // Bounds mentése
            AddComponent(new NoSpawnData
            {
                Forbidden = blobRef,
                Bounds = new NoSpawnBounds
                {
                    Min = new int2(bounds.xMin, bounds.yMin),
                    Max = new int2(bounds.xMax, bounds.yMax)
                }
            });
        }

    }
}

public struct NoSpawnBlob
{
    public BlobArray<int2> Cells;
}
