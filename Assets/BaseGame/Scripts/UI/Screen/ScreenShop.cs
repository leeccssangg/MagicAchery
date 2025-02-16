using UnityEngine;
using Screen = TW.UGUI.Core.Screens.Screen;

public class ScreenShop : Screen
{
    [field: SerializeField] public ScreenShopContext.UIPresenter UIPresenter {get; private set;}

    protected override void Awake()
    {
        base.Awake();
        // The lifecycle event of the view will be added with priority 0.
        // Presenters should be processed after the view so set the priority to 1.
        AddLifecycleEvent(UIPresenter, 1);
    }
}