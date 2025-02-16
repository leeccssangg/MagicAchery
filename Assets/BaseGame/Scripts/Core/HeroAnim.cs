using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using Spine.Unity;
using TW.ACacheEverything;
using TW.Utility.Extension;
using UnityEngine;

public partial class HeroAnim : MonoBehaviour
{
    private static readonly int Black = Shader.PropertyToID("_Black");
    
    private const string StartAttack = "attack1";
    private const string Attack = "attack2";
    private const string EndAttack = "attack3";
    private const string Idle = "idle";
    private const string Run = "run";
    private const string Down = "down";
    private const string Up = "up";
    private const float StartAttackTime = 0.1f;
    private const float AttackTime = 0.333f;
    private const float AttackSpawnTime = 0.133f;
    private const float EndAttackTime = 0.1667f;
    private const float DownTime = 0.667f;
    private const float UpTime = 0.5f;
    
    [field: SerializeField] private SkeletonAnimation SkeletonAnimation {get; set;}
    [field: SerializeField] private Transform RootTarget {get; set;}
    [field: SerializeField] private Transform RootVisual {get; set;}
    [field: SerializeField] private Vector3 DefaultPosition {get; set;}
    [field: SerializeField] private Vector3 OffsetPosition {get; set;}
    [field: SerializeField] private Transform Target {get; set;}

    private Vector3 StartRootTarget {get; set;}
    private Vector3 CurrentRootTarget {get; set;}
    private Vector3 FixedTarget {get; set;}
    private MotionHandle FollowHandle {get; set;}
    private MotionHandle ChangeColorHandle {get; set;}
    
    
    [field: SerializeField] public Color[] HitColor {get; private set;}
    private Color StartColor {get; set;}
    private Color CurrentColor {get; set;}
    private MaterialPropertyBlock Block {get; set;}
    private MeshRenderer MeshRenderer {get; set;}
    
    private void Awake()
    {
        StartRootTarget = RootTarget.localPosition;
        CurrentRootTarget = RootTarget.localPosition;
        MeshRenderer = SkeletonAnimation.GetComponent<MeshRenderer>();
        Block = new MaterialPropertyBlock();
    }
    public async UniTask PlayIdleAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
        await UniTask.WaitUntil(IsCurrentIdleAnimationCache, cancellationToken: cancellationToken);
    }
    [ACacheMethod]
    private bool IsCurrentIdleAnimation()
    {
        return SkeletonAnimation.AnimationState.GetCurrent(0).Animation.Name == Idle;
    }
    public async UniTask PlayRunAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Run, true);
        
        await UniTask.WaitUntil(IsCurrentRunAnimationCache, cancellationToken: cancellationToken);
    }
    [ACacheMethod]
    private bool IsCurrentRunAnimation()
    {
        return SkeletonAnimation.AnimationState.GetCurrent(0).Animation.Name == Run;
    }
    public async UniTask PlayStartAttackAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, StartAttack, false);
        await UniTask.Delay(TimeSpan.FromSeconds(StartAttackTime), cancellationToken: cancellationToken);
    }

    public async UniTask PlayAttackAnimation(float speed, Monster targetMonster, Action<Monster> spawnAttack, CancellationToken cancellationToken)
    {
        float timeScale = speed * AttackTime;
        float attackSpawnTime = AttackSpawnTime / timeScale;
        float attackDelay = (AttackTime - AttackSpawnTime) / timeScale;
        SkeletonAnimation.timeScale = timeScale;
        SkeletonAnimation.AnimationState.SetAnimation(0, Attack, false);
        await UniTask.Delay(TimeSpan.FromSeconds(attackSpawnTime), cancellationToken: cancellationToken);
        spawnAttack?.Invoke(targetMonster);
        await UniTask.Delay(TimeSpan.FromSeconds(attackDelay), cancellationToken: cancellationToken);
    }
    public async UniTask PlayAttackAnimation(float speed, Monster[] targetMonsters, Action<Monster[]> spawnAttack, CancellationToken cancellationToken)
    {
        float timeScale = speed * AttackTime;
        float attackSpawnTime = AttackSpawnTime / timeScale;
        float attackDelay = (AttackTime - AttackSpawnTime) / timeScale;
        SkeletonAnimation.timeScale = timeScale;
        SkeletonAnimation.AnimationState.SetAnimation(0, Attack, false);
        await UniTask.Delay(TimeSpan.FromSeconds(attackSpawnTime), cancellationToken: cancellationToken);
        spawnAttack?.Invoke(targetMonsters);
        await UniTask.Delay(TimeSpan.FromSeconds(attackDelay), cancellationToken: cancellationToken);
    }
    public async UniTask PlayEndAttackAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, EndAttack, false);
        await UniTask.Delay(TimeSpan.FromSeconds(EndAttackTime), cancellationToken: cancellationToken);
    }
    public void ChangeRootTarget(Transform target, float duration)
    {
        if (target == Target) return;

        Target = target;
        StartRootTarget = CurrentRootTarget;
        if (target == null)
        {
            FixedTarget = DefaultPosition;
        }
        else
        {
            FixedTarget = RootVisual.localPosition + (Target.position - RootVisual.position).normalized + OffsetPosition;
        }
        FollowHandle.TryCancel();
        FollowHandle = LMotion.Create(0, 1f, duration)
            .WithEase(Ease.InOutSine)
            .Bind(OnFollowTargetCache);
    }
    public async UniTask PlayDownAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Down, false);
        await UniTask.Delay(TimeSpan.FromSeconds(DownTime), cancellationToken: cancellationToken);
    }
    public async UniTask PlayUpAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Up, false);
        await UniTask.Delay(TimeSpan.FromSeconds(UpTime), cancellationToken: cancellationToken);
    }
    
    [ACacheMethod]
    private void OnFollowTarget(float value)
    {
        CurrentRootTarget = Vector3.Lerp(StartRootTarget, FixedTarget, value);
        RootTarget.localPosition = CurrentRootTarget;
    }

    public void OnHitEffect()
    {
        StartColor =  CurrentColor;
        ChangeColorHandle.TryCancel();
        ChangeColorHandle = LMotion.Create(0f, 2f, 0.1f)
            .Bind(OnChangeColorCache);
    }
    [ACacheMethod]
    private void OnChangeColor(float value)
    {
        Block.SetColor(Black, value < 1 ? 
            Color.Lerp(CurrentColor, HitColor[0], value) : 
            Color.Lerp(HitColor[0], HitColor[1], value - 1));
        MeshRenderer.SetPropertyBlock(Block);
    }
}