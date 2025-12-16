using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;

public class PlayerDeathSystem : IEcsRunSystem
{
    private EcsFilter<PlayerComponent, PositionComponent> _players;
    private EcsFilter<PedestrianComponent, PositionComponent> _pedestrians;
    private DeathSettings _settings;

    private EcsWorld _world;

    public PlayerDeathSystem(DeathSettings settings)
    {
        _settings = settings;
    }

    public void Run()
    {
        float deathRadiusSqr = _settings.deathRadius * _settings.deathRadius;

        foreach (var p in _players)
        {
            ref var playerPos = ref _players.Get2(p);
            var playerEntity = _players.GetEntity(p);

            foreach (var ped in _pedestrians)
            {
                ref var pedestrianPos = ref _pedestrians.Get2(ped);

                if ((playerPos.position - pedestrianPos.position).sqrMagnitude < deathRadiusSqr)
                {
                    var deathEvent = _world.NewEntity();
                    ref var evt = ref deathEvent.Get<DeathEvent>();
                    evt.killedEntity = playerEntity;
                    evt.killerEntity = _pedestrians.GetEntity(ped);

                    return; 
                }
            }
        }
    }
}
