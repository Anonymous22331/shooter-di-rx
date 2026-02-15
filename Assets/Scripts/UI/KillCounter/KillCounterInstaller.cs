using Zenject;

namespace UI
{
    public class KillCounterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<KillCounterController>().AsSingle();
        }
    }
}