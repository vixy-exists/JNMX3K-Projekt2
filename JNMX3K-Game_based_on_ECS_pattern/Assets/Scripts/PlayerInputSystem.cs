using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlayerInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        int dx = 0;
        int dy = 0;

        if (Input.GetKeyDown(KeyCode.W)) dy = 1;
        if (Input.GetKeyDown(KeyCode.S)) dy = -1;
        if (Input.GetKeyDown(KeyCode.A)) dx = -1;
        if (Input.GetKeyDown(KeyCode.D)) dx = 1;

        if (dx == 0 && dy == 0)
            return;

        foreach (var intent in SystemAPI.Query<RefRW<MoveIntent>>().WithAll<PlayerTag>())
        {
            intent.ValueRW.DirectionX = dx;
            intent.ValueRW.DirectionY = dy;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
}
