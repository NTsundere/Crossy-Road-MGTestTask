using Leopotam.Ecs;
using NUnit.Framework;
using UnityEngine;

public class ChestSpawnSystem : IEcsRunSystem
{
    private EcsFilter<ChestSpawnRequestComponent> _requestFilter;
    private EcsWorld _world;

    private bool _isInitialized = false;

    public void Run()
    {
        if (_isInitialized == false)
        {
            foreach (var i in _requestFilter)
            {
                ref var requestComponent = ref _requestFilter.Get1(i);

                EcsEntity chestEntity = _world.NewEntity();

                ChestInitData initData = ChestInitData.LoadFromAssets();
                GameObject chestInstance = GameObject.Instantiate(initData.chestPrefab, requestComponent.Position, Quaternion.identity);

                ref var interactionComponent = ref chestEntity.Get<ChestInteractionComponent>();
                interactionComponent.canActive = true;
                interactionComponent.triggerRadius = 2.5f;

                ref var positionComponent = ref chestEntity.Get<PositionComponent>();
                positionComponent.position = requestComponent.Position;

                _requestFilter.GetEntity(i).Destroy();

                _isInitialized = true;
            }
        }
    }
}
