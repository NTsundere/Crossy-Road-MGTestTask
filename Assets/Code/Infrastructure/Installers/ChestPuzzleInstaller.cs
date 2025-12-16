using ChestPuzzle.Interfaces;
using ChestPuzzle.Models;
using ChestPuzzle.Presenters;
using ChestPuzzle.Views;
using UnityEngine;
using Zenject;

public class ChestPuzzleInstaller : MonoInstaller
{
    [SerializeField] private PuzzleConfig _config;
    [SerializeField] private ChestPuzzleView _viewPrefab;
    [SerializeField] private Transform _uiParent;

    public override void InstallBindings()
    {
        Container.BindInstance(_config).AsSingle();
        Container.Bind<ChestPuzzleModel>().AsSingle();
        Container.Bind<IPuzzleGenerator>().To<PuzzleGenerator>().AsSingle();

        Container.Bind<IChestPuzzleView>()
            .FromComponentInNewPrefab(_viewPrefab)
            .UnderTransform(_uiParent)
            .AsSingle()
            .NonLazy();
        
        Container.BindInterfacesAndSelfTo<ChestPuzzlePresenter>().AsSingle().NonLazy();
    }
}