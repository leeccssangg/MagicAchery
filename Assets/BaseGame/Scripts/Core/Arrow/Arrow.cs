using System;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arrow : ACachedMonoBehaviour, IPoolAble<Arrow>
{
    private PlayerStatData PlayerStatDataCache { get; set; }
    protected PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    protected TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    
    [field: SerializeField] protected Hero Hero {get; set;}
    [field: SerializeField] protected Monster TargetMonster {get; set;}
    [field: SerializeField] public DamageType DamageType {get; private set;}
    [field: SerializeField] public GameObject[] FullProjectile {get; private set;}
    [field: SerializeField] public GameObject[] OnHitProjectile {get; private set;}
    [field: SerializeField] public float MovementSpeed {get; private set;}
    [field: SerializeField, SuffixLabel("ms")] public int DelayDespawn {get; private set;}
    [field: SerializeField] public AnimationCurve ScaleCurve {get; private set;}
    [field: SerializeField] public AnimationCurve TrajectoryCurve {get; private set;}
    [field: SerializeField] public AnimationCurve AxisCurve {get; private set;}
    [field: SerializeField] public AnimationCurve VelocityCurve {get; private set;}
    [field: SerializeField] public float TrajectoryHeight {get; private set;}
    [field: SerializeField, ReadOnly] public BigNumber AttackDamage { get; set; }
    [field: SerializeField, ReadOnly] public BigNumber CriticalRate { get; set; }
    [field: SerializeField, ReadOnly] public BigNumber CriticalDamage { get; set; }
    [field: SerializeField, ReadOnly] public BigNumber OtherAttackDamageScale { get; set; }
    [field: SerializeField, ReadOnly] public BigNumber OtherCriticalDamageScale { get; set; }
    private Transform TargetHitPosition {get; set;}
    private Vector3 StartPosition {get; set;}
    private Vector3 TargetPosition {get; set;}
    private BigNumber DamageDeal {get; set;}
    private float Distance {get; set;}
    private Vector3 RandomNoise {get; set;}
    private bool IsCritical {get; set;}
    private float CurrentMoveSpeed { get; set; }
    public Arrow Setup(Hero hero, Vector3 startPosition, Monster targetMonster)
    {
        Hero = hero;
        StartPosition = startPosition;
        TargetMonster = targetMonster;
        DamageDeal = GetDamageDeal(targetMonster, out bool isCritical);
        IsCritical = isCritical;

        RandomNoise = Random.insideUnitCircle * 0.2f;
        TargetHitPosition = TargetMonster.HitPosition;
        
        TargetPosition = TargetHitPosition.position + RandomNoise;
        Distance = Vector3.Distance(startPosition, TargetPosition);
        Transform.position = startPosition;
        Transform.localScale = Vector3.zero;

        return this;
    }
    public Arrow Launch()
    {
        TargetMonster.WillHit(DamageDeal);
        StartMoveToTarget().Forget();
        return this;
    }
    private async UniTask StartMoveToTarget()
    {
        SetFullState(true);
        await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                           .WithCancellation(this.GetCancellationTokenOnDestroy()))
        {
            if (IsTargetDead())
            {
                this.Despawn();
                break;
            }
            if (TryMoveToTarget()) continue;

            OnMoveComplete();
            break;
        }
    }
    private bool IsTargetDead()
    {
        return TargetMonster.IsDead;
    }
    private void OnMoveComplete()
    {
        SetParent(TargetHitPosition);
        SetFullState(false);
        if (DamageDeal > 0.1f)
        {
            TargetMonster.OnHit(DamageDeal, DamageType, IsCritical);
        }
        OnMonsterHit(TargetMonster);
        DelayAndDespawn().Forget();
    }
    private bool TryMoveToTarget()
    {
        Vector3 currentPosition = Transform.position;
        Vector3 targetPosition = TargetMonster.HitPosition.position + RandomNoise;
        Vector3 distance = targetPosition - StartPosition;
        UpdatePosition(currentPosition, distance, targetPosition, out bool isComplete);
        return !isComplete;
    }
    private void UpdatePosition(Vector3 currentPosition, Vector3 distance, Vector3 targetPosition, out bool isComplete)
    {
        Vector3 nextPosition = currentPosition;
        nextPosition.x = currentPosition.x + CurrentMoveSpeed * Time.deltaTime * Mathf.Sign(distance.x);
        float nextPositionXNormalized = Mathf.Abs((nextPosition.x - StartPosition.x) / distance.x);
        float nextPositionYAxis = AxisCurve.Evaluate(nextPositionXNormalized) * distance.y;
        float trajectoryValue = TrajectoryCurve.Evaluate(nextPositionXNormalized) * TrajectoryHeight * Mathf.Abs(distance.x);

        nextPosition.y = StartPosition.y + trajectoryValue + nextPositionYAxis;

        Vector3 direction = nextPosition - currentPosition;
        Transform.position = nextPosition;
        if (direction != Vector3.zero)
        {
            Quaternion flyRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 90);
            Transform.rotation = flyRotation;
        }
        Transform.localScale = Vector3.one * ScaleCurve.Evaluate(nextPositionXNormalized);

        CurrentMoveSpeed = VelocityCurve.Evaluate(nextPositionXNormalized) * MovementSpeed;
        isComplete = Mathf.Abs(nextPosition.x - StartPosition.x) > Mathf.Abs(targetPosition.x - StartPosition.x);
    }
    private void SetParent(Transform parent)
    {
        Transform.SetParent(parent);
    }
    private void SetFullState(bool isFull)
    {
        foreach (GameObject ob in FullProjectile)
        {
            ob.SetActive(isFull);
        }
        foreach (GameObject ob in OnHitProjectile)
        {
            ob.SetActive(!isFull);
        }
    }
    private async UniTask DelayAndDespawn()
    {
        await UniTask.Delay(DelayDespawn);
        this.Despawn();
    }


    protected virtual BigNumber GetDamageDeal(Monster monster, out bool isCritical)
    {
        AttackDamage = Hero.PhysicalAttack.Current;
        CriticalRate = Hero.PhysicalCriticalChance.Current;
        CriticalDamage = Hero.PhysicalCriticalDamage.Current;
        OtherAttackDamageScale = GetAttackDamageScale();
        OtherCriticalDamageScale = GetCriticalDamageScale();
        
        BigNumber damageDeal = AttackDamage * OtherAttackDamageScale;
        isCritical = Random.Range(0, 100) < CriticalRate;
        if (isCritical)
        {
            damageDeal *= (CriticalDamage + OtherCriticalDamageScale);
        }
        return damageDeal;
    }
    protected virtual void OnMonsterHit(Monster monster)
    {
        
    }
    public Arrow OnSpawn()
    {
        SetParent(null);
        return this;
    }
    public void OnDespawn()
    {

    }

    protected virtual BigNumber GetAttackDamageScale()
    {
        BigNumber damageScale = 1;
        damageScale += GetDamageScaleToHighHpEnemy();
        damageScale += GetDamageScaleToLowHpEnemy();
        damageScale += GetDamageScaleByEnemyType();
        return damageScale;
    }
    protected virtual BigNumber GetCriticalDamageScale()
    {
        BigNumber criticalDamageScale = 0;
        criticalDamageScale += GetPhysicalCriticalDamageByHeroHp();
        return criticalDamageScale;
    }

    private BigNumber GetDamageScaleToHighHpEnemy()
    {
        float enemyHpPercent = TargetMonster.CurrentHitPointPercent();
        if (enemyHpPercent < 0.7f) return 0;
        return TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToHighHpEnemy).Amount / 100f;
    }
    private BigNumber GetDamageScaleToLowHpEnemy()
    {
        float enemyHpPercent = TargetMonster.CurrentHitPointPercent();
        if (enemyHpPercent > 0.9f) return 0;
        int count = (int)((1 - enemyHpPercent) * 100f / 15);
        return count * TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToLowHpEnemy).Amount / 100f;
    }
    private BigNumber GetDamageScaleByEnemyType()
    {
        return TargetMonster.MonsterType switch
        {
            Monster.Type.Normal => TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToNormalEnemy).Amount / 100f,
            Monster.Type.Rare => TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToRareEnemy).Amount / 100f,
            Monster.Type.Epic => TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToEpicEnemy).Amount / 100f,
            Monster.Type.Boss => TalentTreeManager.GetTalentStat(TalentStat.Type.DamageDealtToBossEnemy).Amount / 100f,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    private BigNumber GetPhysicalCriticalDamageByHeroHp()
    {
        if (DamageType != DamageType.Physical) return 0;
        float heroHpPercent = Hero.HitPoint.CurrentPercent;
        if (!(heroHpPercent < 0.99f)) return 0;
        int count = (int)((1 - heroHpPercent) * 100f);
        return count * TalentTreeManager.GetTalentStat(TalentStat.Type.PhysicalCriticalDamageBoostByHeroHp).Amount / 100f;
    }
}