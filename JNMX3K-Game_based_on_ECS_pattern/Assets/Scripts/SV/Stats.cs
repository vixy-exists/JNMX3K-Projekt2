using Unity.Entities;

public struct Stats : IComponentData
{
    public int Level;
    public int MaxHP;
    public int CurrentHP;
    public int MaxMP;
    public int CurrentMP;
    public int Strength;
    public int Speed;
    public int Intelligence;
    public int Defense;
}