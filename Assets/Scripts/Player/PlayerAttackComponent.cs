using System;
using Enemies;
using UniRx;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerAttackComponent : IDisposable
    {
        private readonly Transform _player;
        private readonly PlayerConfig _config;
        private readonly IEnemyCollection _enemies;
        private readonly IObservable<bool> _isMovingStream;

        private readonly CompositeDisposable _disposables = new();

        private readonly IObservable<IEnemy> _targetStream;
        public IObservable<IEnemy> Target => _targetStream;

        [Inject]
        public PlayerAttackComponent(
            Transform player,
            PlayerConfig config,
            IPlayerInput input,
            IEnemyCollection enemies)
        {
            _player = player;
            _config = config;
            _enemies = enemies;
            _isMovingStream = input.IsMoving;

            _targetStream = CreateTargetFindStream().Replay(1).RefCount();
            BindAttackLoop();
        }

        private IObservable<IEnemy> CreateTargetFindStream()
        {
            return Observable.EveryUpdate()
                .Select(_ => FindNearestEnemy())
                .DistinctUntilChanged()
                .Share();
        }

        private void BindAttackLoop()
        {
            if (_config.AttackSpeed <= 0f)
                return;

            var attackInterval = TimeSpan.FromSeconds(1f / _config.AttackSpeed);

            _isMovingStream
                .DistinctUntilChanged()
                .Select(isMoving =>
                {
                    if (isMoving)
                        return Observable.Empty<long>();

                    return Observable.Interval(attackInterval);
                })
                .Switch()
                .WithLatestFrom(_targetStream, (_, target) => target)
                .Where(target => target != null)
                .Subscribe(Attack)
                .AddTo(_disposables);
        }

        private void Attack(IEnemy enemy)
        {
            var direction = enemy.Position - _player.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.0001f)
                _player.forward = direction.normalized;

            enemy.TakeDamage(_config.Damage);
        }

        private IEnemy FindNearestEnemy()
        {
            var origin = _player.position;
            var radiusSqr = _config.AttackRadius * _config.AttackRadius;

            IEnemy best = null;
            var bestSqr = float.PositiveInfinity;

            var list = _enemies.AllEnemies;

            for (int i = 0; i < list.Count; i++)
            {
                var enemy = list[i];
                if (enemy == null || !enemy.IsAlive)
                    continue;

                if (enemy is MonoBehaviour mb && !mb.isActiveAndEnabled)
                    continue;

                var sqr = (enemy.Position - origin).sqrMagnitude;
                if (sqr > radiusSqr)
                    continue;

                if (sqr < bestSqr)
                {
                    best = enemy;
                    bestSqr = sqr;
                }
            }

            return best;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
