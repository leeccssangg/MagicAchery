using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FadeFromTweenTransition : TweenTransition
{
    [field: SerializeField] public CanvasGroup Owner {get; private set;}
    [field: SerializeField] public float StartAlpha {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    [field: SerializeField] public float Delay {get; private set;}
    [field: SerializeField] public Ease Ease {get; private set;}
    
    private float StartValue {get; set;}
    public override void Init()
    {
        base.Init();
        StartValue = Owner.alpha;
    }
    
    public override void SetupStart()
    {
        base.SetupStart();
        Owner.alpha = StartAlpha;
    }
    public override Tween Play()
    {
        MainTween = Owner.DOFade(StartValue, Duration).SetEase(Ease).SetDelay(Delay);
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
