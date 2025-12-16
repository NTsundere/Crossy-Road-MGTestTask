using Leopotam.Ecs;

public class RhythmResultToUISystem : IEcsRunSystem
{
    private EcsFilter<RhythmHitResultEvent> _resultFilter;
    private EcsWorld _world;

    public void Run()
    {
        foreach (var i in _resultFilter)
        {
            ref var result = ref _resultFilter.Get1(i);

            var uiEvent = _world.NewEntity();
            ref var showUI = ref uiEvent.Get<ShowHitResultUIEvent>();
            showUI.Accuracy = result.Accuracy;

            _resultFilter.GetEntity(i).Destroy();
        }
    }
}
