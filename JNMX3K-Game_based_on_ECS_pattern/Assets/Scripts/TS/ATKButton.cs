using Unity.Entities;
using UnityEngine;

public class ATKButton : MonoBehaviour
{
    [Tooltip("Skill power to apply to the player when this button is pressed.")]
    public int playerSkillPower = 1;

    public void OnClick()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.LogWarning("ATKButton: No Default World available.");
            return;
        }

        var em = world.EntityManager;

        // Create a one-shot ActionRequest entity. The system will process and destroy it.
        var archetype = em.CreateArchetype(typeof(ActionRequest));
        var req = em.CreateEntity(archetype);
        em.SetComponentData(req, new ActionRequest { ShouldExecute = true, PlayerSkillPower = playerSkillPower });
    }
}
