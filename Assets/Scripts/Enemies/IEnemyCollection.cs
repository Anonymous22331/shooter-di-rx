using System;
using System.Collections.Generic;
using UniRx;

namespace Enemies
{
    public interface IEnemyCollection
    {
        IReadOnlyList<IEnemy> AllEnemies { get; }

        IObservable<IEnemy> OnEnemyDeactivatedByLimit { get; }
        IObservable<Unit> OnFreeSlotAppeared { get; }
        IObservable<IEnemy> OnEnemyDied { get; }

        bool TryClaimFreeSlot(IEnemy enemy);
        void ConfirmRespawned(IEnemy enemy);

        void Register(IEnemy enemy);
        void Unregister(IEnemy enemy);
    }

}