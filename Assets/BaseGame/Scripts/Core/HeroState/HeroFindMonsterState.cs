using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroFindMonsterState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroFindMonsterState state, CancellationToken ct);
        UniTask OnUpdate(HeroFindMonsterState state, CancellationToken ct);
        UniTask OnExit(HeroFindMonsterState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public HeroFindMonsterState(IHandler owner)
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

public partial class Hero : HeroFindMonsterState.IHandler
{
    private HeroFindMonsterState m_FindMonsterState;
    public HeroFindMonsterState FindMonsterState => m_FindMonsterState ??= new HeroFindMonsterState(this);
    public async UniTask OnEnter(HeroFindMonsterState state, CancellationToken ct)
    {
        await HeroAnim.PlayRunAnimation(ct);
        IsRunning = true;
    }

    public UniTask OnUpdate(HeroFindMonsterState state, CancellationToken ct)
    {
        BattleManager.UpdateBackground(Time.deltaTime);
        return UniTask.CompletedTask;
    }

    public async UniTask OnExit(HeroFindMonsterState state, CancellationToken ct)
    {
        await HeroAnim.PlayIdleAnimation(ct);
        IsRunning = false;
    }
}