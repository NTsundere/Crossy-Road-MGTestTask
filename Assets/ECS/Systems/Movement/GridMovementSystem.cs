using Leopotam.Ecs;
using UnityEngine;

public class GridMovementSystem : IEcsRunSystem
{
    private const float _heightY = 0.2f;

    private EcsFilter<PlayerStateComponent, InputEventComponent,
                     GridMovementComponent, MovableComponent> _playerFilter;

    public void Run()
    {
        foreach (var i in _playerFilter)
        {
            ref var state = ref _playerFilter.Get1(i);
            ref var input = ref _playerFilter.Get2(i);
            ref var grid = ref _playerFilter.Get3(i);
            ref var movable = ref _playerFilter.Get4(i);

            if (!grid.isMoving && input.hasInput)
            {
                Vector2Int destination = grid.currentGridPos + input.direction;

                float actualStepDistance = 2.0f; 

                Vector3 directionVector = new Vector3(input.direction.x, 0, input.direction.y);
                grid.startWorldPos = movable.transform.position;
                grid.targetWorldPos = grid.startWorldPos + (directionVector * actualStepDistance);

                grid.moveDuration = 0.2f;
                grid.moveTimer = 0f;
                grid.isMoving = true;

                grid.targetGridPos = new Vector2Int(
                    grid.currentGridPos.x + (input.direction.x * (int)actualStepDistance),
                    grid.currentGridPos.y + (input.direction.y * (int)actualStepDistance)
                );
            }

            if (grid.isMoving)
            {
                grid.moveTimer += Time.deltaTime;
                float percent = Mathf.Clamp01(grid.moveTimer / grid.moveDuration);

                Vector3 newPos = Vector3.Lerp(grid.startWorldPos, grid.targetWorldPos, percent);

                newPos.y = _heightY + (grid.jumpHeight * Mathf.Sin(Mathf.PI * percent));
                movable.transform.position = newPos;

                Vector3 rotation = movable.transform.localRotation.eulerAngles;
                float tilt = -5f * Mathf.PI * Mathf.Cos(Mathf.PI * percent);
                movable.transform.localRotation = Quaternion.Euler(tilt, rotation.y, rotation.z);

                if (grid.moveTimer >= grid.moveDuration)
                {
                    movable.transform.position = grid.targetWorldPos;
                    grid.currentGridPos = grid.targetGridPos;
                    grid.isMoving = false;

                    movable.transform.localRotation = Quaternion.Euler(0, rotation.y, rotation.z);

                    if (state.currentState == PlayerStateComponent.GameState.Moving)
                    {
                        state.currentState = PlayerStateComponent.GameState.Ready;
                    }
                }
            }
        }
    }

    private bool InStartArea(Vector2Int location)
    {
        return (location.y > -5) && (location.y < 100) && (location.x > -6) && (location.x < 6);
    }

    private void RotateCharacter(Transform characterTransform, Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            characterTransform.localRotation = Quaternion.identity;
        else if (direction == Vector2Int.down)
            characterTransform.localRotation = Quaternion.Euler(0, 180, 0);
        else if (direction == Vector2Int.left)
            characterTransform.localRotation = Quaternion.Euler(0, -90, 0);
        else if (direction == Vector2Int.right)
            characterTransform.localRotation = Quaternion.Euler(0, 90, 0);
    }
}
