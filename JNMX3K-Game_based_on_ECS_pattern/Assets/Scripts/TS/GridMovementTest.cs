/*using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial struct MovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var wallPositions = new NativeHashSet<int2>(100, Allocator.Temp);

        // Összes enemy pozíció begyűjtése
        foreach (var pos in SystemAPI.Query<RefRO<GridPosition>>()
                                     .WithAll<EnemyTag>())
        {
            wallPositions.Add(pos.ValueRO.Value);
        }

        // Mozgás csak ha nincs fal
        foreach (var (pos, entity) in SystemAPI
                     .Query<RefRW<GridPosition>>()
                     .WithAll<Player>()
                     .WithEntityAccess())
        {
            int2 target = pos.ValueRO.Value + new int2(1, 0); // pl. jobbra lép

            if (!wallPositions.Contains(target))
            {
                pos.ValueRW.Value = target;
            }
        }

        wallPositions.Dispose();
    }
}*/