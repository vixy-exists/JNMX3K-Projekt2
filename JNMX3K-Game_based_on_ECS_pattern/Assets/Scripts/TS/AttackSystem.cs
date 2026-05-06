using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

// System that processes ActionRequest entities: for each request it applies damage once to one enemy (random skillPower 1-4)
// and once to the player using the PlayerSkillPower passed in the request. After processing the request entity is destroyed.
public partial struct AttackSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // no specific requirements
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
            return;

        var em = world.EntityManager;

        var reqQuery = em.CreateEntityQuery(ComponentType.ReadWrite<ActionRequest>());
        if (reqQuery.IsEmptyIgnoreFilter)
            return;

        using (var reqEntities = reqQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var reqEntity in reqEntities)
            {
                var req = em.GetComponentData<ActionRequest>(reqEntity);
                if (!req.ShouldExecute)
                {
                    em.DestroyEntity(reqEntity);
                    continue;
                }

                // 1) Apply damage to one enemy (if any) with random skillPower 1-4
                var enemyQuery = em.CreateEntityQuery(ComponentType.ReadWrite<Stats>(), ComponentType.ReadOnly<EnemyTag>());
                if (!enemyQuery.IsEmptyIgnoreFilter)
                {
                    using (var enemies = enemyQuery.ToEntityArray(Allocator.Temp))
                    {
                        var targetEnemy = enemies[0];
                        var enemyStats = em.GetComponentData<Stats>(targetEnemy);

                        int enemySkillPower = UnityEngine.Random.Range(1, 5); // 1..4

                        int newEnemyHP = DamageCalculator.CalculateNewHP(enemyStats.CurrentHP, enemyStats.Strength, enemyStats.Speed, enemyStats.Defense, enemySkillPower);
                        enemyStats.CurrentHP = Math.Max(0, Math.Min(enemyStats.MaxHP, newEnemyHP));
                        em.SetComponentData(targetEnemy, enemyStats);

                        Debug.Log($"AttackSystem: Damaged enemy {targetEnemy.Index} with skillPower {enemySkillPower}. NewHP={enemyStats.CurrentHP}");
                    }
                }
                else
                {
                    Debug.Log("AttackSystem: No enemy found to damage.");
                }

                // 2) Apply damage to the player (if any) using req.PlayerSkillPower
                var playerQuery = em.CreateEntityQuery(ComponentType.ReadWrite<Stats>(), ComponentType.ReadOnly<Player>());
                if (!playerQuery.IsEmptyIgnoreFilter)
                {
                    using (var players = playerQuery.ToEntityArray(Allocator.Temp))
                    {
                        var targetPlayer = players[0];
                        var playerStats = em.GetComponentData<Stats>(targetPlayer);

                        int playerSkillPower = req.PlayerSkillPower;
                        if (playerSkillPower <= 0) playerSkillPower = 1; // fallback

                        int newPlayerHP = DamageCalculator.CalculateNewHP(playerStats.CurrentHP, playerStats.Strength, playerStats.Speed, playerStats.Defense, playerSkillPower);
                        playerStats.CurrentHP = Math.Max(0, Math.Min(playerStats.MaxHP, newPlayerHP));
                        em.SetComponentData(targetPlayer, playerStats);

                        Debug.Log($"AttackSystem: Damaged player {targetPlayer.Index} with skillPower {playerSkillPower}. NewHP={playerStats.CurrentHP}");
                    }
                }
                else
                {
                    Debug.Log("AttackSystem: No player found to damage.");
                }

                // Destroy the request entity after processing
                em.DestroyEntity(reqEntity);
            }
        }
    }
}
