/*using Unity.Entities;
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
}*/