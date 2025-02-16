using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class MonsterMoveToWaitStateState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterMoveToWaitStateState state, CancellationToken ct);
        UniTask OnUpdate(MonsterMoveToWaitStateState state, CancellationToken ct);
        UniTask OnExit(MonsterMoveToWaitStateState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterMoveToWaitStateState(IHandler owner)
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

public partial class Monster : MonsterMoveToWaitStateState.IHandler
{
    private MonsterMoveToWaitStateState m_MoveToWaitStateState;
    public MonsterMoveToWaitStateState MoveToWaitStateState => m_MoveToWaitStateState ??= new MonsterMoveToWaitStateState(this);
    public UniTask OnEnter(MonsterMoveToWaitStateState state, CancellationToken ct)
    {
        MonsterAnim.PlayRunAnimation();
        MonsterAnim.UpdateFlip(BattleManager.WaitPosition.position);
        TargetPosition = BattleManager.WaitPosition.position + Random.insideUnitSphere * 3;
        MoveDirection = (TargetPosition - Transform.position).normalized;
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterMoveToWaitStateState state, CancellationToken ct)
    {
        float distance = Vector3.Distance(TargetPosition, Transform.position);
        if (distance < 1f)
        {
            OnMoveToWaitPositionComplete();
        }
        else
        {
            OnMoveToWaitPositionUpdate();
        }

        if (!Hero.IsFainted)
        {
            StateMachine.RequestTransition(MoveToHeroState);
        }
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterMoveToWaitStateState state, CancellationToken ct)
    {
        MonsterAnim.PlayIdleAnimation();
        return UniTask.CompletedTask;
    }
    private void OnMoveToWaitPositionUpdate()
    {
        Transform.position += MoveDirection * MovementSpeed * Time.deltaTime;
        UpdateSortingOrder();
    }
    private void OnMoveToWaitPositionComplete()
    {
        StateMachine.RequestTransition(WaitState);
    }
}