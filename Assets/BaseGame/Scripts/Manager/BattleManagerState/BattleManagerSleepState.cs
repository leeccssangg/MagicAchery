using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class BattleManagerSleepState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(BattleManagerSleepState state, CancellationToken ct);
        UniTask OnUpdate(BattleManagerSleepState state, CancellationToken ct);
        UniTask OnExit(BattleManagerSleepState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public BattleManagerSleepState(IHandler owner)
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

public partial class BattleManager : BattleManagerSleepState.IHandler
{
    private BattleManagerSleepState m_SleepState;
    public BattleManagerSleepState SleepState => m_SleepState ??= new BattleManagerSleepState(this);
    public UniTask OnEnter(BattleManagerSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(BattleManagerSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(BattleManagerSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}
