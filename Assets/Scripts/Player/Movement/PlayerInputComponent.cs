using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputComponent : IPlayerInput
    {
        private readonly InputActionMap _map;
        private readonly InputAction _moveAction;
        private readonly InputAction _lookAction;
        private readonly CompositeDisposable _disposables = new();
        public IObservable<Vector2> Move { get; }
        public IObservable<Vector2> Look { get; }
        public IObservable<bool> IsMoving { get; }

        public PlayerInputComponent(InputActionAsset actions, string actionMapName, string moveActionName,
            string lookActionName)
        {
            _map = actions.FindActionMap(actionMapName, true);
            _moveAction = _map.FindAction(moveActionName, true);
            _lookAction = _map.FindAction(lookActionName, true);
            
            Move = CreateVector2Stream(_moveAction).Publish().RefCount();
            Look = CreateVector2Stream(_lookAction).Publish().RefCount();
            
            _map.devices = new InputDevice[] { Keyboard.current, Mouse.current };
            
            IsMoving = Move.Select(v => v.sqrMagnitude > 0.0001f).DistinctUntilChanged().Publish().RefCount();
            _map.Enable();
        }

        private IObservable<Vector2> CreateVector2Stream(InputAction action)
        {
            return Observable.EveryUpdate().Select(_ => action.ReadValue<Vector2>()).DistinctUntilChanged();
        }

        public void Dispose()
        {
            _map.Disable();
            _disposables.Dispose();
        }
    }
}