using Leopotam.Ecs;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    [SerializeField] private Loader _loader;
    public override void InstallBindings()
    {
        Container.Bind<Loader>().FromInstance(_loader).AsSingle();
    }
}
