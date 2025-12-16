using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class PedestrianMovementSystem : IEcsRunSystem
{
    private EcsFilter<PedestrianComponent, PositionComponent> _pedestrianFilter;
    private EcsFilter<RoadComponent> _roadFilter;

    public void Run()
    {
        Dictionary<int, RoadComponent> roads = new Dictionary<int, RoadComponent>();

        foreach (var idx in _roadFilter)
        {
            ref var road = ref _roadFilter.Get1(idx);
            roads[road.id] = road;
        }

        foreach (var i in _pedestrianFilter)
        {
            ref var pedestrianComponent = ref _pedestrianFilter.Get1(i);
            ref var positionComponent = ref _pedestrianFilter.Get2(i);

            if (!roads.TryGetValue(pedestrianComponent.roadId, out var roadComponent))
                continue;

            Vector3 roadDirection = (roadComponent.endPoint - roadComponent.startPoint).normalized;
            Vector3 perpendicular = new Vector3(-roadDirection.z, 0, roadDirection.x);

            float delta = pedestrianComponent.moveSpeed * Time.deltaTime;
            float roadLength = Vector3.Distance(roadComponent.startPoint, roadComponent.endPoint);
            delta /= roadLength;

            if (pedestrianComponent.movingRight)
                pedestrianComponent.progress += delta;
            else
                pedestrianComponent.progress -= delta;

            if (pedestrianComponent.progress > 1f)
                pedestrianComponent.progress = 0f;
            else if (pedestrianComponent.progress < 0f)
                pedestrianComponent.progress = 1f;

            Vector3 positionOnCenterLine = Vector3.Lerp(
                roadComponent.startPoint,
                roadComponent.endPoint,
                pedestrianComponent.progress
            );

            Vector3 finalPosition = positionOnCenterLine + perpendicular * pedestrianComponent.laneOffset;

            pedestrianComponent.transform.position = finalPosition;

            positionComponent.position = finalPosition;
        }
    }
}