using Leopotam.Ecs;
using System.Collections;
using UnityEngine;
using Zenject;

public class RoadAuthoring : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private int _width = 2;
    [SerializeField] private int _id;

    void Start()
    {
        StartCoroutine(WaitForWorld());
    }
    private IEnumerator WaitForWorld()
    {
        while (EcsWorldContainer.World == null)
        {
            yield return null;
        }

        CreateRoadUsingContainer();
    }

    private void CreateRoadUsingContainer()
    {
        EcsEntity roadEntity = EcsWorldContainer.World.NewEntity();
        ref var road = ref roadEntity.Get<RoadComponent>();

        road.id = _id;
        road.moveSpeed = _speed;
        road.startPoint = transform.position - transform.right * 13f;
        road.endPoint = transform.position + transform.right * 13f;
        road.width = _width;

        enabled = false;
    }
}
