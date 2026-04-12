using Unity.Entities;
using Unity.Mathematics;

public struct GridMoveInput : IComponentData
{
    public int2 Direction; // pl. (1,0), (-1,0), (0,1), (0,-1)
}