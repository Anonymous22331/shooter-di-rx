using System;
using Enemies;
using UniRx;
using UnityEngine;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class PlayerTargetLineView : MonoBehaviour, ILateTickable, IDisposable
    {
        [SerializeField] private Transform _player;
        [SerializeField] private float _startHeight = 1.2f;
        [SerializeField] private float _endHeight = 1.0f;

        private LineRenderer _lineRenderer;
        private IEnemy _currentTarget;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Construct(PlayerAttackComponent attack)
        {
            attack.Target
                .Subscribe(OnTargetChanged)
                .AddTo(_disposables);
        }

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }

        private void OnTargetChanged(IEnemy enemy)
        {
            _currentTarget = enemy;
            _lineRenderer.enabled = enemy != null;
        }

        public void LateTick()
        {
            if (_currentTarget == null)
                return;

            var start = (_player != null ? _player.position : transform.position)
                        + Vector3.up * _startHeight;

            var end = _currentTarget.Position + Vector3.up * _endHeight;

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}