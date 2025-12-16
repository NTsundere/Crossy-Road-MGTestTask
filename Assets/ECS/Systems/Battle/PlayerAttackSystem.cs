using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsFilter<RhythmHitResultEvent> _attackEventFilter;
    private EcsFilter<PlayerComponent, AttackComponent> _playerFilter;
    private EcsFilter<EnemyComponent, HealthComponent> _enemyFilter;

    private EcsWorld _world;

    private SharedData _data;
    private Image _enemyHealthBar;

    private bool _isWin = false;

    public void Init()
    {
        _enemyHealthBar = _data.EnemyHealth.GetComponentInChildren<Image>();
    }   

    public void Run()
    {
        if (_attackEventFilter.IsEmpty() || _enemyFilter.IsEmpty()) return;

        ref var attackEvent = ref _attackEventFilter.Get1(0);

        EcsEntity playerEntity = _playerFilter.GetEntity(0);
        ref var playerAttackComponent = ref _playerFilter.Get2(0);
        ref var playerPositionComponent = ref playerEntity.Get<PositionComponent>();

        foreach (var i in _enemyFilter)
        {
            ref var enemyHealthComponent = ref _enemyFilter.Get2(i);
            if (enemyHealthComponent.health > 0)
            {
                enemyHealthComponent.health -= (int)((float)playerAttackComponent.BaseDamage * attackEvent.DamageMultiplier);
                _enemyHealthBar.fillAmount = (float)enemyHealthComponent.health / enemyHealthComponent.maxHealth;
            }
            else if (enemyHealthComponent.health <= 0)
            {
                var enemyEntity = _enemyFilter.GetEntity(i);
                ref var deathEvent = ref enemyEntity.Get<DeathEvent>();
                deathEvent.killerEntity = playerEntity;
                deathEvent.killedEntity = enemyEntity;
                EndBattle(playerEntity, playerPositionComponent);
            }
        }
    }

    private void EndBattle(EcsEntity playerEntity, PositionComponent playerPositionComponent)
    {
        if (_isWin == false)
        {
            playerEntity.Del<RhythmCombatComponent>();
            _data.PlayerHealth.SetActive(false);
            _data.EnemyHealth.SetActive(false);
            PuzzleActions.OnBattleEnded();

            var entity = _world.NewEntity();
            ref var chestSpawnComponent = ref entity.Get<ChestSpawnRequestComponent>();
            chestSpawnComponent.Position = playerPositionComponent.position + new Vector3(0f, -0.5f, 5f);

            ref var state = ref playerEntity.Get<PlayerStateComponent>();
            state.currentState = PlayerStateComponent.GameState.Ready;

            _isWin = true;
        }
    }
}