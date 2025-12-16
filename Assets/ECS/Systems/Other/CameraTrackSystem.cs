using Leopotam.Ecs;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CameraTrackSystem : IEcsRunSystem
{
    private EcsFilter<PlayerComponent> _playerFilter;
    private SharedData _sharedData;

    private bool _isInitialized = false;

    public CameraTrackSystem(SharedData sharedData)
    {
        _sharedData = sharedData;
    }

    public void Run()
    {
        if (_sharedData.Camera != null && !_isInitialized)
        {
            foreach (var i in _playerFilter)
            {
                ref var player—omponent = ref _playerFilter.Get1(i);

                _sharedData.Camera.Follow = player—omponent.transform;
                _sharedData.Camera.LookAt = player—omponent.transform;
                _isInitialized = true;
            }
        }
    }
}
