using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DungeonAuthoring : MonoBehaviour
{
    public class Baker : Baker<DungeonAuthoring>
    {
        public override void Bake(DungeonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<WallTag>(entity);
        }
    }
}