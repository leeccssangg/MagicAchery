using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using System.Collections.Generic;
using Pextension;
using R3.Triggers;
using System.Collections;
using TW.Utility.Extension;
using System.Threading;
using TW.UGUI.MVPPattern;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class ActivityMapInfoContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action HideActivity { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public ReactiveValue<int> CurMapId { get; private set; }
        [field: SerializeField] public ReactiveValue<int> CurrentJobLevel { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            CurMapId = PlayerBattleData.Instance.JobId;
            CurrentJobLevel = PlayerBattleData.Instance.JobLevel;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public RectTransform RectTfContent { get; private set; }
        [field: SerializeField] public List<UIMapMonsterInfo> ListUIMapMonster { get; private set; }
        [field: SerializeField] public UIMapBossInfo UIMapBoss { get; private set; }

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

            View.MainView.LateUpdateAsObservable()
           .Subscribe(OnLateUpdate)
           .AddTo(View.MainView);

            Events.HideActivity = HideActivity;

            Model.CurMapId.ReactiveProperty
               .CombineLatest(Model.CurrentJobLevel.ReactiveProperty, (id, level) => (id, level))
               .Subscribe(SetupUI).AddTo(View.MainView);
        }
        private void SetupUI((int id, int level) value)
        {
            for(int i = 0; i < View.ListUIMapMonster.Count; i++)
            {
                View.ListUIMapMonster[i].Setup(i);
            }
            View.UIMapBoss.Setup(View.ListUIMapMonster.Count);
        }
        private void OnLateUpdate(Unit unit)
        {
            //if (!View.GoGiftDetail.activeInHierarchy) return;;
            if (Input.GetMouseButtonDown(0) &&
                !RectTransformUtility.RectangleContainsScreenPoint(View.RectTfContent, Input.mousePosition, null))
            {
                HideActivity();
            };
        }
        private void HideActivity()
        {
            ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityMapInfo));
        }
    }
}