using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct IntRange
{
    public int min;
    public int max;
}

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;
}

public class PlayerAuthoring : MonoBehaviour
{
    [Header("Stat randomization ranges")]
    public IntRange levelRange = new IntRange { min = 1, max = 1 };
    public IntRange hpRange = new IntRange { min = 10, max = 20 };
    public IntRange mpRange = new IntRange { min = 0, max = 5 };
    public IntRange strengthRange = new IntRange { min = 1, max = 5 };
    public IntRange speedRange = new IntRange { min = 1, max = 5 };
    public IntRange intRange = new IntRange { min = 1, max = 5 };
    public IntRange defenseRange = new IntRange { min = 0, max = 5 };

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Player>(entity);
            AddComponent(entity, new GridMoveRe { IsMoving = false });
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

            stats.CurrentHP = stats.MaxHP;
            stats.CurrentMP = stats.MaxMP;

            AddComponent(entity, stats);
        }
    }
}
