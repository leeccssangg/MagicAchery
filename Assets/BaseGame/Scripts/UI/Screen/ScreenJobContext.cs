using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using Pextension;
using TW.UGUI.Core.Sheets;
using TW.Utility.CustomType;

[Serializable]
public class ScreenJobContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public ReactiveValue<int> CurrentJobId { get; private set; }
        [field: SerializeField] public ReactiveValue<int> CurrentJobLevel { get; private set; }
        [field: SerializeField] public JobConfig CurrentJobConfig { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            CurrentJobId = InGameDataManager.Instance.InGameData.PlayerBattleData.JobId;
            CurrentJobLevel = InGameDataManager.Instance.InGameData.PlayerBattleData.JobLevel;
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
        [field: SerializeField] public TextMeshProUGUI TxtJobName { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtJobLevel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtJobReward { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtJobUpgadeRequire { get; private set; }
        [field: SerializeField] public Button BtnUpgradeJob { get; private set; }
        [field: SerializeField] public UIResourceCoin UIResourceCoin { get; private set; }
        
        public UniTask Initialize(Memory<object> args)
        {
            UIResourceCoin.Initialize(args);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        private List<IDisposable> disposable = new();

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            Model.CurrentJobId.ReactiveProperty
                .CombineLatest(Model.CurrentJobLevel.ReactiveProperty, (id, level) => (id, level))
                .Subscribe(Setup).AddTo(View.MainView);
            //disposable = Model.CurrentJobConfig.ReactiveProperty.Subscribe(ReSubcribeStatsRequire).AddTo(View.MainView);
        }
        public void Setup((int id, int level) value)
        {
            Model.OnJobIdChanged(Model.CurrentJobId);
            View.TxtJobName.SetText($"{Model.CurrentJobConfig.Name}");
            View.TxtJobLevel.SetText($"{Model.CurrentJobLevel.Value}");
            View.TxtJobReward.SetText($"{Model.CurrentJobConfig.GetJobReward(Model.CurrentJobLevel.Value).Amount.ToStringUI()} <sprite index=0>");
            View.TxtJobUpgadeRequire.SetText($"{Model.CurrentJobConfig.GetJobRequireDescription(Model.CurrentJobLevel.Value)}");
            View.BtnUpgradeJob.SetOnClickDestination(OnUpgradeJob);
            ReSubcribeStatsRequire();
            //View.BtnUpgradeJob.interactable = JobManager.Instance.IsUpdradeAbleJob();
        }
        public void ReSubcribeStatsRequire()
        {
            for(int i = 0; i< disposable.Count; i++)
            {
                disposable[i].Dispose();
            }
            disposable = new();
            for(int i = 0; i< Model.CurrentJobConfig.JobUpgradeRequirement.Count; i++)
            {
                JobUpgradeRequirement jobUpgradeRequirement = Model.CurrentJobConfig.JobUpgradeRequirement[i];
                GameStat.Type gameStat = jobUpgradeRequirement.StatType;
                IDisposable dis = PlayerStatData.Instance.GetGameResource(gameStat).ReactiveExperience.ReactiveProperty.Subscribe(OnExpStatRequireChange).AddTo(View.MainView);
                disposable.Add(dis);
            }
        }
        public void OnExpStatRequireChange(BigNumber value)
        {
           View.BtnUpgradeJob.interactable = JobManager.Instance.IsUpdradeAbleJob();
        }
        private void OnUpgradeJob(Unit _)
        {
            JobManager.Instance.UpgradeJob();
        }
    }
}