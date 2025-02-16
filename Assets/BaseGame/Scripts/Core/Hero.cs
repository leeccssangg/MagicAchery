using System;
using System.Threading;
using Core.AttributeAttackSpeed;
using Core.AttributeHeal;
using Core.AttributeHitPoint;
using Core.AttributeMagicalAttack;
using Core.AttributePhysicalAttack;
using Core.AttributeShield;
using Core.GameStatusEffect;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public partial class Hero : ACachedMonoBehaviour, IStatusEffectAble, IShield, IHeal, IHitPoint, IPhysicalAttack, IMagicalAttack, IAttackSpeed
{
    private FactoryManager FactoryManagerCache { get; set; }
    private FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
    private PlayerStatData PlayerStatDataCache { get; set; }
    private PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    private TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    [field: SerializeField] public StatusEffectStack StatusEffectStack {get; private set;}
    [field: SerializeField] private HeroAnim HeroAnim { get; set; }
    [field: SerializeField] private SortingGroup SortingGroup { get; set; }
    [field: SerializeField] private Arrow NormalArrow { get; set; }
    [field: SerializeField] private Arrow StrengthArrow { get; set; }
    [field: SerializeField] private Arrow MagicArrow { get; set; }
    [field: SerializeField] private Arrow FireArrow { get; set; }
    [field: SerializeField] private Arrow SpectralArrow { get; set; }
    [field: SerializeField] private Arrow ThunderArrow {get; set;}
    [field: SerializeField] private HeroIllusion HeroIllusion {get; set;}
    [field: SerializeField] private Transform ProjectileSpawnPosition { get; set; }
    [field: SerializeField] private StateMachine StateMachine { get; set; }

    [field: Title("Hero Stats")]
    #region HitPoint Function

    [field: SerializeField] public HitPoint HitPoint { get; set;}
    #endregion
    #region AttackSpeed Function
    
    [field: SerializeField] public AttackSpeed AttackSpeed { get; set; }
    
    #endregion
    #region PhysicalAttack Function

    [field: SerializeField] public PhysicalAttack PhysicalAttack {get; private set;}
    [field: SerializeField] public PhysicalCriticalChance PhysicalCriticalChance {get; private set;}
    [field: SerializeField] public PhysicalCriticalDamage PhysicalCriticalDamage {get; private set;}

    #endregion
    #region MagicalAttack Function

    [field: SerializeField] public MagicalAttack MagicalAttack {get; private set;}
    [field: SerializeField] public MagicalCriticalChance MagicalCriticalChance {get; private set;}
    [field: SerializeField] public MagicalCriticalDamage MagicalCriticalDamage {get; private set;}
    
    #endregion
    #region Shield Function

    [field: SerializeField] public Shield Shield { get; set; }
    [ShowInInspector] public BigNumber MaxShield => HitPoint.Max;
    public BigNumber CurrentShield => Shield.CurrentShield.Value;
    #endregion
    #region Heal Function

    [field: SerializeField] public Heal Heal { get; set; }

    public void TakeHeal(BigNumber heal)
    {
        HitPoint.ChangeHitPoint(heal);
    }

    public bool IsShielded()
    {
        return Shield.IsHavingShield();
    }

    public bool IsInCombat()
    {
        return StateMachine.CurrentState != FaintState && StateMachine.CurrentState != FindMonsterState;
    }

    #endregion
    public Action OnTakeDamageCallback { get; set; } = delegate { };
    private Monster[] MonsterArray { get; set; } = new Monster[20];
    private Monster TargetMonster { get; set; }
    public bool IsRunning { get; private set; }
    public bool IsFainted { get; private set; }

    private void Start()
    {
        InitStateMachine();
        InitStatusEffect();
        UpdateSortingOrder();
        
        HitPoint.Init(this);
        AttackSpeed.Init(this);
        PhysicalAttack.Init(this);
        MagicalAttack.Init(this);
        Shield.Init(this);
        Heal.Init(this);
    }

    private void OnDestroy()
    {
        StateMachine.Stop();
        StatusEffectStack.Stop();
    }

    private void InitStateMachine()
    {
        StateMachine = new StateMachine();
        StateMachine.RegisterState(SleepState);
        StateMachine.Run();
        StateMachine.RequestTransition(IdleState);
    }
    
    private void InitStatusEffect()
    {
        StatusEffectStack = new StatusEffectStack(this);
        StatusEffectStack.Run();
    }
    public bool IsInIdleState()
    {
        return StateMachine.CurrentState == IdleState;
    }

    public void StartAttack()
    {
        StateMachine.RequestTransition(IdleState);
    }

    public void StartFindMonster()
    {
        StateMachine.RequestTransition(FindMonsterState);
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
    private void UpdateSortingOrder()
    {
        SortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }
    
    public void OnHit(BigNumber damage)
    {
        if (IsFainted) return;
        BigNumber damageAfterShield = Shield.GetDamageAfterShield(damage);
        HitPoint.ChangeHitPoint(-damageAfterShield);
        if (HitPoint.IsExhausted)
        {
            IsFainted = true;
            StateMachine.RequestTransition(FaintState);
        }
        else if (damageAfterShield > 0.1f)
        {
            OnTakeDamage(damage);
        }
    }

    private void OnTakeDamage(BigNumber damage)
    {
        RegenWhenTakeDamage();
        TakeDamageGainShield();
        OnTakeDamageCallback?.Invoke();
    }

    private void TakeDamageGainShield()
    {
        float takeDamageGainShieldRate = TalentTreeManager.GetTalentStat(TalentStat.Type.TakeDamageGainShield).Amount.ToFloat();
        float rate = Random.Range(0, 100);
        if (takeDamageGainShieldRate > rate)
        {
            Shield.ChangeShieldPercent(0.5f);
        }
    }
    private void RegenWhenTakeDamage()
    {
        float takeDamageRegen = TalentTreeManager.GetTalentStat(TalentStat.Type.TakeDamageRegen).Amount.ToFloat();
        if (takeDamageRegen > 0.1f)
        {
            HitPoint.ChangeHitPoint(takeDamageRegen);
        }
    }
    public void OnHitMonster(Monster monster, BigNumber damage, DamageType damageType, bool isCritical)
    {
        RegenWhenDealDamage();
        RegenWhenDealDamagePercent();
        CriticalOverKill(monster,damageType, isCritical);
    }

    public void OnKillMonster()
    {
        RegenShieldWhenKill();
        RegenWhenKill();
    }
    private void RegenWhenDealDamage()
    {
        float dealDamageRegen = TalentTreeManager.GetTalentStat(TalentStat.Type.DealDamageRegen).Amount.ToFloat();
        if (dealDamageRegen > 0.1f)
        {
            HitPoint.ChangeHitPoint(dealDamageRegen);
        }
    }
    private void RegenWhenDealDamagePercent()
    {
        float dealDamageRegenPercent = TalentTreeManager.GetTalentStat(TalentStat.Type.DealDamageRegenPercent).Amount.ToFloat();
        if (dealDamageRegenPercent > 0.1f)
        {
            HitPoint.ChangeHitPointPercent(dealDamageRegenPercent/100f);
        }
    }

    private void CriticalOverKill(Monster monster, DamageType damageType, bool isCritical)
    {
        if (damageType != DamageType.Physical || !isCritical || Random.Range(0, 100) >= 10) return;
        BigNumber criticalOverKillLevel = TalentTreeManager.GetTalentStat(TalentStat.Type.PhysicalCriticalOverkillDamage).Amount;
        if (criticalOverKillLevel == 0) return;
        BigNumber pureDamage = monster.MaxHitPoint * criticalOverKillLevel / 100f;
        monster.OnHit(pureDamage, DamageType.Pure, true);
    }
    private void RegenShieldWhenKill()
    {
        float killGainShield = TalentTreeManager.GetTalentStat(TalentStat.Type.KillGainShield).Amount.ToFloat();
        if (killGainShield > 0.1f)
        {
            Shield.ChangeShieldPercent(killGainShield/100f);
        }
    }
    private void RegenWhenKill()
    {
        float killRegen = TalentTreeManager.GetTalentStat(TalentStat.Type.KillRegen).Amount.ToFloat();
        if (killRegen > 0.1f)
        {
            HitPoint.ChangeHitPoint(killRegen);
        }
    }
    

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectStack.Add(statusEffect);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectStack.Remove(statusEffect);
    }

}