using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsFilter<EnemyComponent, AttackComponent> _enemyFilter;
    private EcsFilter<PlayerComponent, HealthComponent> _playerFilter;

    private SharedData _data;
    private Image _playerHealthBar;

    public void Init()
    {
        _playerHealthBar = _data.PlayerHealth.GetComponentInChildren<Image>();
    }   

    public void Run()
    {
        if (_playerFilter.IsEmpty() || _enemyFilter.IsEmpty()) return;

        var playerEntity = _playerFilter.GetEntity(0);
        ref var playerHealthComponent = ref _playerFilter.Get2(0);

        foreach (var i in _enemyFilter)
        {
            ref var enemyAttackComponent = ref _enemyFilter.Get2(i);

            //foreach (var j in _playerFilter)
            //{
                //ref var playerHealthComponent = ref _playerFilter.Get2(j);

                if (enemyAttackComponent.CurrentAttackTimer > 0 && playerHealthComponent.health > 0)
                {
                    enemyAttackComponent.CurrentAttackTimer -= Time.deltaTime;
                }
                else if (playerHealthComponent.health <= 0)
                {
                    ref var deathEvent = ref playerEntity.Get<DeathEvent>();
                    deathEvent.killerEntity = _enemyFilter.GetEntity(i);
                    deathEvent.killedEntity = playerEntity;
                }
                else
                {
                    playerHealthComponent.health -= enemyAttackComponent.BaseDamage;
                    _playerHealthBar.fillAmount = (float)playerHealthComponent.health / playerHealthComponent.maxHealth;
                    enemyAttackComponent.CurrentAttackTimer = enemyAttackComponent.AttackInterval;
                }
            //}
        }
    }
}
