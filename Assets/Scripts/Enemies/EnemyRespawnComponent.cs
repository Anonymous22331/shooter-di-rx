using System;
using UniRx;
using Zenject;

namespace Enemies
{
    public class EnemyRespawnComponent : IInitializable, IDisposable
    {
        private EnemyHealthComponent _health;
        private EnemyConfig _config;
        private IEnemyCollection _collection;
        private IEnemy _enemy;

        private CompositeDisposable _disposables;
        private SerialDisposable _respawnProcess;

        private Subject<Unit> _respawnedSubject;
        public IObservable<Unit> OnRespawned => _respawnedSubject;

        public EnemyRespawnComponent(
            EnemyHealthComponent health,
            EnemyConfig config,
            IEnemyCollection collection,
            IEnemy enemy)
        {
            _health = health;
            _config = config;
            _collection = collection;
            _enemy = enemy;

            _disposables = new CompositeDisposable();
            _respawnProcess = new SerialDisposable();
            _respawnedSubject = new Subject<Unit>();
        }

        public void Initialize()
        {
            _health.OnDeath
                .Subscribe(_ => BeginRespawnFlow())
                .AddTo(_disposables);

            _respawnProcess.AddTo(_disposables);
        }

        private void BeginRespawnFlow()
        {
            _respawnProcess.Disposable = CreateRespawnObservable()
                .Subscribe(_ => CompleteRespawn());
        }

        private IObservable<Unit> CreateRespawnObservable()
        {
            return Observable
                .Timer(TimeSpan.FromSeconds(_config.RespawnTime))
                .SelectMany(_ => WaitForFreeSlot());
        }

        private IObservable<Unit> WaitForFreeSlot()
        {
            if (_collection.TryClaimFreeSlot(_enemy))
                return Observable.ReturnUnit();

            return _collection.OnFreeSlotAppeared
                .Where(_ => _collection.TryClaimFreeSlot(_enemy))
                .Take(1);
        }

        private void CompleteRespawn()
        {
            _health.Reset();
            _respawnedSubject.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _respawnedSubject.Dispose();
        }
    }
}
