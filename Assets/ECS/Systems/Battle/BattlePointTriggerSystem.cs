using Leopotam.Ecs;
using UnityEngine;

public class BattlePointTriggerSystem : IEcsRunSystem
{
    private EcsFilter<PlayerComponent, PositionComponent> _playerFilter;
    private EcsFilter<BattlePointComponent> _battlePointFilter;

    private EcsWorld _world;
    private SharedData _data;
    
    public void Run()
    {
        EcsEntity playerEntity = _playerFilter.GetEntity(0);
        ref var playerPos = ref _playerFilter.Get2(0);

        foreach (var pointIdx in _battlePointFilter)
        {
            ref var battlePoint = ref _battlePointFilter.Get1(pointIdx);

            if (battlePoint.IsTriggered) continue;

            if (Vector3.Distance(playerPos.position, battlePoint.Position) <= battlePoint.ActivationRadius)
            {
                battlePoint.IsTriggered = true;
                _data.BattlePoint.SetActive(false);
                StartBossEncounter(battlePoint.Position);
                ref var state = ref playerEntity.Get<PlayerStateComponent>();
                state.currentState = PlayerStateComponent.GameState.Battle;
                break;
            }
        }
    }

    private void StartBossEncounter(Vector3 battlePointPos)
    {
        var encounterRequest = _world.NewEntity();
        ref var request = ref encounterRequest.Get<BossEncounterRequestComponent>();
        request.BattleCenter = battlePointPos;
    }
}