using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public sealed class PlayerMovementComponent : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly Transform _transform;
        private readonly float _moveSpeed;

        public PlayerMovementComponent(Transform transform, PlayerConfig config, IObservable<Vector2> moveStream)
        {
            _transform = transform;
            _moveSpeed = config.MoveSpeed;
            var sharedMove = moveStream.StartWith(Vector2.zero).Replay(1).RefCount();
            Observable.EveryUpdate().WithLatestFrom(sharedMove, (_, axis) => axis)
                .Where(axis => axis.sqrMagnitude > 0.0001f).Subscribe(Move).AddTo(_disposables);
        }

        private void Move(Vector2 axis)
        {
            var direction = _transform.right * axis.x + _transform.forward * axis.y;
            if (direction.sqrMagnitude > 0.0001f) direction.Normalize();
            var delta = direction * _moveSpeed * Time.deltaTime;
            _transform.position += delta;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}