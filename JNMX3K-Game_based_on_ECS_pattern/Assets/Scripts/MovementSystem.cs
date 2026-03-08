using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var walls = SystemAPI.QueryBuilder().WithAll<WallTag, GridPosition>().Build();

        foreach (var (pos, intent) in SystemAPI.Query<RefRW<GridPosition>, RefRW<MoveIntent>>())
        {
            int targetX = pos.ValueRO.x + intent.ValueRO.DirectionX;
            int targetY = pos.ValueRO.y + intent.ValueRO.DirectionY;

            bool blocked = false;

            // TODO: check walls using walls query; left commented out earlier

            if (!blocked)
            {
                pos.ValueRW.x = targetX;
                pos.ValueRW.y = targetY;
            }

            intent.ValueRW.DirectionX = 0;
            intent.ValueRW.DirectionY = 0;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
