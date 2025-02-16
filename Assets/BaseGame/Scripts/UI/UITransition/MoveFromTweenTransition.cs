using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class MoveFromTweenTransition : TweenTransition
{
    [field: SerializeField] public Transform Owner {get; private set;}
    [field: SerializeField] public Vector3 MoveOffset {get; private set;}
    [field: SerializeField] public float Duration {get; private set;}
    [field: SerializeField] public float Delay {get; private set;}
    [field: SerializeField] public Ease Ease {get; private set;}
    
    private Vector3 StartPosition {get; set;}
    public override void Init()
    {
        base.Init();
        StartPosition = Owner.position;
    }
    
    public override void SetupStart()
    {
        base.SetupStart();
        Owner.position = StartPosition + MoveOffset;
    }
    public override Tween Play()
    {
        MainTween = Owner.DOMove(StartPosition, Duration).SetEase(Ease).SetDelay(Delay);
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