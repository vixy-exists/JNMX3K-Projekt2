using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[BurstCompile]
public partial struct InitialSpawnSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NoSpawnData>();
        _random = new Random(1234567);
    }

    public void OnUpdate(ref SystemState state)
    {
        // Csak egyszer fusson
        state.Enabled = false;

        var noSpawn = SystemAPI.GetSingleton<NoSpawnData>();
        ref var blob = ref noSpawn.Forbidden.Value;

        // Tiltott cellák HashSet-be
        var forbidden = new NativeHashSet<int2>(blob.Cells.Length, Allocator.Temp);
        for (int i = 0; i < blob.Cells.Length; i++)
            forbidden.Add(blob.Cells[i]);

        // Tilemap bounds
        int2 min = noSpawn.Bounds.Min;
        int2 max = noSpawn.Bounds.Max;

        // Minden EnemyTag és ItemTag entitás
        foreach (var (transform, entity) in
            SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAny<EnemyTag, ItemTag>()
            .WithEntityAccess())
        {
            int2 cell;

            // Addig keresünk random cellát, amíg nem tiltott
            do
            {
                cell = new int2(
                    _random.NextInt(min.x, max.x),
                    _random.NextInt(min.y, max.y)
                );
            }
            while (forbidden.Contains(cell));

            // Cell → világpozíció
            float3 worldPos = new float3(cell.x + 0.5f, cell.y + 0.5f, 0);

            transform.ValueRW.Position = worldPos;
        }

        forbidden.Dispose();
    }
}