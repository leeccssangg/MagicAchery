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
using TW.UGUI.Core.Activities;


[Serializable]
public class ScreenStatsContext 
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
        [field: SerializeField] public ReactiveValue<int> CurrentUnlockedId { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            CurrentUnlockedId = InGameDataManager.Instance.InGameData.StatsData.UnlockedId;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set; }
        [field: SerializeField] public List<UIStatsInfo> ListUIStatsInfo { get; private set; }
        [field: SerializeField] public Transform TfContent { get; private set; }
        [field: SerializeField] public UIResourceCoin UIResourceCoin { get; private set; }
        [field: SerializeField] public Button BtnMapInfo { get; private set; }
        [field: SerializeField] public Button BtnAdventureInfo { get; private set; }
        [field: SerializeField] public Button BtnGacha { get; private set; }
        [field: SerializeField] public Button BtnX2Ads { get; private set; }
        [field: SerializeField] public Button BtnQuest { get; private set; }
        
        
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
        private MiniPool<UIStatsInfo> m_poolUI = new();
        [field: SerializeField] public UIStatsInfo UIPrefabs { get; private set; }
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            SpawnUI();

            View.BtnMapInfo.SetOnClickDestination(OnClickBtnMapInfo);
            View.BtnAdventureInfo.SetOnClickDestination(OnClickBtnAdventureInfo);
            View.BtnGacha.SetOnClickDestination(OnClickBtnGacha);
            View.BtnQuest.SetOnClickDestination(OnClickBtnQuest);
            //View.BtnX2Ads.SetOnClickDestination(OnClickBtnX2Ads);
            //Model.CurrentUnlockedId.ReactiveProperty.Subscribe(OnUnlockedIdChange).AddTo(View.MainView);
        }   
        public void SpawnUI()
        {
            m_poolUI.OnInit(UIPrefabs,10,View.TfContent);
            for (int i = 0; i < StatsUnlockGlobalConfig.Instance.statUnlocks.Count; i++)
            {
                StatUnlock statUnlock = StatsUnlockGlobalConfig.Instance.statUnlocks[i];
                UIStatsInfo uiStatsInfo = m_poolUI.Spawn(View.TfContent.position,Quaternion.identity);
                uiStatsInfo.Setup(statUnlock.type);
                View.ListUIStatsInfo.Add(uiStatsInfo);
            }
        }
        public void OnUnlockedIdChange(int id)
        {
            //for (int i = 0; i < StatsUnlockGlobalConfig.Instance.statUnlocks.Count; i++)
            //{
            //    StatUnlock statUnlock = StatsUnlockGlobalConfig.Instance.statUnlocks[i];
            //    UIStatsInfo uiStatsInfo = View.ListUIStatsInfo[i];
            //    uiStatsInfo.Setup(statUnlock.type);
            //}
        }
        private void OnClickBtnMapInfo(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ActivityMapInfo));
            ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options);
        }
        private void OnClickBtnAdventureInfo(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ActivityAdventureInfo));
            ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options);
        }
        private void OnClickBtnGacha(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ModalGachaTreasure));
            ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
        }
        private void OnClickBtnX2Ads(Unit _)
        {
            
        }
        private void OnClickBtnQuest(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ModalQuestAchivement));
            ModalContainer.Find(ContainerKey.Modals).PushAsync(options);
        }
    }
}