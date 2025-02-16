using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleFromTweenTransition : TweenTransition
{
    [field: SerializeField] public Transform Owner {get; private set;}
    [field: SerializeField] public Vector3 ScaleOffset {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    [field: SerializeField] public float Delay {get; private set;}
    [field: SerializeField] public Ease Ease {get; private set;}
    
    private Vector3 StartScale {get; set;}
    public override void Init()
    {
        base.Init();
        StartScale = Owner.localScale;
    }
    
    public override void SetupStart()
    {
        base.SetupStart();
        Owner.localScale = StartScale + ScaleOffset;
    }
    public override Tween Play()
    {
        MainTween = Owner.DOScale(StartScale, Duration).SetEase(Ease).SetDelay(Delay);
        return MainTween;
    }
    public override Tween Kill()
    {
        MainTween?.Kill();
        return MainTween;
    }
    public override float GetDuration()
    {
        return Duration;
    }
    
    public override float GetDelay()
    {
        return Delay;
    }
}
