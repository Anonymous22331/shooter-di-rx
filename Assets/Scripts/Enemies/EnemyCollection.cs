using System;
using System.Collections.Generic;
using UniRx;

namespace Enemies
{
    public class EnemyCollection : IEnemyCollection, IDisposable
    {
        private readonly List<IEnemy> _allEnemies = new();
        private readonly HashSet<IEnemy> _inactiveEnemiesByLimit = new();
        private readonly HashSet<IEnemy> _reservedForRespawn = new();

        private readonly Dictionary<IEnemy, IDisposable> _deathSubscriptions = new();

        private readonly int _maxEnemiesOnLevel;

        private readonly Subject<IEnemy> _enemyDeactivatedByLimit = new();
        private readonly Subject<Unit> _freeSlotAppeared = new();
        private readonly Subject<IEnemy> _enemyDied = new();

        public IReadOnlyList<IEnemy> AllEnemies => _allEnemies;
        public IObservable<IEnemy> OnEnemyDeactivatedByLimit => _enemyDeactivatedByLimit;
        public IObservable<Unit> OnFreeSlotAppeared => _freeSlotAppeared;
        public IObservable<IEnemy> OnEnemyDied => _enemyDied;

        public EnemyCollection(EnemyConfig config)
        {
            _maxEnemiesOnLevel = config.MaxEnemiesOnLevel;
        }

        public void Register(IEnemy enemy)
        {
            if (enemy == null || _allEnemies.Contains(enemy))
                return;

            _allEnemies.Add(enemy);
            
            var sub = enemy.OnDeath
                .Take(1)
                .Subscribe(_ => HandleEnemyDeath(enemy));

            _deathSubscriptions[enemy] = sub;

            EvaluateLimit(enemy);
        }

        public void Unregister(IEnemy enemy)
        {
            if (enemy == null)
                return;

            _allEnemies.Remove(enemy);
            _inactiveEnemiesByLimit.Remove(enemy);
            _reservedForRespawn.Remove(enemy);

            if (_deathSubscriptions.TryGetValue(enemy, out var sub))
            {
                sub.Dispose();
                _deathSubscriptions.Remove(enemy);
            }

            _freeSlotAppeared.OnNext(Unit.Default);
        }

        private void HandleEnemyDeath(IEnemy enemy)
        {
            if (!_inactiveEnemiesByLimit.Contains(enemy))
                _freeSlotAppeared.OnNext(Unit.Default);

            _enemyDied.OnNext(enemy);
        }

        private void EvaluateLimit(IEnemy enemy)
        {
            if (GetAliveActiveCount() <= _maxEnemiesOnLevel)
                return;

            if (_inactiveEnemiesByLimit.Add(enemy))
                _enemyDeactivatedByLimit.OnNext(enemy);
        }

        private int GetAliveActiveCount()
        {
            int count = 0;

            for (int i = 0; i < _allEnemies.Count; i++)
            {
                var enemy = _allEnemies[i];
                if (enemy == null) continue;
                if (_inactiveEnemiesByLimit.Contains(enemy)) continue;
                if (!enemy.IsAlive) continue;
                count++;
            }

            return count;
        }

        private bool HasFreeSlot()
            => GetAliveActiveCount() + _reservedForRespawn.Count < _maxEnemiesOnLevel;

        public bool TryClaimFreeSlot(IEnemy enemy)
        {
            if (!HasFreeSlot())
                return false;

            if (_inactiveEnemiesByLimit.Remove(enemy))
                return true;

            if (!enemy.IsAlive)
                return _reservedForRespawn.Add(enemy);

            return false;
        }

        public void ConfirmRespawned(IEnemy enemy)
        {
            _reservedForRespawn.Remove(enemy);
        }

        public void Dispose()
        {
            foreach (var kv in _deathSubscriptions)
                kv.Value.Dispose();
            _deathSubscriptions.Clear();

            _enemyDeactivatedByLimit.Dispose();
            _freeSlotAppeared.Dispose();
            _enemyDied.Dispose();
        }
    }
}