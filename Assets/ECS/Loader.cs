using Leopotam.Ecs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    EcsWorld _world;
    EcsSystems _systems;

    public EcsWorld World => _world;

    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private RhytmUIHandler _rhytmUIHandler;
    [SerializeField] private Transform _battlePointPosition;
    [SerializeField] private GameObject _battlePoint;
    [SerializeField] private GameObject _enemyHealth;
    [SerializeField] private GameObject _playerHealth;

    private SharedData _sharedData;
    private DeathSettings _deathSettings;

    private void Awake()
    {
        _world = new EcsWorld();
        
        EcsWorldContainer.World = _world;
        
        _systems = new EcsSystems(_world);

        _sharedData = new SharedData();
        _sharedData.Camera = _camera;
        _sharedData.RhytmUIHandler = _rhytmUIHandler;
        _sharedData.BattlePointPosition = _battlePointPosition.position;
        _sharedData.BattlePoint = _battlePoint;
        _sharedData.EnemyHealth = _enemyHealth;
        _sharedData.PlayerHealth = _playerHealth;

        _deathSettings = new DeathSettings();

        _systems
            .Add(new GameInitSystem(_sharedData))

            .Add(new PlayerInputSystem())
            .Add(new GridMovementSystem()) 

            .Add(new PedestrianSpawnSystem()) 
            .Add(new CameraTrackSystem(_sharedData))
            .Add(new PositionSyncSystem())
            .Add(new PedestrianMovementSystem())

            .Add(new PlayerDeathSystem(_deathSettings))
            .Add(new DeathProcessingSystem())

            .Add(new BossEncounterStartSystem(_sharedData))
            .Add(new BattlePointTriggerSystem())
            .Add(new RhythmBeatControlSystem(_sharedData))
            .Add(new RhythmCombatInputSystem())
            .Add(new RhythmHitResolutionSystem())
            .Add(new PlayerAttackSystem())
            .Add(new RhythmResultToUISystem())
            .Add(new EnemyAttackSystem())

            .Add(new ChestSpawnSystem())
            .Add(new ChestTriggerSystem())

            .Add(new UISyncSystem(_sharedData))
            .Inject(_sharedData)
            .Inject(_deathSettings)
            .Init();
    }

    private void Update()
    {
        _systems.Run();
    }

    private void OnDestroy()
    {
        _systems.Destroy();

        _world.Destroy();
    }
}
