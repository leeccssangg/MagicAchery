using System.Threading;
using Cysharp.Threading.Tasks;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;

public class MonsterAttackState : IState
{
    public interface IHandler
    {
        UniTask OnEnter(MonsterAttackState state, CancellationToken ct);
        UniTask OnUpdate(MonsterAttackState state, CancellationToken ct);
        UniTask OnExit(MonsterAttackState state, CancellationToken ct);
    }
    private IHandler Owner { get; set; }
    public MonsterAttackState(IHandler owner)
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

public partial class Monster : MonsterAttackState.IHandler
{
    private MonsterAttackState m_AttackState;
    public MonsterAttackState AttackState => m_AttackState ??= new MonsterAttackState(this);
    public UniTask OnEnter(MonsterAttackState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }

    public async UniTask OnUpdate(MonsterAttackState state, CancellationToken ct)
    {
        MonsterAnim.PlayAttackAnimation(ct);
        await UniTask.Delay((int)(AttackHitDelay/ AttackSpeed), cancellationToken: ct);
        OnAttackHit();
        await UniTask.Delay((int)(1000 - AttackHitDelay/ AttackSpeed), cancellationToken: ct);
        if (Hero.IsFainted)
        {
            StateMachine.RequestTransition(MoveToWaitStateState);
        }
    }

    public UniTask OnExit(MonsterAttackState state, CancellationToken ct)
    {
        return UniTask.CompletedTask;
    }
    private void OnAttackHit()
    {
        Hero.OnHit(AttackDamage);
    }
}