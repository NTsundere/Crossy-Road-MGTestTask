using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawnSystem : IEcsRunSystem
{
    private EcsWorld _world;
    private EcsFilter<RoadComponent> _roadsFilter;
    
    private bool _isInitialized = false;

    public void Run()
    {
        if (!_isInitialized)
        {
            if (_roadsFilter.GetEntitiesCount() > 0)
            {
                InitializePedestrians();
                _isInitialized = true;
            }
        }
    }

    private void InitializePedestrians()
    {
        foreach (var i in _roadsFilter)
        {
            ref var road = ref _roadsFilter.Get1(i);
            int pedestrianCount = (road.id + 1) * 5;

            road.startPoint.y = -0.3f;
            road.endPoint.y = -0.3f;

            List<Vector3> positions = GeneratePedestrianPositions(road, pedestrianCount);

            foreach (Vector3 position in positions)
            {
                CreatePedestrianAtPosition(road, position);
            }
        }
    }

    private List<Vector3> GeneratePedestrianPositions(RoadComponent road, int count)
    {
        List<Vector3> positions = new List<Vector3>();

        if (count <= 0 || road.width <= 0)
            return positions;

        Vector3 roadDirection = (road.endPoint - road.startPoint).normalized;
        Vector3 perpendicular = new Vector3(-roadDirection.z, 0, roadDirection.x);

        float laneSpacing = 3.5f;
        float firstRowOffset = -2f;

        float basePerLane = count / road.width;     
        float remainder = count % road.width;        

        for (int lane = 0; lane < road.width; lane++)
        {
            float pedestriansInThisLane = basePerLane;
            if (lane < remainder) 
                pedestriansInThisLane++;

            if (pedestriansInThisLane <= 0)
                continue; 

            float laneOffset = (lane == 0) ? firstRowOffset : firstRowOffset + (lane * laneSpacing);

            for (int i = 0; i < pedestriansInThisLane; i++)
            {
                float segmentStart = (float)i / pedestriansInThisLane;
                float segmentEnd = (float)(i + 1) / pedestriansInThisLane;
                float randomT = Random.Range(segmentStart, segmentEnd);

                Vector3 position = Vector3.Lerp(road.startPoint, road.endPoint, randomT);
                position += perpendicular * laneOffset;

                positions.Add(position);
            }
        }

        Shuffle(positions);
        return positions;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void CreatePedestrianAtPosition(RoadComponent road, Vector3 position)
    {
        EcsEntity pedestrianEntity = _world.NewEntity();
        PedestrianInitData initData = PedestrianInitData.LoadFromAssets();

        if (initData == null || initData.pedestrianPrefab == null) return;

        GameObject pedestrianInstance = GameObject.Instantiate(
            initData.pedestrianPrefab,
            position,
            Quaternion.identity
        );

        ref var positionComponent = ref pedestrianEntity.Get<PositionComponent>();
        positionComponent.position = position;

        ref var pedestrianComponent = ref pedestrianEntity.Get<PedestrianComponent>();
        pedestrianComponent.transform = pedestrianInstance.transform;
        pedestrianComponent.roadId = road.id;
        pedestrianComponent.moveSpeed = road.moveSpeed;

        Vector3 roadVector = road.endPoint - road.startPoint;
        float roadLength = roadVector.magnitude;
        Vector3 toPedestrian = position - road.startPoint;

        pedestrianComponent.progress = Vector3.Dot(toPedestrian, roadVector.normalized) / roadLength;

        int randMoving = Random.Range(0, 2); 
        pedestrianComponent.movingRight = (randMoving == 1);

        Vector3 roadDirection = (road.endPoint - road.startPoint).normalized;
        Vector3 perpendicular = new Vector3(-roadDirection.z, 0, roadDirection.x);

        Vector3 closestPointOnLine = GetClosestPointOnLine(road.startPoint, road.endPoint, position);

        Vector3 offsetVector = position - closestPointOnLine;
        pedestrianComponent.laneOffset = Vector3.Dot(offsetVector, perpendicular);
        if (!pedestrianComponent.movingRight)
        {
            roadDirection = -roadDirection; 
        }

        if (roadDirection != Vector3.zero)
        {
            pedestrianComponent.transform.rotation = Quaternion.LookRotation(roadDirection);
        }
    }
    private Vector3 GetClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 line = end - start;
        float lineLength = line.magnitude;
        Vector3 lineDirection = line.normalized;

        Vector3 startToPoint = point - start;
        float dotProduct = Vector3.Dot(startToPoint, lineDirection);
        dotProduct = Mathf.Clamp(dotProduct, 0f, lineLength);

        return start + lineDirection * dotProduct;
    }
}