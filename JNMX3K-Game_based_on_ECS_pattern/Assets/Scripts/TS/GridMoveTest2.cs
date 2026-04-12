/*using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct GridMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        foreach (var (transform, collider, input) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<PhysicsCollider>, RefRO<GridMoveInput>>()
                 .WithAll<Player>())
        {
            int2 dir = input.ValueRO.Direction;

            if (math.all(dir == int2.zero))
                continue;

            float cellSize = 8f;
            float3 move = new float3(dir.x * cellSize, 0, dir.y * cellSize); // 2D XZ sík
            float3 start = transform.ValueRO.Position;
            float3 end = start + move;

            // ColliderCast
            var castInput = new ColliderCastInput
            {
                Collider = (Unity.Physics.Collider*)collider.ValueRO.Value.GetUnsafePtr(),
                Orientation = quaternion.identity,
                Start = start,
                End = end
            };

            bool hit = physicsWorld.CastCollider(castInput, out var hitInfo);

            if (!hit)
            {
                transform.ValueRW.Position = end;
            }
            // ha hit → nem lép (fal blokkol)
        }
    }
}*/