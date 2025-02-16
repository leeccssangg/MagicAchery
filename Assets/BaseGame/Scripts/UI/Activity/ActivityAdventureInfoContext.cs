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
using TW.Utility.CustomType;

[Serializable]
public class ActivityAdventureInfoContext 
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
        [field: SerializeField] public JobConfig CurrentJobConfig { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            CurMapId = PlayerBattleData.Instance.JobId;
            CurrentJobLevel = PlayerBattleData.Instance.JobLevel;
            return UniTask.CompletedTask;
        }
        public void OnJobIdChanged(int id)
        {
            CurrentJobConfig = JobManager.Instance.GetJobConfig(id);
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public RectTransform RectTfContent { get; private set; }
        [field: SerializeField] public Button BtnNextMap { get; private set; }
        [field: SerializeField] public List<UIStatsRequire> ListUIStatsRequire { get; private set; }

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
        private List<IDisposable> disposable = new();

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

            View.BtnNextMap.SetOnClickDestination(OnClickBtnNextMap);


            
        }
        private void SetupUI((int id, int level) value)
        {
            Debug.Log("OnJobIdChanged");
            Model.OnJobIdChanged(value.id);
            for (int i = 0; i < View.ListUIStatsRequire.Count; i++)
            {
                if(i < Model.CurrentJobConfig.JobUpgradeRequirement.Count)
                {
                    View.ListUIStatsRequire[i].Setup(i);
                }
                else
                {
                    View.ListUIStatsRequire[i].Setup(-1);
                }
            }
            //View.BtnNextMap.interactable = JobManager.Instance.IsUpdradeAbleJob();
            ReSubcribeStatsRequire();
        }
        public void ReSubcribeStatsRequire()
        {
            for (int i = 0; i < disposable.Count; i++)
            {
                disposable[i].Dispose();
            }
            disposable = new();
            for (int i = 0; i < Model.CurrentJobConfig.JobUpgradeRequirement.Count; i++)
            {
                JobUpgradeRequirement jobUpgradeRequirement = Model.CurrentJobConfig.JobUpgradeRequirement[i];
                GameStat.Type gameStat = jobUpgradeRequirement.StatType;
                IDisposable dis = PlayerStatData.Instance.GetGameResource(gameStat).ReactiveLevel.ReactiveProperty.Subscribe(OnExpStatRequireChange).AddTo(View.MainView);
                disposable.Add(dis);
            }
        }
        public void OnExpStatRequireChange(BigNumber value)
        {
            Debug.Log("OnExpStatRequireChange");
            View.BtnNextMap.interactable = JobManager.Instance.IsUpdradeAbleJob();
            //if(JobManager.Instance.IsUpdradeAbleJob())
            //{
            //    Debug.Log("UpgradeAble");
            //}
        }
        private void OnClickBtnNextMap(Unit _)
        {
            Debug.Log("UpgradeJob");
            JobManager.Instance.UpgradeJob();
            //HideActivity();
        }
        private void OnLateUpdate(Unit unit)
        {
            //if (!View.GoGiftDetail.activeInHierarchy) return;;
            //Debug.Log(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) &&
                !RectTransformUtility.RectangleContainsScreenPoint(View.RectTfContent, Input.mousePosition, null))
            {
                HideActivity();
            };
        }
        private void HideActivity()
        {
            ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityAdventureInfo));
        }
    }
}