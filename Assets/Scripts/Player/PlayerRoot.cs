using System;
using Enemies;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class PlayerRoot : MonoBehaviour, IPlayer, IDisposable
    {
        [Header("Camera Settings")] 
        [SerializeField] private Transform _cameraPivot;

        [Header("Look")] 
        [SerializeField] private float _turnSpeed = 10f;
        [SerializeField] private float _minVerticalAngle = -20f;
        [SerializeField] private float _maxVerticalAngle = 60f;
        private IPlayerInput _input;
        private PlayerLookComponent _look;
        private PlayerMovementComponent _movement;
        private PlayerAttackComponent _attack;
        public Vector3 Position => transform.position;

        [Inject]
        public void Construct(IPlayerInput input, PlayerConfig config, IEnemyCollection enemies,
            PlayerAttackComponent attack)
        {
            _input = input;
            _look = new PlayerLookComponent(transform, _cameraPivot, input.Look, _turnSpeed, _minVerticalAngle, _maxVerticalAngle);
            _movement = new PlayerMovementComponent(transform, config, input.Move);
            _attack = attack;
        }

        public void Dispose()
        {
            _look?.Dispose();
            _movement?.Dispose();
            _attack?.Dispose();
            _input?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}