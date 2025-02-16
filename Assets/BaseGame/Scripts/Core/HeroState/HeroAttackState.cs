using System;
using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeroAttackState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroAttackState state, CancellationToken ct);
        UniTask OnUpdate(HeroAttackState state, CancellationToken ct);
        UniTask OnExit(HeroAttackState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public HeroAttackState(IHandler owner)
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

public partial class Hero : HeroAttackState.IHandler
{
    private HeroAttackState m_AttackState;
    public HeroAttackState AttackState => m_AttackState ??= new HeroAttackState(this);
    public async UniTask OnEnter(HeroAttackState state, CancellationToken ct)
    {
        HeroAnim.ChangeRootTarget(TargetMonster.HitPosition, 0.2f);
        await HeroAnim.PlayStartAttackAnimation(ct);
    }

    public async UniTask OnUpdate(HeroAttackState state, CancellationToken ct)
    {
        if (TargetMonster.IsDead || TargetMonster.IsFutureDead)
        {
            if (TryGetTargetMonster(out Monster monster))
            {
                TargetMonster = monster;

            }
            else
            {
                StateMachine.RequestTransition(IdleState);
            }
        }
        
        if (!TargetMonster.IsDead && !TargetMonster.IsFutureDead)
        {
            SpawnStrengthIllusion();
            SpawnSpiritIllusion();
            SpawnFireIllusion();
            SpawnSpectralIllusion();
            SpawnThunderIllusion();
            HeroAnim.ChangeRootTarget(TargetMonster.HitPosition, 0.2f);
            await HeroAnim.PlayAttackAnimation(AttackSpeed.Current, TargetMonster, SpawnNormalArrowCache, ct);
        }
    }

    public async UniTask OnExit(HeroAttackState state, CancellationToken ct)
    {   
        HeroAnim.ChangeRootTarget(null, 0.2f);
        await HeroAnim.PlayEndAttackAnimation(ct);
    }
    
    [ACacheMethod]
    private void SpawnNormalArrow(Monster monster)
    {
        NormalArrow.Spawn(ProjectileSpawnPosition.position, Quaternion.identity)
            .Setup(this, ProjectileSpawnPosition.position, monster)
            .Launch();
    }
    private void SpawnStrengthIllusion()
    {
        BigNumber rate = TalentTreeManager.GetTalentStat(TalentStat.Type.StrengthArrowRate).Amount;
        if (Random.Range(0,100) >= rate) return;
        
        HeroIllusion.Spawn(Transform.position, Quaternion.identity)
            .WithFireRate(AttackSpeed.Current)
            .WithArrow(StrengthArrow)
            .WithMultiTarget(false)
            .WithAttackCount(1);
    }
    private void SpawnSpiritIllusion()
    {
        BigNumber rate = TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowRate).Amount;
        if (Random.Range(0,100) >= rate) return;
        bool isMultiTarget = Random.Range(0, 100) < TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowMulti).Amount;
        //bool isMultiTarget = true;
        int attackCount = Random.Range(0, 100) < TalentTreeManager.GetTalentStat(TalentStat.Type.SpiritArrowDoubleShot).Amount ? 2 : 1;
        //int attackCount = 2;
        HeroIllusion.Spawn(Transform.position, Quaternion.identity)
            .WithFireRate(AttackSpeed.Current)
            .WithArrow(MagicArrow)
            .WithMultiTarget(isMultiTarget)
            .WithAttackCount(attackCount);
    }
    
    private void SpawnFireIllusion()
    {
        BigNumber rate = TalentTreeManager.GetTalentStat(TalentStat.Type.FireArrowRate).Amount;
        if (Random.Range(0,100) >= rate) return;
        bool isMultiTarget = Random.Range(0, 100) < TalentTreeManager.GetTalentStat(TalentStat.Type.FireArrowMulti).Amount;
        
        HeroIllusion.Spawn(Transform.position, Quaternion.identity)
            .WithFireRate(AttackSpeed.Current)
            .WithArrow(FireArrow)
            .WithMultiTarget(isMultiTarget)
            .WithAttackCount(1);
    }
    
    private void SpawnSpectralIllusion()
    {
        BigNumber rate = TalentTreeManager.GetTalentStat(TalentStat.Type.SpectralArrowRate).Amount;
        if (Random.Range(0,100) >= rate) return;
        int attackCount = Random.Range(0, 100) < TalentTreeManager.GetTalentStat(TalentStat.Type.SpectralArrowTripleShoot).Amount ? 3 : 1;
        HeroIllusion.Spawn(Transform.position, Quaternion.identity)
            .WithFireRate(AttackSpeed.Current)
            .WithArrow(SpectralArrow)
            .WithMultiTarget(false)
            .WithAttackCount(attackCount);
    }
    
    private void SpawnThunderIllusion()
    {
        BigNumber rate = TalentTreeManager.GetTalentStat(TalentStat.Type.ThunderArrowRate).Amount;
        if (Random.Range(0,100) >= rate) return;
        
        HeroIllusion.Spawn(Transform.position, Quaternion.identity)
            .WithFireRate(AttackSpeed.Current)
            .WithArrow(ThunderArrow)
            .WithMultiTarget(false)
            .WithAttackCount(1);
    }
}