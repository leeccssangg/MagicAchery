using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Activities;
using UnityEngine;

public class ActivityTutorialEquipment : Activity
{
    [field: SerializeField] public ActivityTutorialEquipmentContext.UIPresenter UIPresenter {get; private set;}

    protected override void Awake()
    {
        base.Awake();
        AddLifecycleEvent(UIPresenter, 1);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
}
