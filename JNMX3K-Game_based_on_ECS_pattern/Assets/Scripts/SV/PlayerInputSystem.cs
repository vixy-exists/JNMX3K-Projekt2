using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerInputSystemRe : SystemBase
{
    protected override void OnUpdate()
    {
        float3 move = float3.zero;

        var kb = Keyboard.current;
        if (kb == null)
        {
            // No keyboard available via the new Input System; nothing to do.
            return;
        }

        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) move = new float3(0, 8, 0);
        if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) move = new float3(0, -8, 0);
        if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) move = new float3(-8, 0, 0);
        if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) move = new float3(8, 0, 0);

        if (math.all(move == float3.zero))
            return;

        Entities
            .WithAll<Player>()
            .ForEach((ref GridMoveRe gridMove, in LocalTransform transform) =>
            {
                if (!gridMove.IsMoving)
                {
                    gridMove.TargetPosition = transform.Position + move;
                    gridMove.IsMoving = true;
                }
            }).Schedule();
    }
}