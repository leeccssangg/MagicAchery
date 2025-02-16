using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Windows;

using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    protected override void Start()
    {
        base.Start();
        ShowLoadingScreen().Forget();
    }

    private async UniTaskVoid ShowLoadingScreen()
    {
        ScreenOptions options = new ScreenOptions(nameof(ScreenTitle), false, loadAsync: false, stack: false);
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    }
}
