using System;
using Core.SimplePool;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : ACachedMonoBehaviour, IPoolAble<Projectile>
{
    private PlayerStatData PlayerStatDataCache { get; set; }
    protected PlayerStatData PlayerStatData => PlayerStatDataCache ??= PlayerStatData.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    protected TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    
    [field: SerializeField] protected Hero Hero {get; set;}
    [field: SerializeField] private Monster TargetMonster {get; set;}
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
    private Transform TargetHitPosition {get; set;}
    private Vector3 StartPosition {get; set;}
    private Vector3 TargetPosition {get; set;}
    private BigNumber DamageDeal {get; set;}
    private float Distance {get; set;}
    private Vector3 RandomNoise {get; set;}
    private bool IsCritical {get; set;}
    private float CurrentMoveSpeed { get; set; }
    public Projectile Setup(Hero hero, Vector3 startPosition, Monster targetMonster)
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
    public Projectile Launch()
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
        TargetMonster.OnHit(DamageDeal, DamageType, IsCritical);
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
        isCritical = false;
        return DamageDeal;
    }
    protected virtual void OnMonsterHit(Monster monster)
    {
        
    }
    public Projectile OnSpawn()
    {
        SetParent(null);
        return this;
    }

    public void OnDespawn()
    {

    }
}