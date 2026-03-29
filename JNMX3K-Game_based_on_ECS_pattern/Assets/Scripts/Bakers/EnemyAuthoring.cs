using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Tooltip("If true, use the specified startX/startY instead of the GameObject transform position.")]
    public bool overrideStartGrid = false;
    public int startX = 0;
    public int startY = 0;

    [Tooltip("If true, create a LocalTransform component so spawn systems can place this entity in the world.")]
    public bool addLocalTransform = true;

    [Header("Stat randomization ranges")]
    public IntRange levelRange = new IntRange { min = 1, max = 1 };
    public IntRange hpRange = new IntRange { min = 10, max = 20 };
    public IntRange mpRange = new IntRange { min = 0, max = 5 };
    public IntRange strengthRange = new IntRange { min = 1, max = 5 };
    public IntRange speedRange = new IntRange { min = 1, max = 5 };
    public IntRange intRange = new IntRange { min = 1, max = 5 };
    public IntRange defenseRange = new IntRange { min = 0, max = 5 };

    [Header("Stat boost (Intelligence OR Strength)")]
    [Tooltip("If >0, a random bonus in this range will be applied to either Intelligence or Strength (exclusive).")]
    public IntRange boostRange = new IntRange { min = 1, max = 3 };
    [Tooltip("Probability (0..1) that a boost will be applied. If 1, always apply boost to either Intelligence or Strength.")]
    public float boostProbability = 1f;

    public class Baker : Baker<EnemyAuthoring>
    {
        
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Tag as enemy
            AddComponent<EnemyTag>(entity);
            AddComponent(entity, new GridMoveRe { IsMoving = false });
            // initial grid position (will be set by spawn systems)
            AddComponent(entity, new GridPosition { Value = new int2(0, 0) });

            // Randomize stats within ranges (inclusive for ints)
            var stats = new Stats
            {
                Level = UnityEngine.Random.Range(authoring.levelRange.min, authoring.levelRange.max + 1),
                MaxHP = UnityEngine.Random.Range(authoring.hpRange.min, authoring.hpRange.max + 1),
                CurrentHP = 0, // set after MaxHP assigned
                MaxMP = UnityEngine.Random.Range(authoring.mpRange.min, authoring.mpRange.max + 1),
                CurrentMP = 0, // set after MaxMP assigned
                Strength = UnityEngine.Random.Range(authoring.strengthRange.min, authoring.strengthRange.max + 1),
                Speed = UnityEngine.Random.Range(authoring.speedRange.min, authoring.speedRange.max + 1),
                Intelligence = UnityEngine.Random.Range(authoring.intRange.min, authoring.intRange.max + 1),
                Defense = UnityEngine.Random.Range(authoring.defenseRange.min, authoring.defenseRange.max)
            };

            // Optionally apply a boost to either Intelligence OR Strength (exclusive)
            if (UnityEngine.Random.value <= authoring.boostProbability)
            {
                int bonus = UnityEngine.Random.Range(authoring.boostRange.min, authoring.boostRange.max + 1);
                if (UnityEngine.Random.value < 0.5f)
                    stats.Intelligence += bonus;
                else
                    stats.Strength += bonus;
            }

            stats.CurrentHP = stats.MaxHP;
            stats.CurrentMP = stats.MaxMP;

            AddComponent(entity, stats);
        }
    }
}
