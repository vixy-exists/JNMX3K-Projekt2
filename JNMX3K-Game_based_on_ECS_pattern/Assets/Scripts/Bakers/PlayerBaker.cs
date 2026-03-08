using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerBaker : MonoBehaviour
{
    // Expose initial grid position override in inspector
    public bool overrideStartGrid = false;
    public int startX = 0;
    public int startY = 0;
}

// Baker converts the authoring GameObject into an entity and adds the required components.
public class PlayerBakerBaker : Baker<PlayerBaker>
{
    public override void Bake(PlayerBaker authoring)
    {
        // Request a dynamic entity for this GameObject (so transform can change at runtime)
        var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        // Add a tag to mark this entity as the player
        AddComponent(entity, new PlayerTag());

        // Determine initial grid position from authoring transform (or inspector override)
        int gx, gy;
        if (authoring.overrideStartGrid)
        {
            gx = authoring.startX;
            gy = authoring.startY;
        }
        else
        {
            var p = authoring.transform.position;
            gx = Mathf.RoundToInt(p.x);
            gy = Mathf.RoundToInt(p.y);
        }

        // Add GridPosition component
        AddComponent(entity, new GridPosition { x = gx, y = gy });

        // Add empty MoveIntent component (no movement initially)
        AddComponent(entity, new MoveIntent { DirectionX = 0, DirectionY = 0 });
    }
}
