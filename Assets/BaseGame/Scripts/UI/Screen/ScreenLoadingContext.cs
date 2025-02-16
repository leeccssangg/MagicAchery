using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine;
using UnityEngine.UI;
using TW.UGUI.MVPPattern;

[Serializable]
public class ScreenLoadingContext 
{
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Slider LoadingBar {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter , IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIView View { get; set; } = new();
        
        public async UniTask Initialize(Memory<object> args)
        {
            await View.Initialize(args);
        }

        public void DidPushEnter(Memory<object> args)
        {
            StartLoading();
        }

        public void DidPushExit(Memory<object> args)
        {

        }

        public void StartLoading()
        {
            View.LoadingBar.value = 0;
            View.LoadingBar.DOValue(1, 4).OnComplete(OnLoadingComplete);
        }
        public void OnLoadingComplete()
        {
            //if (TutorialManager.Instance.NeedPlayTutorialAim)
            //{
            //    GameManager.Instance.CreateTutorial();

            //    ViewOptions options1 = new ViewOptions(nameof(ScreenInGame));
            //    ScreenContainer.Find(ContainerKey.Screens).PushAsync(options1); 
            //    return;
            //}


            ViewOptions options = new ViewOptions(nameof(ScreenMainMenuTab));
            ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
            //ScreenContainer.Find(ContainerKey.Screens).PopAsync(true);
            //ViewOptions options = new ViewOptions(nameof(ScreenStats));
            //ScreenContainer.Find(ContainerKey.TabScreens).PushAsync(options);
            //GameManager.Instance.HandleChestReward();
            //GameManager.Instance.HandleAttention();
        }
    }
}