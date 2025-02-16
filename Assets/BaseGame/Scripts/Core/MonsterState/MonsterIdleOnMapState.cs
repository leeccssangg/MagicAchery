using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class MonsterIdleOnMapState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterIdleOnMapState state, CancellationToken ct);
        UniTask OnUpdate(MonsterIdleOnMapState state, CancellationToken ct);
        UniTask OnExit(MonsterIdleOnMapState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterIdleOnMapState(IHandler owner)
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

public partial class Monster : MonsterIdleOnMapState.IHandler
{
    private MonsterIdleOnMapState m_IdleOnMapState;
    public MonsterIdleOnMapState IdleOnMapState => m_IdleOnMapState ??= new MonsterIdleOnMapState(this);
    public UniTask OnEnter(MonsterIdleOnMapState state, CancellationToken ct)
    {
        UpdateSortingOrder();
        MonsterAnim.PlayIdleAnimation();
        BattleManager.OnUpdateMap += OnUpdateMap;
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdate(MonsterIdleOnMapState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(MonsterIdleOnMapState state, CancellationToken ct)
    {
        BattleManager.OnUpdateMap -= OnUpdateMap;
        return UniTask.CompletedTask;
    }
    private void OnUpdateMap(float mapMoveDistance) 
    {
        Transform.position += Vector3.left * mapMoveDistance;
    }
}