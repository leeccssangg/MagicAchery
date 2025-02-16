using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class MonsterIdleState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterIdleState state, CancellationToken ct);
        UniTask OnUpdate(MonsterIdleState state, CancellationToken ct);
        UniTask OnExit(MonsterIdleState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterIdleState(IHandler owner)
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

public partial class Monster : MonsterIdleState.IHandler
{
    private MonsterIdleState m_IdleState;
    public MonsterIdleState IdleState => m_IdleState ??= new MonsterIdleState(this);
    public UniTask OnEnter(MonsterIdleState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterIdleState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterIdleState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}