using Leopotam.Ecs;
using UnityEngine;

public class ChestTriggerSystem : IEcsRunSystem
{
    private EcsFilter<ChestInteractionComponent, PositionComponent> _chestTriggerFilter;
    private EcsFilter<PlayerComponent, PositionComponent> _playerFilter;
    private EcsWorld _world;

    private bool _isInitialized = false;

    public void Run()
    {
        var playerEntity = _playerFilter.GetEntity(0);
        ref var playerPositionComponent = ref _playerFilter.Get2(0);

        if (_isInitialized == false)
        {
            foreach (var i in _chestTriggerFilter)
            {
                ref var chestInteractionComponent = ref _chestTriggerFilter.Get1(i);
                ref var chestPositionComponent = ref _chestTriggerFilter.Get2(i);

                if (chestInteractionComponent.canActive == false) continue;
                if (Vector3.Distance(playerPositionComponent.position, chestPositionComponent.position) <= chestInteractionComponent.triggerRadius)
                {
                    chestInteractionComponent.canActive = false;
                    PuzzleActions.OnChestTouched();
                    _isInitialized = true;
                    break;
                }
            }
        }
    }
}