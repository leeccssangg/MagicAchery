using System.Threading;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using TW.Utility.CustomComponent;
using UnityEngine;

public class MonsterAnim : ACachedMonoBehaviour
{
    private const string Idle = "idle";
    private const string Run = "move";
    private const string Attack = "attack_melee_1";
    
    [field: SerializeField] private SkeletonAnimation SkeletonAnimation {get; set;}
    private Transform SkeletonTransformCache {get; set;}
    private Transform SkeletonTransform => SkeletonTransformCache ??= SkeletonAnimation.transform;
    public void PlayIdleAnimation()
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Idle, true);
    }
    public void PlayRunAnimation()
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Run, true);
    }
    
    public void PlayAttackAnimation(CancellationToken cancellationToken)
    {
        SkeletonAnimation.timeScale = 1;
        SkeletonAnimation.AnimationState.SetAnimation(0, Attack, false);
    }
    public void UpdateFlip(Vector3 lookTarget) 
    {
        SkeletonTransform.localEulerAngles = new Vector3(0, lookTarget.x > SkeletonTransform.position.x ? 0 : 180, 0);
    }
}