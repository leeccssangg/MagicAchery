using Core.GameStatusEffect;
using Core.SimplePool;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using R3;
using TW.Utility.DesignPattern;
using UnityEngine.Rendering;

public partial class Monster : ACachedMonoBehaviour, IPoolAble<Monster>, IStatusEffectAble, IBurnAble, IParalysisAble, IPoisonAble
{
    public enum Type
    {
        Normal,
        Rare,
        Epic,
        Boss,
    }
    public Transform HitTransform => HitPosition;
    private BattleManager BattleManagerCache { get; set; }
    private BattleManager BattleManager => BattleManagerCache ??= BattleManager.Instance;
    private FactoryManager FactoryManagerCache { get; set; }
    private FactoryManager FactoryManager => FactoryManagerCache ??= FactoryManager.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    private TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    [field: SerializeField] public StatusEffectStack StatusEffectStack {get; private set;}
    [field: SerializeField] public Type MonsterType {get; private set;}
    [field: SerializeField] private SortingGroup SortingGroup {get; set;}
    [field: SerializeField] private MonsterAnim MonsterAnim {get; set;}
    [field: SerializeField] private float MovementSpeed {get; set;}
    [field: SerializeField] private float AttackRange {get; set;}
    [field: SerializeField] public Transform HitPosition {get; private set;}
    [field: SerializeField] private ProgressBar HitPointBar {get; set;}
    [field: SerializeField] private int AttackHitDelay {get; set;}
    [field: SerializeField] private BigNumber AttackDamage {get; set;}
    [field: SerializeField] private float AttackSpeed {get; set;}
    [field: SerializeField] private ReactiveValue<BigNumber> HitPoint {get; set;} = new(0);
    [field: SerializeField] private BigNumber FutureHitPoint {get; set;} = new(0);
    [field: SerializeField] public BigNumber MaxHitPoint {get; private set;} = new(0);
    [field: SerializeField] private BigNumber Experience {get; set;} = new(0);
    [field: SerializeField] private StateMachine StateMachine {get; set;}
    private GameResource[] ResourceDrop { get; set;}
    private Hero Hero {get; set;}
    private Vector3 TargetPosition { get; set; }
    private Vector3 MoveDirection { get; set; }
    public bool IsDead => HitPoint.Value <= 0;
    public bool IsFutureDead => FutureHitPoint <= 0;

    public int ParalysisStack { get; set; }
    private void Awake()
    {
        HitPoint.ReactiveProperty.Subscribe(OnHitPointChange).AddTo(this);
    }

    public Monster InitStat(BigNumber damage, float attackSpeed, BigNumber hitPoint, BigNumber experience)
    {
        AttackDamage = damage;
        AttackSpeed = attackSpeed;
        HitPoint.Value = hitPoint;
        FutureHitPoint = hitPoint;
        MaxHitPoint = hitPoint;
        Experience = experience;
        return this;
    }
    public Monster InitStat(MonsterConfig monsterConfig)
    {
        AttackDamage = monsterConfig.AttackDamage;
        AttackSpeed = monsterConfig.AttackSpeed;
        MovementSpeed = monsterConfig.MovementSpeed;
        HitPoint.Value = monsterConfig.HitPoint;
        FutureHitPoint = monsterConfig.HitPoint;
        MaxHitPoint = monsterConfig.HitPoint;
        Experience = monsterConfig.Experience;
        ResourceDrop = monsterConfig.ResourceDrop;
        return this;
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
        StateMachine.RequestTransition(IdleOnMapState);
    }
    private void InitStatusEffect()
    {
        StatusEffectStack = new StatusEffectStack(this);
        StatusEffectStack.Run();
    }
    public Monster MoveToTargetPosition(Hero hero)
    {
        Hero = hero;
        StateMachine.RequestTransition(MoveToHeroState);
        return this;
    }
    public Monster WillHit(BigNumber damage)
    {
        FutureHitPoint -= damage;
        return this;
    }
    public Monster OnHit(BigNumber damage, DamageType damageType, bool isCritical)
    {
        HitPoint.Value -= damage;
        FactoryManager.SpawnDamageText(damage, damageType, HitPosition.position, isCritical);
        Hero.OnHitMonster(this, damage, damageType, isCritical);
        if (HitPoint.Value <= 0)
        {
            OnDeath();
            return this;
        }
        return this;
    }
    public float CurrentHitPointPercent()
    {
        return (HitPoint.Value / MaxHitPoint).ToFloat();
    }
    public Monster InstanceDie()
    {
        HitPoint.Value -= HitPoint.Value + 1;
        OnDeath();
        return this;
    }

    private void OnDeath()
    {
        DropResource();
        DropExperience();
        Hero.OnKillMonster();
        this.Despawn();
    }
    private void DropExperience()
    {
        BigNumber bonusExperience = GetBonusExperience(MonsterType);
        BigNumber experience = Experience * (1 + bonusExperience/100f);
        BattleManager.GainExperience(experience, HitPosition.position);
    }
    private void DropResource()
    {
        BigNumber resourceDrop = GetBonusResource(MonsterType);
        bool isDoubleDrop = Random.Range(0, 100) < TalentTreeManager.GetTalentStat(TalentStat.Type.DoubleResourceDropRate).Amount;
        for (int i = 0; i < ResourceDrop.Length; i++)
        {
            BigNumber amount = (ResourceDrop[i].Amount * (1 + resourceDrop/100f)).RoundToInt();
            if (isDoubleDrop)
            {
                amount *= 2;
            }
            PlayerResourceData.Instance.AddGameResource(ResourceDrop[i].ResourceType, amount);
        }
    }
    private void OnHitPointChange(BigNumber hitPoint)
    {
        float progress = (hitPoint / MaxHitPoint).ToFloat();
        HitPointBar.SetProgress(progress);
    }
    private void UpdateSortingOrder()
    {
        SortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }

    private BigNumber GetBonusExperience(Type monsterType)
    {
        return monsterType switch
        {
            Type.Normal => TalentTreeManager.GetTalentStat(TalentStat.Type.NormalEnemyExpDrop).Amount,
            Type.Rare => TalentTreeManager.GetTalentStat(TalentStat.Type.RareEnemyExpDrop).Amount,
            Type.Epic => TalentTreeManager.GetTalentStat(TalentStat.Type.EpicEnemyExpDrop).Amount,
            Type.Boss => TalentTreeManager.GetTalentStat(TalentStat.Type.BossEnemyExpDrop).Amount,
            _ => 0
        };
    }
    private BigNumber GetBonusResource(Type monsterType)
    {
        return monsterType switch
        {
            Type.Normal => TalentTreeManager.GetTalentStat(TalentStat.Type.NormalEnemyResourceDrop).Amount,
            Type.Rare => TalentTreeManager.GetTalentStat(TalentStat.Type.RareEnemyResourceDrop).Amount,
            Type.Epic => TalentTreeManager.GetTalentStat(TalentStat.Type.EpicEnemyResourceDrop).Amount,
            Type.Boss => TalentTreeManager.GetTalentStat(TalentStat.Type.BossEnemyResourceDrop).Amount,
            _ => 0
        };
    }
    public Monster OnSpawn()
    {
        BattleManager.AddMonster(this);
        InitStateMachine();
        InitStatusEffect();

        return this;
    }
    public void OnDespawn()
    {
        BattleManager.RemoveMonster(this);
        StateMachine.Stop();
        StatusEffectStack.Stop();
    }

    public void AddStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectStack.Add(statusEffect);
    }

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        StatusEffectStack.Remove(statusEffect);
    }

    public void TakeParalysisDamage(BigNumber damage)
    {
        WillHit(damage);
        OnHit(damage, DamageType.Pure, false);
    }

    public void TakeBurnDamage(BigNumber damage)
    {
        WillHit(damage);
        OnHit(damage, DamageType.Pure, false);
    }

    public void TakePoisonDamage(BigNumber damage)
    {
        WillHit(damage);
        OnHit(damage, DamageType.Magical, false);
    }
}