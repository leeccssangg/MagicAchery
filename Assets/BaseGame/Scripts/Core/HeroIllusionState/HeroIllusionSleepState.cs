using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class HeroIllusionSleepState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroIllusionSleepState state, CancellationToken ct);
        UniTask OnUpdate(HeroIllusionSleepState state, CancellationToken ct);
        UniTask OnExit(HeroIllusionSleepState state, CancellationToken ct);
    }

    private IHandler Owner { get; set; }

    public HeroIllusionSleepState(IHandler owner)
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

public partial class HeroIllusion : HeroIllusionSleepState.IHandler
{
    private HeroIllusionSleepState m_SleepState;
    public HeroIllusionSleepState SleepState => m_SleepState ??= new HeroIllusionSleepState(this);

    public UniTask OnEnter(HeroIllusionSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(HeroIllusionSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExit(HeroIllusionSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}