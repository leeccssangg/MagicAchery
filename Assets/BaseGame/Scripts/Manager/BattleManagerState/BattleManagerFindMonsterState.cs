using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class BattleManagerFindMonsterState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(BattleManagerFindMonsterState state, CancellationToken ct);
        UniTask OnUpdate(BattleManagerFindMonsterState state, CancellationToken ct);
        UniTask OnExit(BattleManagerFindMonsterState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public BattleManagerFindMonsterState(IHandler owner)
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

public partial class BattleManager : BattleManagerFindMonsterState.IHandler
{
    private BattleManagerFindMonsterState m_FindMonsterState;
    public BattleManagerFindMonsterState FindMonsterState => m_FindMonsterState ??= new BattleManagerFindMonsterState(this);
    public async UniTask OnEnter(BattleManagerFindMonsterState state, CancellationToken ct)
    {
        CurrentFindingMonsterTime = FindingMonsterBaseTime;
        await UniTask.WaitUntil(Hero.IsInIdleState, cancellationToken: ct);
        Hero.StartFindMonster();
        IsSpawnMonster = false;
    }

    public UniTask OnUpdate(BattleManagerFindMonsterState state, CancellationToken ct)
    {
        float speedFindMonsterBonus = TalentTreeManager.GetTalentStat(TalentStat.Type.SpeedFindingMonster).Amount.ToFloat();
        CurrentFindingMonsterTime -= Time.deltaTime * (1 + speedFindMonsterBonus/100f);
        if (CurrentFindingMonsterTime < 0 && !IsSpawnMonster)
        {
            IsSpawnMonster = true;
            SpawnMonster();
        }

        if (IsAnyEnemyCanAttackedByHero())
        {
            StateMachine.RequestTransition(IdleState);
        }
        return UniTask.CompletedTask;
    }

    public  UniTask OnExit(BattleManagerFindMonsterState state, CancellationToken ct)
    {
        Hero.StartAttack();
        foreach (Monster monster in MonsterList)
        {
            monster.MoveToTargetPosition(Hero);
        }
        return UniTask.CompletedTask;
    }

    private bool IsAnyEnemyCanAttackedByHero()
    {
        foreach (Monster monster in MonsterList)
        {
            if (Vector3.Distance(monster.Transform.position, Hero.Transform.position) < StartBattleDistance)
            {
                return true;
            }
        }
        return false;
    }
}