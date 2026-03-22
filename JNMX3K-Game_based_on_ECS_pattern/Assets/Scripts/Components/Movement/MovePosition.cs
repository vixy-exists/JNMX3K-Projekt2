using Unity.Entities;
using Unity.Mathematics;

// Stores the world position for entities that are moved by grid systems.
public struct MovePosition : IComponentData
{
    public float3 Value;
}
