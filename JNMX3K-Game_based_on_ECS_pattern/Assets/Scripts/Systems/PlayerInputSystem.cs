using Unity.Burst;
using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlayerInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        int dx = 0;
        int dy = 0;


        // Use the new Input System only
        var kb = Keyboard.current;
        if (kb == null)
        {
            // No keyboard available via the new Input System; nothing to do.
            return;
        }

        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) dy = 1;
        if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) dy = -1;
        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) dx = -1;
        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) dx = 1;

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
