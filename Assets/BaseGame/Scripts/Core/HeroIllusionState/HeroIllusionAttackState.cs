using System.Threading;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;
using UnityEngine;

public class HeroIllusionAttackState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(HeroIllusionAttackState state, CancellationToken ct);
        UniTask OnUpdate(HeroIllusionAttackState state, CancellationToken ct);
        UniTask OnExit(HeroIllusionAttackState state, CancellationToken ct);
    }

    private IHandler Owner { get; set; }

    public HeroIllusionAttackState(IHandler owner)
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

public partial class HeroIllusion : HeroIllusionAttackState.IHandler
{
    private HeroIllusionAttackState m_AttackState;
    public HeroIllusionAttackState AttackState => m_AttackState ??= new HeroIllusionAttackState(this);

    public UniTask OnEnter(HeroIllusionAttackState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public async UniTask OnUpdate(HeroIllusionAttackState state, CancellationToken ct)
    {
        for (int i = 0; i < AttackCount; i++)
        {
            await Attack(ct);
        }

        this.Despawn();
        StateMachine.RequestTransition(SleepState);
    }

    public UniTask OnExit(HeroIllusionAttackState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
    [ACacheMethod]
    private void SpawnArrow(Monster monster)
    {
        if (monster.IsDead || monster.IsFutureDead)
        {
            if (TryGetTargetMonster(out Monster newMonster))
            {
                Arrow.Spawn(ProjectileSpawnPosition.position, Quaternion.identity)
                    .Setup(Hero, ProjectileSpawnPosition.position, newMonster)
                    .Launch();
            }
        }
        else
        {
            Arrow.Spawn(ProjectileSpawnPosition.position, Quaternion.identity)
                .Setup(Hero, ProjectileSpawnPosition.position, monster)
                .Launch();
        }
    }
    private async UniTask Attack(CancellationToken ct)
    {
        if (IsMultiTarget && TryGetMultiTargetMonster(out Monster[] monsters))
        {
            await HeroAnim.PlayAttackAnimation(FireRate.ToFloat(), monsters, SpawnMultiArrowCache, ct);
        }
        
        if (!IsMultiTarget && TryGetTargetMonster(out Monster monster))
        {
            TargetMonster = monster;
            HeroAnim.ChangeRootTarget(TargetMonster.HitPosition, 0.2f);
            await HeroAnim.PlayAttackAnimation(FireRate.ToFloat(), TargetMonster, SpawnArrowCache, ct);
        }
    }
    [ACacheMethod]
    private void SpawnMultiArrow(Monster[] monsters)
    {
        foreach (Monster monster in monsters)
        {
            if (monster.IsDead || monster.IsFutureDead) continue;
            Arrow.Spawn(ProjectileSpawnPosition.position, Quaternion.identity)
                .Setup(Hero, ProjectileSpawnPosition.position, monster)
                .Launch();
        }
    }
}