using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

public class GameInitSystem : IEcsInitSystem
{
    private EcsWorld _world;

    private SharedData _data;

    public GameInitSystem(SharedData data)
    {
        _data = data;
    }

    public void Init()
    {
        CreatePlayer();
        CreateBattlePoint();
    }

    private void CreateBattlePoint()
    {
        EcsEntity battlePointEntity = _world.NewEntity();
        ref var battlePoint = ref battlePointEntity.Get<BattlePointComponent>();
        battlePoint.Position = _data.BattlePointPosition;
        battlePoint.ActivationRadius = 2.0f;
    }

    private void CreatePlayer()
    {
        EcsEntity playerEntity = _world.NewEntity();

        PlayerInitData initData = PlayerInitData.LoadFromAssets();
        GameObject playerInstance = GameObject.Instantiate(initData.playerPrefab, Vector3.zero, Quaternion.identity);

        ref var player = ref playerEntity.Get<PlayerComponent>();
        player.transform = playerInstance.transform;
        
        ref var positionComponent = ref playerEntity.Get<PositionComponent>();
        positionComponent.position = player.transform.position;

        ref var healthComponent = ref playerEntity.Get<HealthComponent>();
        healthComponent.maxHealth = 100;
        healthComponent.health = healthComponent.maxHealth;
        _data.PlayerHealth.GetComponentInChildren<Image>().fillAmount = (float)healthComponent.health / healthComponent.maxHealth;
       
        ref var attackComponent = ref playerEntity.Get<AttackComponent>();
        attackComponent.BaseDamage = 15;

        ref var movable = ref playerEntity.Get<MovableComponent>();
        movable.transform = playerInstance.transform;
        movable.moveSpeed = initData.defaultSpeed;
        movable.isMoving = false;

        ref var gridMovement = ref playerEntity.Get<GridMovementComponent>();
        gridMovement.currentGridPos = new Vector2Int(0, -1);
        gridMovement.targetGridPos = new Vector2Int(0, -1);
        gridMovement.moveDuration = 0.2f;
        gridMovement.jumpHeight = 0.5f;
        gridMovement.moveTimer = 0f;
        gridMovement.isMoving = false;
        gridMovement.startWorldPos = new Vector3(0, 0.2f, -1);
        gridMovement.targetWorldPos = new Vector3(0, 0.2f, -1);

        ref var state = ref playerEntity.Get<PlayerStateComponent>();
        state.currentState = PlayerStateComponent.GameState.Ready;

        ref var input = ref playerEntity.Get<InputEventComponent>();
        input.direction = Vector2Int.zero;
        input.hasInput = false;
    }
}
