using System;
using TW.Utility.CustomComponent;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponSelect : ACachedMonoBehaviour
{
    [field: SerializeField] public float OffsetProcess {get; private set;}
    [field: SerializeField] public CanvasGroup MainGroup {get; private set;}
    [field: SerializeField] public Image ImageSelect {get; private set;}
    
    [field: SerializeField] public Vector3 StartPosition {get; private set;}
    [field: SerializeField] public Vector3 EndPosition {get; private set;}
    [field: SerializeField] public AnimationCurve ScaleCurve {get; private set;}
    [field: SerializeField] public AnimationCurve ImageAlphaCurve {get; private set;}
    [field: SerializeField] public AnimationCurve AlphaCurve {get; private set;}
    public void UpdateProcess(float process)
    {
        float processValue = Mathf.Repeat(process + OffsetProcess, 6f);
        Transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, processValue/6f);
        Transform.localScale = Vector3.one * ScaleCurve.Evaluate(processValue);
        ImageSelect.color = new Color(1f, 1f, 1f, ImageAlphaCurve.Evaluate(processValue));
        MainGroup.alpha = AlphaCurve.Evaluate(processValue);
        
    }
}
