using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIImageTweenTransition : TweenTransition
{
    [field: SerializeField] public Image MainImage {get; private set;}
    [field: SerializeField] public List<Sprite> SpriteList {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    [field: SerializeField] public float Delay {get; private set;}
    private int CurrentIndex { get; set; }
    private float Progress { get; set; }
    
    private void OnDestroy()
    {
        //this.ClearAllDelegate();
    }

    public override void Init()
    {
        base.Init();
        //this.AddDelegate<DOGetter<float>>(nameof(GetProgress));
        //this.AddDelegate<DOSetter<float>>(nameof(SetProgress));
        //this.AddDelegate<TweenCallback>(nameof(ChangeSprite));
    }
    
    public override void SetupStart()
    {
        base.SetupStart();
        CurrentIndex = 0;
        MainImage.sprite = SpriteList[0];
    }

    public override Tween Play()
    {
        Progress = 0;
        MainTween = DOTween.To(GetProgress,
            SetProgress, 1, Duration)
            .SetDelay(Delay)
            .SetEase(Ease.Linear)
            .OnUpdate(ChangeSprite);
        
        return base.Play();
    }
    
    private float GetProgress()
    {
        return Progress;
    }
    
    private void SetProgress(float progress)
    {
        Progress = progress;
    }
    
    private void ChangeSprite()
    {
        CurrentIndex = Mathf.Clamp(Mathf.FloorToInt(Progress * (SpriteList.Count - 1)), 0, SpriteList.Count - 1);
        MainImage.sprite = SpriteList[CurrentIndex];
    }
    public override Tween Kill()
    {
        return base.Kill();
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
