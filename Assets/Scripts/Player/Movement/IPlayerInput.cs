using System;
using UnityEngine;

namespace Player
{
    public interface IPlayerInput : IDisposable
    {
        IObservable<Vector2> Move { get; }
        IObservable<Vector2> Look { get; }
        IObservable<bool> IsMoving { get; }
    }
}