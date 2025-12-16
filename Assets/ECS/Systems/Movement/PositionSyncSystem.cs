using Leopotam.Ecs;

public class PositionSyncSystem : IEcsRunSystem
{
    private EcsFilter<PlayerComponent, PositionComponent> _playerFilter;

    private EcsFilter<PedestrianComponent, PositionComponent> _pedestrianFilter;

    public void Run()
    {
        foreach (var i in _playerFilter)
        {
            ref var player = ref _playerFilter.Get1(i);
            ref var position = ref _playerFilter.Get2(i);

            if (player.transform != null)
                position.position = player.transform.position;
        }

        foreach (var i in _pedestrianFilter)
        {
            ref var pedestrian = ref _pedestrianFilter.Get1(i);
            ref var position = ref _pedestrianFilter.Get2(i);

            if (pedestrian.transform != null)
                position.position = pedestrian.transform.position;
        }
    }
}