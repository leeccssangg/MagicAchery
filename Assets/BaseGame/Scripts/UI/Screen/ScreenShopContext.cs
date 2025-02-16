using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Sheets;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[Serializable]
public class ScreenShopContext 
{
    public static class Events
    {
        public static Action<Vector3, int> DelayAddCoin { get; set; } 
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }

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
        [field: SerializeField] public Button ButtonClose {get; private set;}

        public UniTask Initialize(Memory<object> args)
        {
            
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        
        private int[] SheetId { get; set; } = new int[3];
        
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.DelayAddCoin = null;
            return UniTask.CompletedTask;
        }

        public void DidPushEnter(Memory<object> args)
        {

        }

    }
}