using UniRx;

namespace UI
{
    public interface IKillCounterController
    {
        IReadOnlyReactiveProperty<int> KillCount { get; }
    }
}
