using Unity.Entities;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    [Tooltip("Skill power to apply to the player when this skill button is pressed.")]
    public int skillPower = 3;

    public void OnClick()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("SkillButton: No Default World available.");
            return;
        }

        var em = world.EntityManager;
        var archetype = em.CreateArchetype(typeof(ActionRequest));
        var req = em.CreateEntity(archetype);
        em.SetComponentData(req, new ActionRequest { ShouldExecute = true, PlayerSkillPower = skillPower });
    }
}
