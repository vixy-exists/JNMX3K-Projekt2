using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// ECS-oriented GridMover component (pure data)
public struct GridMover : IComponentData
{
    public float3 MovePoint;
    public float3 CurrentPosition;
    public float MoveSpeed;
    public float SnapDistance;
}

// Authoring MonoBehaviour to add GridMover data during conversion
[DisallowMultipleComponent]
public class GridMoverAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float snapDistance = 0.05f;
}

// Baker for converting the MonoBehaviour authoring component into an ECS component at conversion time
public class GridMoverBaker : Baker<GridMoverAuthoring>
{
    public override void Bake(GridMoverAuthoring authoring)
    {
        var data = new GridMover
        {
            MovePoint = authoring.transform.position,
            CurrentPosition = authoring.transform.position,
            MoveSpeed = authoring.moveSpeed,
            SnapDistance = authoring.snapDistance
        };

        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        AddComponent(entity, data);
    }
}

partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Nothing to initialize for now
    }

    // Read Input from managed API; do not Burst this method
    public void OnUpdate(ref SystemState state)
    {
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        foreach (var gridMoverRef in SystemAPI.Query<RefRW<GridMover>>())
        {
            var mover = gridMoverRef.ValueRW;

            float3 pos = mover.CurrentPosition;
            float3 target = mover.MovePoint;

            // Smoothly move towards the MovePoint
            float step = mover.MoveSpeed * Time.deltaTime;
            float3 toTarget = target - pos;
            float dist = math.length(toTarget);
            if (dist > 0f)
            {
                if (dist <= step)
                    pos = target;
                else
                    pos += (toTarget / dist) * step;

                mover.CurrentPosition = pos;
            }

            if (math.distance(pos, mover.MovePoint) <= mover.SnapDistance)
            {
                if (math.abs(horiz) == 1f)
                {
                    mover.MovePoint += new float3(math.sign(horiz), 0f, 0f);
                }
                else if (math.abs(vert) == 1f)
                {
                    mover.MovePoint += new float3(0f, math.sign(vert), 0f);
                }

                gridMoverRef.ValueRW = mover;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Nothing to clean up for now
    }
}
