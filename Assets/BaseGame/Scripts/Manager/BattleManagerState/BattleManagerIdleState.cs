using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

public class BattleManagerIdleState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(BattleManagerIdleState state, CancellationToken ct);
        UniTask OnUpdate(BattleManagerIdleState state, CancellationToken ct);
        UniTask OnExit(BattleManagerIdleState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public BattleManagerIdleState(IHandler owner)
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

public partial class BattleManager : BattleManagerIdleState.IHandler
{
    private BattleManagerIdleState m_IdleState;
    public BattleManagerIdleState IdleState => m_IdleState ??= new BattleManagerIdleState(this);
    public UniTask OnEnter(BattleManagerIdleState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(BattleManagerIdleState state, CancellationToken ct)
    {
        if (MonsterList.Count == 0) 
        {
            StateMachine.RequestTransition(FindMonsterState);
        }
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(BattleManagerIdleState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
}