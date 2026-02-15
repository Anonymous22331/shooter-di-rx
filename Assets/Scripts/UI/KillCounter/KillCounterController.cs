using Enemies;
using UniRx;
using Zenject;

namespace UI
{
    public class KillCounterController : IKillCounterController, IInitializable
    {
        private readonly IEnemyCollection _enemies;
        private readonly ReactiveProperty<int> _killCount = new(0);

        public IReadOnlyReactiveProperty<int> KillCount => _killCount;

        public KillCounterController(IEnemyCollection enemies)
        {
            _enemies = enemies;
        }

        public void Initialize()
        {
            _enemies.OnEnemyDied
                .Subscribe(_ => _killCount.Value++);
        }
    }
}