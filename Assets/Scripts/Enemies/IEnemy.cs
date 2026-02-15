using System;
using UniRx;
using UnityEngine;

namespace Enemies
{
    public interface IEnemy : IDamageable
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
        IObservable<Unit> OnDeath { get; }
    }
}