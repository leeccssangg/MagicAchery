using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;

public class ScreenTitle : Screen
{
    public override void DidPushEnter(Memory<object> args)
    {
        base.DidPushEnter(args);
        //AudioManager.Instance.PlayMusic(AudioType.BgMainMenu);
        //FirstScene.Events.OnLoadingComplete?.Invoke();
        WaitToChangeToLoading().Forget();
    }
    private async UniTask WaitToChangeToLoading()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        ScreenOptions options = new ScreenOptions(nameof(ScreenLoading), playAnimation: false, loadAsync: false, stack: false);
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    }
}
