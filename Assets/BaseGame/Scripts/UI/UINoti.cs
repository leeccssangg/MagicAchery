using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using TW.Utility.CustomType;
using UnityEngine;
using R3;
using TW.UGUI.MVPPattern;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;

public class UINoti : ACachedMonoBehaviour
{
    [field: SerializeField] public FeelAnimation FeelAnimation { get; private set; }

    public void PlayAnimation(bool isPlay)
    {
        //if (!isPlay)
        //{
        //    return;
        //}
        //FeelAnimation.Play();
    }
}
