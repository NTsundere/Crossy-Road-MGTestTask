using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem;

public class RhythmCombatInputSystem : IEcsRunSystem
{
    private EcsFilter<RhythmCombatComponent> _combatFilter;
    private EcsWorld _world;
    private SharedData _data;

    public void Run()
    {
        foreach (var i in _combatFilter)
        {
            ref var combatComponent = ref _combatFilter.Get1(i);
            if (!combatComponent.IsActive) continue;

            float currentTime = Time.time;

            bool inputDetected = false;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                inputDetected = true;
            }
            else if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                inputDetected = true;
            }
            else if (Touchscreen.current != null &&
                     Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                inputDetected = true;
            }

            if (inputDetected)
            {
                EcsEntity inputEvent = _world.NewEntity();
                ref var inputComponent = ref inputEvent.Get<PlayerInputEvent>();
                inputComponent.InputTime = currentTime;
            }
        }
    }
}
