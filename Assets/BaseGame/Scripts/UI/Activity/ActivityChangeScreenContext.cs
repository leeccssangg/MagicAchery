using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.Shared;
using TW.UGUI.Utility;
using TW.UGUI.MVPPattern;

[Serializable]
public class ActivityChangeScreenContext 
{
    public static class Events
    {
        // public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    { 
        public UniTask Initialize(Memory<object> args)
        {   
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}        
        [field: SerializeField] public FeelAnimation ShowAnimation {get; private set;}
        [field: SerializeField] public FeelAnimation HideAnimation {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

        }

        public void DidEnter(Memory<object> args)
        {
            EnterFunction(args).Forget();
        }

        public async UniTask EnterFunction(Memory<object> args)
        {
            Func<UniTask> onShowFunction = args.Span[0] as Func<UniTask>;
            float delay = (float)args.Span[2];
            
            await (onShowFunction?.Invoke() ?? UniTask.CompletedTask);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: View.MainView.GetCancellationTokenOnDestroy());
            
            await ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityChangeScreen));
            Func<UniTask> onHideFunction = args.Span[1] as Func<UniTask>;
            await (onHideFunction?.Invoke() ?? UniTask.CompletedTask);
        }
        
    }
}