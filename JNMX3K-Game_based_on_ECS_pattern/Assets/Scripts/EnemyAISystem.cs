using Unity.Burst;
using Unity.Entities;

partial struct EnemyAISystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        // Note: this method uses UnityEngine.Random (managed API) so it shouldn't be Burst-compiled.
        foreach (var intent in SystemAPI.Query<RefRW<MoveIntent>>().WithAll<EnemyTag>())
        {
            int r = UnityEngine.Random.Range(0, 4);
            switch (r)
            {
                case 0:
                    intent.ValueRW.DirectionX = 1; intent.ValueRW.DirectionY = 0; break;
                case 1:
                    intent.ValueRW.DirectionX = -1; intent.ValueRW.DirectionY = 0; break;
                case 2:
                    intent.ValueRW.DirectionX = 0; intent.ValueRW.DirectionY = 1; break;
                default:
                    intent.ValueRW.DirectionX = 0; intent.ValueRW.DirectionY = -1; break;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
