using System.Threading;
using Core.GameStatusEffect;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroFaintState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroFaintState state, CancellationToken ct);
        UniTask OnUpdate(HeroFaintState state, CancellationToken ct);
        UniTask OnExit(HeroFaintState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public HeroFaintState(IHandler owner)
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

public partial class Hero : HeroFaintState.IHandler
{
    private HeroFaintState m_FaintState;
    public HeroFaintState FaintState => m_FaintState ??= new HeroFaintState(this);
    public async UniTask OnEnter(HeroFaintState state, CancellationToken ct)
    {
        await HeroAnim.PlayDownAnimation(ct);
        HitPoint.SetCurrentHitPoint(0);
    }

    public UniTask OnUpdate(HeroFaintState state, CancellationToken ct)
    {
        RecoveryOnFaint();
        return UniTask.CompletedTask;
    }

    public async UniTask OnExit(HeroFaintState state, CancellationToken ct)
    {
        await HeroAnim.PlayUpAnimation(ct);
        IsFainted = false;
        AddAttackSpeedAfterRecovery();
        AddShieldAfterRecovery();
    }
    private void RecoveryOnFaint()
    {
        HitPoint.Recover(Time.deltaTime);
        if (HitPoint.IsFull)
        {
            StateMachine.RequestTransition(IdleState);
        }
    }
    private void AddShieldAfterRecovery()
    {
        float recoveryShield = TalentTreeManager.GetTalentStat(TalentStat.Type.RecoveryShield).Amount.ToFloat();
        if (recoveryShield < 0.1f) return;
        Shield.ChangeShieldPercent(recoveryShield/100f);
    }
    private void AddAttackSpeedAfterRecovery()
    {
        float attackSpeedBuff = TalentTreeManager.GetTalentStat(TalentStat.Type.AttackSpeedBoostAfterRecovery).Amount.ToFloat();
        if (attackSpeedBuff < 0.1f) return;
        AddStatusEffect(new AttackSpeedChangeStatusEffect(5, attackSpeedBuff));
    }
}