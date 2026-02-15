using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private EnemyConfig config;

        public override void InstallBindings()
        {
            Container.Bind<EnemyConfig>()
                .FromInstance(config)
                .AsSingle();

            Container.Bind<IEnemyCollection>()
                .To<EnemyCollection>()
                .AsSingle();

            Container.BindInterfacesTo<EnemyRoot>()
                .FromComponentsInHierarchy()
                .AsCached();
        }
    }
}