using Unity.Entities;
using Unity.Mathematics;

public struct Player : IComponentData { }

public struct GridMoveRe : IComponentData
{
    public float3 TargetPosition;
    public bool IsMoving;
}
