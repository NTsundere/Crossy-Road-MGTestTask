using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class RhythmBeatControlSystem : IEcsRunSystem
{
    private EcsFilter<RhythmCombatComponent> _combatFilter;
    private EcsWorld _world;

    private SharedData _data;

    public RhythmBeatControlSystem(SharedData data)
    {
        _data = data;
    }

    public void Run()
    {
        foreach (var i in _combatFilter)
        {
            ref var combatComponent = ref _combatFilter.Get1(i);

            if (!combatComponent.IsActive) continue;

            combatComponent.CurrentBeatTimer -= Time.deltaTime;
            if (combatComponent.CurrentBeatTimer < 0)
            {
                EcsEntity beatAnimationEvent = _world.NewEntity();
                ref var evnt = ref beatAnimationEvent.Get<StartBeatAnimationEvent>();
                evnt.HitWindow = combatComponent.HitWindow;
                evnt.BeatDuration = combatComponent.BeatInterval;

                EcsEntity beatEntity = _world.NewEntity();
                ref var beatComponent = ref beatEntity.Get<RhythmBeatComponent>();
                beatComponent.WasHit = false;
                beatComponent.LifeTime = combatComponent.HitWindow;
                beatComponent.SpawnTime = Time.time; 

                ref var scaleComponent = ref beatEntity.Get<BeatScaleComponent>();
                scaleComponent.StartScale = _data.RhytmUIHandler.StartScale; 
                scaleComponent.TargetScale = _data.RhytmUIHandler.TargetScale; 
                scaleComponent.CurrentScale = scaleComponent.StartScale;
                scaleComponent.ScaleSpeed = 1f;

                combatComponent.CurrentBeatTimer = combatComponent.BeatInterval;
                combatComponent.BeatCount++;
            }
        }
    }
}
