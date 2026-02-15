using System;
using UniRx;

namespace Enemies
{
    public class EnemyHealthComponent : IDisposable
    {
        private int _maxHp;
        private int _currentHp;

        private Subject<Unit> _deathSubject;

        public bool IsAlive => _currentHp > 0;
        public IObservable<Unit> OnDeath => _deathSubject;

        public EnemyHealthComponent(int maxHp)
        {
            _maxHp = maxHp;
            _currentHp = maxHp;
            _deathSubject = new Subject<Unit>();
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;

            _currentHp -= damage;

            if (_currentHp <= 0)
                Die();
        }

        private void Die()
        {
            _currentHp = 0;
            _deathSubject.OnNext(Unit.Default);
        }

        public void Reset()
        {
            _currentHp = _maxHp;
        }

        public void Dispose()
        {
            _deathSubject.Dispose();
        }
    }
}