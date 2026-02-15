using UnityEngine;
using Zenject;

namespace Bootstrap
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private string _firstSceneName = "MainScene";

        public override void InstallBindings()
        {
            Container.BindInstance(_firstSceneName);
            Container.BindInterfacesTo<GameBootstrapper>().AsSingle().NonLazy();
        }
    }
}