using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyRoot : MonoBehaviour,
        IEnemy,
        ITickable,
        IInitializable,
        IDisposable
    {
        [SerializeField] private PatrolPath _patrolPath;

        private EnemyHealthComponent _health;
        private EnemyRespawnComponent _respawn;
        private EnemyPatrolComponent _patrol;

        private EnemyConfig _config;
        private IEnemyCollection _collection;

        private bool _isInactiveDueToLimit;

        public Vector3 Position => transform.position;
        public bool IsAlive => _health.IsAlive;
        public IObservable<Unit> OnDeath => _health.OnDeath;

        [Inject]
        public void Construct(
            EnemyConfig config,
            IEnemyCollection collection)
        {
            _config = config;
            _collection = collection;
        }

        public void Initialize()
        {
            _health = new EnemyHealthComponent(_config.MaxHp);
            _respawn = new EnemyRespawnComponent(_health, _config, _collection, this);
            _patrol = new EnemyPatrolComponent(transform, _patrolPath, _config);

            _respawn.OnRespawned
                .Subscribe(_ => HandleRespawned())
                .AddTo(this);

            _health.OnDeath
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(this);

            _collection.OnEnemyDeactivatedByLimit
                .Where(e => ReferenceEquals(e, this))
                .Subscribe(_ => DeactivateByLimit())
                .AddTo(this);

            _collection.OnFreeSlotAppeared
                .Where(_ => _isInactiveDueToLimit && _collection.TryClaimFreeSlot(this))
                .Subscribe(_ => ReactivateFromLimit())
                .AddTo(this);

            _collection.Register(this);
            _respawn.Initialize();
        }

        public void TakeDamage(int damage)
        {
            _health.TakeDamage(damage);
        }

        public void Tick()
        {
            if (!IsAlive || _isInactiveDueToLimit)
                return;

            _patrol.Tick();
        }

        private void HandleRespawned()
        {
            gameObject.SetActive(true);
            _collection.ConfirmRespawned(this);
        }

        private void DeactivateByLimit()
        {
            _isInactiveDueToLimit = true;
            gameObject.SetActive(false);
        }

        private void ReactivateFromLimit()
        {
            _isInactiveDueToLimit = false;
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            _collection?.Unregister(this);
            _health?.Dispose();
            _respawn?.Dispose();
        }
        
    }
}
