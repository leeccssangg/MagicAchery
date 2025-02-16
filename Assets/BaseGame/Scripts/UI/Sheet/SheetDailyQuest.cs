using System.Collections.Generic;
using R3;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Sheets;
using UnityEngine;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Modals;
using System;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using TMPro;
using Pextension;
using System.Linq;
using Sirenix.Utilities;
using TW.Reactive.CustomComponent;
using UnityEditor.PackageManager.Requests;
using AssetKits.ParticleImage;
using MoreMountains.Tools;
using DG.Tweening;

public class SheetDailyQuest : Sheet, ISetupAble
{
    public static class Events
    {
        public static Action<DailyQuest, Action> ClaimQuest { get; set; }
    }
    private DailyQuestManager dailyQuestManager => AllQuestManager.Instance.DailyQuestManager;
    private MiniPool<UIDailyQuestInfo> poolUIDailyQuestInfo = new();
    [field: SerializeField] public Transform TfParentInfo { get; private set; }
    [field: SerializeField] public UIDailyQuestInfo UIDailyQuestInfoPrefab { get; private set; }
    [field: SerializeField] public Slider SliderProcess { get; private set; }

    [field: SerializeField] public List<UIDailyQuestPointGift> ListUIQuestPointGift { get; set; }
    [field: SerializeField] public List<UIDailyQuestInfo> ListUIQuestInfo { get; set; }

    [field: SerializeField] public TextMeshProUGUI TxtCountDown { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtPoint { get; private set; }
    private Action m_SubAction;
    private DailyQuest m_Quest;

    //[field: SerializeField] public List<TweenTransition> DailyGiftInfoOpenTweenTransitionList { get; set; } = new();
    //[field: SerializeField] public ScrollRect Scroll { get; private set; }
    //[field: SerializeField] public ParticleImage ParticleImage { get; private set; }
    public override async UniTask Initialize(Memory<object> args)
    {
        int numQuest = dailyQuestManager.GetNumDailyQuest();
        //poolUIDailyQuestInfo.Collect();
        for (int i = 0; i < numQuest; i++)
        {
            UIDailyQuestInfo ui = poolUIDailyQuestInfo.Spawn(TfParentInfo.position, Quaternion.identity);
            ui.Setup(dailyQuestManager.GetQuest(i));
            ListUIQuestInfo.Add(ui);
        }
        SortUIDailyQuestInfo();
        Events.ClaimQuest = OnClaimQuest;
        dailyQuestManager.DailyQuestData.CurrentPoint.ReactiveProperty.Subscribe(OnBonusPointChange).AddTo(this);
        dailyQuestManager.DailyQuestData.CurrentStage.ReactiveProperty.Subscribe(OnStagePointChange).AddTo(this);
        AllQuestManager.Instance.TimeToNextDay.ReactiveProperty.Subscribe(OnCountDown).AddTo(this);
        //Model.QuestsData.ForEach(questData => questData.ReactiveProperty.Subscribe(OnQuestDataChange).AddTo(View.MainView));
        for (int i = 0; i < dailyQuestManager.DailyQuestData.QuestsData.Count; i++)
        {
            //Model.QuestsData[i].ReactiveProperty.Subscribe(OnQuestDataChange).AddTo(View.MainView);
            ReactiveValue<QuestData> data = dailyQuestManager.DailyQuestData.QuestsData[i];
            data.ReactiveProperty
                .CombineLatest(data.Value.Collect.ReactiveProperty, (q, c) => (q, c))
                .CombineLatest(data.Value.IsClaimed.ReactiveProperty, (qc, icd) => (qc.q, qc.c, icd))
                .Subscribe(OnQuestDataChange)
                .AddTo(this);
        }
    }
    public void Setup()
    {
        poolUIDailyQuestInfo.OnInit(UIDailyQuestInfoPrefab, 5, TfParentInfo);
    }
    public override UniTask WillEnter(Memory<object> args)
    {
        
        return UniTask.CompletedTask;
    }
    public override void DidEnter(Memory<object> args)
    {
        
    }
    public override UniTask Cleanup(Memory<object> args)
    {
        Events.ClaimQuest = null;
        return UniTask.CompletedTask;
    }
    public void SortUIDailyQuestInfo()
    {
        ListUIQuestInfo = ListUIQuestInfo.OrderByDescending(x => x.IsClaimable).ThenBy(x => x.IsClaimed).ToList();
        ListUIQuestInfo.ForEach((s, i) =>
        {
            s.Transform.SetSiblingIndex(i);
        });
        //View.DailyGiftInfoOpenTweenTransitionList.ForEach(t => t?.Kill());
        //View.DailyGiftInfoOpenTweenTransitionList.Clear();
        //View.ListUIQuestInfo.ForEach((s, i) =>
        //{
        //    View.DailyGiftInfoOpenTweenTransitionList.Add(s.m_Tween);
        //    s.SetUpDelay(i + 1);
        //});
        //foreach (var tween in View.DailyGiftInfoOpenTweenTransitionList)
        //{
        //    tween.Play();
        //}
    }
    public void SetupPointGift()
    {
        for (int i = 0; i < ListUIQuestPointGift.Count; i++)
        {
            ListUIQuestPointGift[i].Setup(dailyQuestManager.GetDailyQuestConfigs().GetStageReward()[i], OnClaimQuestPointGift);
        }
    }

    public void OnClickClose(Unit _)
    {
        ModalContainer.Find(ContainerKey.Modals).Pop(true);
        ActivityGiftDetailContext.Events.HideActivity?.Invoke();
    }
    public void OnQuestDataChange(QuestData questDatas)
    {
        //for (int i = 0; i < View.ListUIQuestInfo.Count; i++)
        //{
        //    View.ListUIQuestInfo[i].Setup(dailyQuestManager.GetQuest(i));
        //}
        //SortUIDailyQuestInfo();
        UIDailyQuestInfo ui = ListUIQuestInfo.Find(x => x.GetQuestId() == questDatas.Id);
        ui.UpdateData();
        SortUIDailyQuestInfo();
    }
    public void OnQuestDataChange((QuestData questDatas, int cl, int icd) value)
    {
        UIDailyQuestInfo ui = ListUIQuestInfo.Find(x => x.GetQuestId() == value.questDatas.Id);
        ui.Setup(dailyQuestManager.GetQuest(value.questDatas.Id));
        SortUIDailyQuestInfo();
    }
    public void OnBonusPointChange(int i)
    {
        SetupPointGift();
        SliderProcess.DOValue((float)i / dailyQuestManager.GetMaxDailyQuestPoint(), 0.5f);
        TxtPoint.SetText($"{i}");
    }
    public void OnStagePointChange(int i)
    {
        SetupPointGift();
    }
    public void OnClaimQuest(DailyQuest quest, Action subAction)
    {
        //View.ParticleImage.Play();
        m_SubAction = subAction;
        m_Quest = quest;
        OnClaimQuestCompleted();
    }
    public void OnClaimQuestCompleted()
    {
        AllQuestManager.Instance.ClaimDailyQuest(m_Quest);
        m_SubAction?.Invoke();
    }
    public void OnClaimQuestPointGift(List<GameResource> reward, Vector3 pos)
    {
        //if (reward[0].ResourceType == GameResource.Type.Coin)
        //    View.UIResourceCoin.SetDelayIncreaseValue(pos, 10);
        //if (reward[0].ResourceType == GameResource.Type.Gem)
        //    View.UIResourceGem.SetDelayIncreaseValue(pos, 10);
        PlayerResourceData.Instance.ClaimListResources(reward, 1);
        OnClaimQuestPointCompleted();
    }
    public void OnClaimQuestPointCompleted()
    {
        AllQuestManager.Instance.ClaimDailyQuestStageReward();
    }
    public void OnCountDown(TimeSpan t)
    {
        TxtCountDown.SetText(string.Format("{0:D1}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds)); ;
    }

}
