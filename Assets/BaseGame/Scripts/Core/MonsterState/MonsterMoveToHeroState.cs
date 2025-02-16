using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;
using UnityEngine;

public class MonsterMoveToHeroState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterMoveToHeroState toHeroState, CancellationToken ct);
        UniTask OnUpdate(MonsterMoveToHeroState toHeroState, CancellationToken ct);
        UniTask OnExit(MonsterMoveToHeroState toHeroState, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterMoveToHeroState(IHandler owner)
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

public partial class Monster : MonsterMoveToHeroState.IHandler
{
    private MonsterMoveToHeroState m_MoveToHeroState;
    public MonsterMoveToHeroState MoveToHeroState => m_MoveToHeroState ??= new MonsterMoveToHeroState(this);
    public UniTask OnEnter(MonsterMoveToHeroState toHeroState, CancellationToken ct)
    {
        MonsterAnim.PlayRunAnimation();
        MonsterAnim.UpdateFlip(Hero.Transform.position);
        TargetPosition = Hero.Transform.position + Random.insideUnitSphere * 0.5f;
        MoveDirection = (TargetPosition - Transform.position).normalized;
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterMoveToHeroState toHeroState, CancellationToken ct)
    {
        float distance = Vector3.Distance(TargetPosition, Transform.position);
        if (distance < AttackRange)
        {
            OnMoveToHeroComplete();
        }
        else
        {
            OnMoveToHeroUpdate();
        }
        
        if (Hero.IsFainted)
        {
            StateMachine.RequestTransition(MoveToWaitStateState);
        }
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterMoveToHeroState toHeroState, CancellationToken ct)
    {
        MonsterAnim.PlayIdleAnimation();
        return UniTask.CompletedTask;
    }
    private void OnMoveToHeroUpdate()
    {
        Transform.position += MoveDirection * MovementSpeed * Time.deltaTime;
        UpdateSortingOrder();
    }
    private void OnMoveToHeroComplete()
    {
        StateMachine.RequestTransition(AttackState);
    }
}