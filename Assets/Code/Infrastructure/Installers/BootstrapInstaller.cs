using Zenject;

public class BootstrapInstaller : MonoInstaller, ICoroutineRunner, IInitializable
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
    }

    public void Initialize()
    {
        Container.Resolve<ISceneLoader>().LoadScene(Scenes.Main);
    }
}
