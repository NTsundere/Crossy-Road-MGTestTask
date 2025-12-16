using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathProcessingSystem : IEcsRunSystem
{
    private EcsFilter<DeathEvent> _deathEvents;

    public void Run()
    {

        foreach (var i in _deathEvents)
        {
            ref var deathEvent = ref _deathEvents.Get1(i);

            if (deathEvent.killedEntity.Has<PlayerComponent>())
            {
                SceneManager.LoadScene(Scenes.Main);
            }
            else
            {
                if (deathEvent.killedEntity.Has<EnemyComponent>())
                {
                    ref var enemyComponent = ref deathEvent.killedEntity.Get<EnemyComponent>();
                    GameObject.Destroy(enemyComponent.transform.gameObject);
                }
                deathEvent.killedEntity.Destroy();
            }
        }
    }
}