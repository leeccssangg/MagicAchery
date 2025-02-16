using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class HeroSleepState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroSleepState state, CancellationToken ct);
        UniTask OnUpdate(HeroSleepState state, CancellationToken ct);
        UniTask OnExit(HeroSleepState state, CancellationToken ct);
    }

    private IHandler Owner { get; set; }

    public HeroSleepState(IHandler owner)
    {
        Owner = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return Owner.OnEnter(this, ct);
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return Owner.OnUpdate(this, ct);
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return Owner.OnExit(this, ct);
    }
}

public partial class Hero : HeroSleepState.IHandler
{
    private HeroSleepState m_SleepState;
    public HeroSleepState SleepState => m_SleepState ??= new HeroSleepState(this);

    public UniTask OnEnter(HeroSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(HeroSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExit(HeroSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}