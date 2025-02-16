using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class MonsterSleepState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterSleepState state, CancellationToken ct);
        UniTask OnUpdate(MonsterSleepState state, CancellationToken ct);
        UniTask OnExit(MonsterSleepState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterSleepState(IHandler owner)
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

public partial class Monster : MonsterSleepState.IHandler
{
    private MonsterSleepState m_SleepState;
    public MonsterSleepState SleepState => m_SleepState ??= new MonsterSleepState(this);
    public UniTask OnEnter(MonsterSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterSleepState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}