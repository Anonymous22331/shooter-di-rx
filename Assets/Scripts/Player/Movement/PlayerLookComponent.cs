using System;
using UniRx;
using UnityEngine;

namespace Player
{
    public class PlayerLookComponent : IDisposable
    {
        private readonly Transform _player;
        private readonly Transform _cameraPivot;
        private readonly float _turnSpeed;
        private readonly float _minVerticalAngle;
        private readonly float _maxVerticalAngle;

        private float _verticalAngle;
        private readonly CompositeDisposable _disposables = new();

        public PlayerLookComponent(
            Transform player,
            Transform cameraPivot,
            IObservable<Vector2> lookStream,
            float turnSpeed,
            float minVerticalAngle,
            float maxVerticalAngle)
        {
            _player = player;
            _cameraPivot = cameraPivot;
            _turnSpeed = turnSpeed;
            _minVerticalAngle = minVerticalAngle;
            _maxVerticalAngle = maxVerticalAngle;

            InitializePitch();

            lookStream
                .Where(v => v.sqrMagnitude > 0.000001f)
                .Subscribe(UpdateLook)
                .AddTo(_disposables);
        }

        private void InitializePitch()
        {
            if (_cameraPivot == null) return;

            var x = _cameraPivot.localEulerAngles.x;
            _verticalAngle = NormalizePitch(x);
            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);
            _cameraPivot.localRotation = Quaternion.Euler(_verticalAngle, 0f, 0f);
        }

        private void UpdateLook(Vector2 look)
        {
            var dt = Time.deltaTime;

            _player.Rotate(0f, look.x * _turnSpeed * dt, 0f, Space.World);

            if (_cameraPivot == null)
                return;

            _verticalAngle -= look.y * _turnSpeed * dt;
            _verticalAngle = Mathf.Clamp(_verticalAngle, _minVerticalAngle, _maxVerticalAngle);
            _cameraPivot.localRotation = Quaternion.Euler(_verticalAngle, 0f, 0f);
        }

        private static float NormalizePitch(float x)
        {
            x %= 360f;
            if (x > 180f) x -= 360f;
            return x;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
