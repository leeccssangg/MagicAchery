using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TW.Reactive.CustomComponent;
using TW.UGUI.MVPPattern;
using UnityEngine;

public class UIBonusBar : MonoBehaviour, IAView
{
    [field: SerializeField] public Transform[] BonusText {get; private set;}
    [field: SerializeField] public Transform Arrow {get; private set;}
    [field: SerializeField] public float XPos {get; private set;}
    [field: SerializeField] public float[] BonusTable {get; private set;}
    [field: SerializeField] public ReactiveValue<float> CurrentBonus {get; private set;}
    [field: SerializeField] public AnimationCurve TextScaleCurve {get; private set;}
    private Tween TweenBonus { get; set; }
    private float Process { get; set; }
    public UniTask Initialize(Memory<object> args)
    {
        Process = 0;
        TweenBonus = DOTween.To(() => Process, x => Process = x, 7, 2)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .OnUpdate(OnTweenUpdate);

        return UniTask.CompletedTask;
    }

    public void Pause()
    {
        TweenBonus?.Pause();
    }
    public void Resume()
    {
        TweenBonus?.Play();
    }
    private void OnDestroy()
    {
        TweenBonus?.Kill();
    }

    private void OnTweenUpdate()
    {
        Vector3 pos = Arrow.localPosition;
        Arrow.localPosition = new Vector3(Remap(Process, 0, 7, -XPos, XPos), pos.y, pos.z);
        CurrentBonus.Value = BonusTable[Mathf.Clamp(Mathf.FloorToInt(Process), 0, BonusTable.Length - 1)];
        for (int i = 0; i < BonusText.Length; i++)
        {
            BonusText[i].localScale = TextScaleCurve.Evaluate(Process - i) * Vector3.one;
        }
    }
    private static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
