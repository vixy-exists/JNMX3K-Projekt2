using Unity.Entities;

public struct ActionRequest : IComponentData
{
    // When true the system should process and then the request entity will be destroyed
    public bool ShouldExecute;

    // Skill power value to use when applying damage to the player
    public int PlayerSkillPower;
}
