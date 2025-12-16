using Cysharp.Threading.Tasks;
using DG.Tweening;
using Leopotam.Ecs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class BossEncounterStartSystem : IEcsRunSystem
{
    private EcsFilter<BossEncounterRequestComponent> _requestFilter;
    private EcsFilter<PlayerComponent> _playerFilter;

    private EcsWorld _world;
    private SharedData _data;

    private bool _isInitialized = false;

    public BossEncounterStartSystem(SharedData data)
    {
        _data = data;
    }

    public void Run()
    {
        if (_isInitialized == false)
        {
            foreach (var i in _requestFilter)
            {
                ref var requestComponent = ref _requestFilter.Get1(i);

                for (int j = 0; j < 2; j++)
                {
                    EcsEntity bossEntity = _world.NewEntity();

                    BossInitData initData = BossInitData.LoadFromAssets();
                    GameObject bossInstance = GameObject.Instantiate(initData.bossPrefab, requestComponent.BattleCenter + new Vector3(6 * j - 3f, 3.5f, 5f), Quaternion.identity);

                    ref var bossEnemyComponent = ref bossEntity.Get<EnemyComponent>();
                    bossEnemyComponent.transform = bossInstance.transform;

                    ref var bossPositionComponent = ref bossEntity.Get<PositionComponent>();
                    bossPositionComponent.position = bossInstance.transform.position;

                    ref var bossHealthComponent = ref bossEntity.Get<HealthComponent>();
                    bossHealthComponent.maxHealth = 100;
                    bossHealthComponent.health = bossHealthComponent.maxHealth;

                    ref var bossAttackComponent = ref bossEntity.Get<AttackComponent>();
                    bossAttackComponent.AttackInterval = 1.5f * j + 1.5f;
                    bossAttackComponent.CurrentAttackTimer = bossAttackComponent.AttackInterval;
                    bossAttackComponent.BaseDamage = 10;

                    bossInstance.transform.LookAt(requestComponent.BattleCenter + Vector3.up);

                    ChangeCameraSize(11f).Forget();
                    ActivatePlayerRhytmCombat();

                    _data.PlayerHealth.SetActive(true);
                    _data.EnemyHealth.SetActive(true);

                    _isInitialized = true;
                }

            }
        }
    }

    private void ActivatePlayerRhytmCombat()
    {
        foreach (var k in _playerFilter)
        {
            EcsEntity playerEntity = _playerFilter.GetEntity(k);
            ref var combatComponent = ref playerEntity.Get<RhythmCombatComponent>();
            combatComponent.IsActive = true;
            combatComponent.BeatInterval = 1f;
            combatComponent.CurrentBeatTimer = combatComponent.BeatInterval;
            combatComponent.HitWindow = 0.2f;
            combatComponent.MaxBeats = 10;
        }
    }

    private async UniTaskVoid ChangeCameraSize(float newSize)
    {
        if (_data.Camera.Lens.OrthographicSize == newSize) return;

        CinemachineFollow cameraFollow = _data.Camera.GetComponent<CinemachineFollow>();

        for (float i = 0; i < 2f; i += Time.deltaTime)
        {
            _data.Camera.Lens.OrthographicSize = Mathf.Lerp(_data.Camera.Lens.OrthographicSize, newSize, EaseInOut(i));
            cameraFollow.FollowOffset.x = Mathf.Lerp(cameraFollow.FollowOffset.x, 22f, EaseInOut(i));
            cameraFollow.FollowOffset.y = Mathf.Lerp(cameraFollow.FollowOffset.x, 23f, EaseInOut(i));

            await UniTask.Delay(10);
        }
    }
    private float EaseInOut(float x)
    {
        return x < 0.5f ? x * x * 2 : (1 - (1 - x) * (1 - x) * 2);
    }
}
