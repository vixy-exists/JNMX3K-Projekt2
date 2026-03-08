using Unity.Entities;

public struct GridPosition : IComponentData
{
    // Grid position stored as two integers to avoid Unity.Mathematics types
    public int x;
    public int y;
}
