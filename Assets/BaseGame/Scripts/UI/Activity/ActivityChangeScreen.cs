using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Activities;
using UnityEngine;

public class ActivityChangeScreen : Activity
{
    [field: SerializeField] public ActivityChangeScreenContext.UIPresenter UIPresenter {get; private set;}

    protected override void Awake()
    {
        base.Awake();
        // The lifecycle event of the view will be added with priority 0.
        // Presenters should be processed after the view so set the priority to 1.
        AddLifecycleEvent(UIPresenter, 1);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
}
