using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyPatrolComponent : ITickable
    {
        private Transform _transform;
        private PatrolPath _path;
        private float _moveSpeed;

        private int _currentPointIndex;
        private const float _arriveSqrThreshold = 0.01f;

        public EnemyPatrolComponent(
            Transform transform,
            PatrolPath path,
            EnemyConfig config)
        {
            _transform = transform;
            _path = path;
            _moveSpeed = config.MoveSpeed;
        }

        public void Tick()
        {
            if (_path == null || _path.Points.Count == 0)
                return;

            Transform target = _path.Points[_currentPointIndex];
            if (target == null)
            {
                AdvanceIndex();
                return;
            }

            Vector3 currentPosition = _transform.position;
            Vector3 targetPosition = target.position;

            Vector3 nextPosition = Vector3.MoveTowards(
                currentPosition,
                targetPosition,
                _moveSpeed * Time.deltaTime);

            _transform.position = nextPosition;

            if ((targetPosition - nextPosition).sqrMagnitude <= _arriveSqrThreshold)
                AdvanceIndex();
        }

        private void AdvanceIndex()
        {
            _currentPointIndex = (_currentPointIndex + 1) % _path.Points.Count;
        }
    }
}