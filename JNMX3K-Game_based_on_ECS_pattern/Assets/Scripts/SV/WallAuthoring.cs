using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WallAuthoring : MonoBehaviour
{
    public class Baker : Baker<WallAuthoring>
    {
        public override void Bake(WallAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<WallTag>(entity);
        }
    }
}