using System;
using Core.SimplePool;
using LitMotion;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Rendering;

public partial class HeroIllusion : ACachedMonoBehaviour, IPoolAble<HeroIllusion>
{
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
    
    private Hero Hero => BattleManager.Hero;
    
    [field: SerializeField] private HeroAnim HeroAnim { get; set; }
    [field: SerializeField] private SortingGroup SortingGroup {get; set;}
    [field: SerializeField] private Transform ProjectileSpawnPosition {get; set;}
    [field: SerializeField] private StateMachine StateMachine {get; set;}
    private Vector3 StartPosition { get; set; }
    private Vector3 EndPosition { get; set; }
    private MotionHandle MotionHandle { get; set; }
    private Monster[] MonsterArray { get; set; } = new Monster[20];
    private Monster TargetMonster { get; set; }
    private BigNumber FireRate { get; set; }
    private Arrow Arrow { get; set; }
    private bool IsMultiTarget { get; set; }
    private int  AttackCount { get; set; }
    private void InitStateMachine()
    {
        StateMachine = new StateMachine();
        StateMachine.RegisterState(SleepState);
        StateMachine.Run();
        StateMachine.RequestTransition(MoveState);
    }
    public HeroIllusion WithFireRate(BigNumber fireRate)
    {
        FireRate = fireRate;
        return this;
    }
    public HeroIllusion WithArrow(Arrow arrow)
    {
        Arrow = arrow;
        return this;
    }
    public HeroIllusion WithMultiTarget(bool isMultiTaget)
    {
        IsMultiTarget = isMultiTaget;
        return this;
    }
    public HeroIllusion WithAttackCount(int attackCount)
    {
        AttackCount = attackCount;
        return this;
    }
    private void UpdateSortingOrder()
    {
        SortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }
    private bool TryGetTargetMonster(out Monster monster)
    {
        int count = BattleManager.GetMonsterNonAlloc(MonsterArray);
        if (count == 0)
        {
            monster = null;
            return false;
        }
        monster = MonsterArray[0];
        for (int i = 0; i < count; i++)
        {
            if (Vector3.Distance(MonsterArray[i].Transform.position, transform.position) <
                Vector3.Distance(monster.Transform.position, transform.position))
            {
                monster = MonsterArray[i];
            }
        }
        return true;
    }
    private bool TryGetMultiTargetMonster(out Monster[] monster)
    {
        int count = BattleManager.GetMonsterNonAlloc(MonsterArray);
        if (count == 0)
        {
            monster = Array.Empty<Monster>();
            return false;
        }
        monster = new Monster[count];
        for (int i = 0; i < count; i++)
        {
            monster[i] = MonsterArray[i];
        }
        return true;
    }
    public HeroIllusion OnSpawn()
    {
        InitStateMachine();
        return this;
    }

    public void OnDespawn()
    {
        StateMachine.Stop();        
    }
}