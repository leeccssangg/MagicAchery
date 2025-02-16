using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Screen = TW.UGUI.Core.Screens.Screen;

public class ScreenMainMenuTab : Screen
{
    [field: SerializeField] public ScreenMainMenuTabContext.UIPresenter UIPresenter { get; private set; }
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

    public override async UniTask WillPushEnter(Memory<object> args)
    {
        await base.WillPushEnter(args);
        //AudioManager.Instance.ChangeMusic(AudioType.BgMainMenu, 1);
    }

    public override async UniTask WillPopEnter(Memory<object> args)
    {
        await base.WillPopEnter(args);
        //AudioManager.Instance.ChangeMusic(AudioType.BgMainMenu, 1);
    }
}
