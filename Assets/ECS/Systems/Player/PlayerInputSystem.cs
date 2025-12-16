using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : IEcsRunSystem
{
    private EcsFilter<PlayerStateComponent, InputEventComponent> _playerFilter;

    public void Run()
    {
        foreach (var i in _playerFilter)
        {
            ref var state = ref _playerFilter.Get1(i);
            ref var input = ref _playerFilter.Get2(i);

            input.hasInput = false;
            input.direction = Vector2Int.zero;

            if (state.currentState != PlayerStateComponent.GameState.Ready || state.currentState == PlayerStateComponent.GameState.Battle) continue;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                input.direction = Vector2Int.up;
                input.hasInput = true;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                input.direction = Vector2Int.down;
                input.hasInput = true;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                input.direction = Vector2Int.left;
                input.hasInput = true;
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                input.direction = Vector2Int.right;
                input.hasInput = true;
            }
        }
    }
}
