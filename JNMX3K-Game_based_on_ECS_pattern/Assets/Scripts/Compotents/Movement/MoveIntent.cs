using Unity.Entities;

public struct MoveIntent : IComponentData
{
    // Direction as integer offset on the grid
    public int DirectionX;
    public int DirectionY;
}
