using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAuthoring : MonoBehaviour
{
    [Tooltip("If true, use the specified startX/startY instead of the GameObject transform position.")]
    public bool overrideStartGrid = false;
    public int startX = 0;
    public int startY = 0;

    [Tooltip("If true, create a LocalTransform component so spawn systems can place this entity in the world.")]
    public bool addLocalTransform = true;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Tag as enemy
            AddComponent<EnemyTag>(entity);

            /*/ Initial grid position from authoring transform or explicit override
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
            }*/

            AddComponent(entity, new GridMoveRe { IsMoving = false });

            /*/ Optionally add LocalTransform so systems that set/animate transforms can act on it
            if (authoring.addLocalTransform)
            {
                float3 worldPos = new float3(gx + 0.5f, gy + 0.5f, authoring.transform.position.z);
                AddComponent(entity, new LocalTransform { Position = worldPos });
            }*/
        }
    }
}
