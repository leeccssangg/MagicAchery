using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class MonsterWaitState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterWaitState state, CancellationToken ct);
        UniTask OnUpdate(MonsterWaitState state, CancellationToken ct);
        UniTask OnExit(MonsterWaitState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterWaitState(IHandler owner)
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

public partial class Monster : MonsterWaitState.IHandler
{
    private MonsterWaitState m_WaitState;
    public MonsterWaitState WaitState => m_WaitState ??= new MonsterWaitState(this);
    public UniTask OnEnter(MonsterWaitState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterWaitState state, CancellationToken ct)
    {
        if (!Hero.IsFainted)
        {
            StateMachine.RequestTransition(MoveToHeroState);
        }
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterWaitState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

}