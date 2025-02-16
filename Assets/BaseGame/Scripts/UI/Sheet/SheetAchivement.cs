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

public class SheetAchivement : Sheet, ISetupAble
{
    public static class Events
    {
        public static Action<Achivement, Action> ClaimAchivement { get; set; }
    }
    private AchivementManager achivementManager => AllQuestManager.Instance.AchivementManager;
    public MiniPool<UIAchivementInfo> poolUI = new();
    [field: SerializeField] public Transform TfParentUI { get; private set; }
    [field: SerializeField] public UIAchivementInfo UIAchivementInfoPrefab { get; private set; }
    [field: SerializeField] public List<UIAchivementInfo> ListUIInfo { get; set; }
    private Action m_SubAction;
    private Achivement m_Achivement;
    public override async UniTask Initialize(Memory<object> args)
    {
        int num = achivementManager.GetNumAchivement();
        for (int i = 0; i < num; i++)
        {
            UIAchivementInfo ui = poolUI.Spawn(TfParentUI.position, Quaternion.identity);
            ui.Setup(achivementManager.GetAchivement(i));
            ListUIInfo.Add(ui);
        }
        for (int i = 0; i < achivementManager.Data.EachAchivementsData.Count; i++)
        {
            //Model.QuestsData[i].ReactiveProperty.Subscribe(OnQuestDataChange).AddTo(View.MainView);
            ReactiveValue<EachAchivementData> data = achivementManager.Data.EachAchivementsData[i];
            data.ReactiveProperty
                .CombineLatest(data.Value.Collect.ReactiveProperty, (q, c) => (q, c))
                .CombineLatest(data.Value.Level.ReactiveProperty, (qc, l) => (qc.q, qc.c, l))
                .Subscribe(OnDataChange)
                .AddTo(this);
        }
        Events.ClaimAchivement = OnClaimAchivement;
    }
    public void Setup()
    {
        poolUI.OnInit(UIAchivementInfoPrefab, 5, TfParentUI);
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
        Events.ClaimAchivement = null;
        return UniTask.CompletedTask;
    }
    public void SortUIInfo()
    {
        ListUIInfo = ListUIInfo.OrderByDescending(x =>x.IsClaimable).ThenBy(x => x.IsMaxLevel).ToList();
        ListUIInfo.ForEach((s, i) =>
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
    public void OnDataChange((EachAchivementData data, int cl, int level) value)
    {
        UIAchivementInfo ui = ListUIInfo.Find(x => x.GetId() == value.data.Id);
        ui.Setup(achivementManager.GetAchivement(value.data.Id));
        SortUIInfo();
    }
    public void OnClaimAchivement(Achivement quest, Action subAction)
    {
        //View.ParticleImage.Play();
        m_SubAction = subAction;
        m_Achivement = quest;
        OnClaimQuestCompleted();
    }
    public void OnClaimQuestCompleted()
    {
        AllQuestManager.Instance.ClaimAchivement(m_Achivement);
        m_SubAction?.Invoke();
        SortUIInfo();
    }
}
