using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroIllusionMoveState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroIllusionMoveState state, CancellationToken ct);
        UniTask OnUpdate(HeroIllusionMoveState state, CancellationToken ct);
        UniTask OnExit(HeroIllusionMoveState state, CancellationToken ct);
    }

    private IHandler Owner { get; set; }

    public HeroIllusionMoveState(IHandler owner)
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

public partial class HeroIllusion : HeroIllusionMoveState.IHandler
{
    private HeroIllusionMoveState m_MoveState;
    public HeroIllusionMoveState MoveState => m_MoveState ??= new HeroIllusionMoveState(this);

    public UniTask OnEnter(HeroIllusionMoveState state, CancellationToken ct)
    {
        StartPosition = Hero.Transform.position;
        EndPosition = StartPosition + new Vector3(Random.Range(-2f, -1f), Random.Range(-2f, 2f), 0);
        MotionHandle = LMotion.Create(StartPosition ,EndPosition, 0.2f)
            .WithEase(Ease.OutSine)
            .WithOnComplete(OnMoveCompleteCache)
            .Bind(OnMoveUpdateCache);
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(HeroIllusionMoveState state, CancellationToken ct)
    {

        return UniTask.CompletedTask;
    }

    public UniTask OnExit(HeroIllusionMoveState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
    [ACacheMethod]
    private void OnMoveUpdate(UnityEngine.Vector3 position)
    {
        Transform.position = position;
        UpdateSortingOrder();
    }
    [ACacheMethod]
    private void OnMoveComplete()
    {
        Transform.position = EndPosition;
        StateMachine.RequestTransition(AttackState);
    }
}