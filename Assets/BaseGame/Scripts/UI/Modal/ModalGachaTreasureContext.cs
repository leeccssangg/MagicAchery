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
using UnityEditor.SceneManagement;
using Lofelt.NiceVibrations;
using DG.Tweening;
using TW.UGUI.Core.Activities;

[Serializable]
public class ModalGachaTreasureContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<GachaTreasureReward> SpawnUIGachaReward { get; set; }
        public static Action CheckCompletedAnim { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public ReactiveValue<int> StageSummon { get; private set; }
        [field: SerializeField] public ReactiveValue<int> NumSummoned { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            TreasureManager.Instance.SetCurrentStageSummon(TreasureGachaGlobalConfig.Instance.StartStage);
            StageSummon = TreasureManager.Instance.CurStageGacha;
            NumSummoned = new(0);
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public List<UITreasureGacha> ListUIHeroGacha { get; private set; }
        [field: SerializeField] public List<UIGachaReward> ListUIGachaReward { get; private set; }

        [field: SerializeField] public GameObject PanelGachaButton { get; private set; }
        [field: SerializeField] public GameObject PanelGachaReward { get; private set; }

        [field: SerializeField] public Transform TfUIHeroGachaContainer { get; private set; }
        [field: SerializeField] public Transform TfUIGachaRewardContainer { get; private set; }
        [field: SerializeField] public Transform TfStartPos { get; private set; }
        [field: SerializeField] public Transform TfEndPos { get; private set; }
        [field: SerializeField] public Transform TfFailedPos { get; private set; }

        [field: SerializeField] public Button BtnGacha10 { get; private set; }
        [field: SerializeField] public Button BtnGacha1 { get; private set; }
        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public Button BtnFree { get; private set; }

        [field: SerializeField] public TextMeshProUGUI TxtNumSummon { get; private set; }
        public UniTask Initialize(Memory<object> args)
        {
            ActiveFalseAllUI();
            return UniTask.CompletedTask;
        }
        public void ActiveFalseAllUI()
        {
            foreach (UITreasureGacha ui in ListUIHeroGacha)
            {
                ui.gameObject.SetActive(false);
            }
            foreach (UIGachaReward ui in ListUIGachaReward)
            {
                ui.gameObject.SetActive(false);
                ui.PlayAnim(false);
            }
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        [field: SerializeField] public int NumTreasureAppearance { get; private set; }
        [field: SerializeField] public List<GachaTreasureConfig> ListSummonTreasureConfig { get; private set; }
        [field: SerializeField] public List<GachaTreasureReward> ListSummonTreasureReward { get; private set; }
        [field: SerializeField] public int NumGacha { get; private set; }
        [field: SerializeField] public int NumAnimCompleted { get; private set; }
        [field: SerializeField] public List<GachaTreasureReward> ListReward { get; private set; }



        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            TreasureManager.Instance.SetCurrentStageSummon(TreasureGachaGlobalConfig.Instance.StartStage);
            Model.NumSummoned.ReactiveProperty.CombineLatest(Model.StageSummon.ReactiveProperty, (nu, st) => (nu, st))
                .Subscribe(_ => UpdateTxtSummoned()).AddTo(View.MainView);

            Events.SpawnUIGachaReward = SpawnUIGachaReward;
            Events.CheckCompletedAnim = CheckAllAnimCompleted;

            View.BtnGacha10.SetOnClickDestination(OnClickBtnGacha10);
            View.BtnGacha1.SetOnClickDestination(OnClickBtnGacha1);
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
            View.BtnFree.SetOnClickDestination(OnClickBtnFree);
            View.BtnFree.gameObject.SetActive(false);
            View.PanelGachaReward.gameObject.SetActive(false);
            ListReward = new();
        }
        public UniTask Cleanup(Memory<object> args)
        {
            Events.SpawnUIGachaReward = null;
            Events.CheckCompletedAnim = null;
            return UniTask.CompletedTask;
        }
        public void DidPushEnter(Memory<object> args)
        {
            //foreach(UIGachaReward ui in View.ListUIGachaReward)
            //{
            //    ui.PlayAnim(false);
            //}
        }
        private void OnClickBtnGacha10(Unit _)
        {
            StartGacha(10);
        }
        private void OnClickBtnGacha1(Unit _)
        {
            StartGacha(1);
        }
        private void OnClickBtnFree(Unit _)
        {
            //View.PoolUIGachaReward.Collect();
            //View.PoolUIHeroGacha.Collect();
            View.BtnFree.gameObject.SetActive(false);
            TreasureManager.Instance.SetCurrentStageSummon(Model.StageSummon.Value + 1);
            Model.NumSummoned.Value = 0;
            StartGacha(NumGacha);
        }
        private void OnClickBtnClose(Unit _)
        {
            View.MainView.interactable = false;
            ClickButtonCloseComplete().Forget();
        }
        private async UniTask ClickButtonCloseComplete()
        {
            await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            if (ListReward.Count > 0)
            {
                ViewOptions options = new ViewOptions(nameof(ModalGachaReward));
                await ModalContainer.Find(ContainerKey.Modals).PushAsync(options, ListReward);
                TreasureManager.Instance.ClaimGachaReward(ListReward);
            }
        }
        private void OnNumSummonedChange(int num)
        {
            View.TxtNumSummon.SetText($"{num}/{Model.StageSummon}");
        }
        private void StartGacha(int numGacha)
        {
            //View.PoolUIHeroGacha.Release();
            //View.PoolUIGachaReward.Release();
            NumGacha = numGacha;
            NumAnimCompleted = 0;
            View.ActiveFalseAllUI();
            View.PanelGachaButton.SetActive(false);
            View.PanelGachaReward.SetActive(true);
            View.BtnClose.interactable = false;
            NumTreasureAppearance = TreasureManager.Instance.GetNumAppearance();
            ListSummonTreasureConfig = TreasureManager.Instance.GetListGachaTreasureConfig(NumTreasureAppearance);
            ListSummonTreasureReward = TreasureManager.Instance.GetListGachaTreasureReward(ListSummonTreasureConfig, numGacha);
            //NumAnimCompleted = GetNumAccquired();
            foreach (GachaTreasureReward reward in ListSummonTreasureReward)
            {
                if (reward.IsAcquired)
                    ListReward.Add(reward);
            }
            int tmp = 0;
            for(int i = View.ListUIGachaReward.Count-1; i>= 0;i--)
            {
                for(int j = tmp;j< ListSummonTreasureReward.Count;j++)
                {
                    if (ListSummonTreasureReward[j].IsAcquired)
                    {
                        //View.ListUIGachaReward[i].gameObject.SetActive(true);
                        View.ListUIGachaReward[i].Setup(ListSummonTreasureReward[j]);
                        tmp=j+1;
                        break;
                    }
                }
            }

            InitViewGacha();
        }
        private void InitViewGacha()
        {
            //View.PoolUIHeroGacha.Collect();
            //View.PoolUIGachaReward.Collect();
            //foreach()
            //foreach (UIGachaReward ui in View.ListUIGachaReward)
            //{
            //    ui.gameObject.SetActive(false);
            //}
            for (int i = 0; i < ListSummonTreasureReward.Count; i++)
            {
                GachaTreasureReward reward = ListSummonTreasureReward[i];
                UITreasureGacha ui = View.ListUIHeroGacha[i];
                if (reward.IsAcquired)
                    ui.Setup(reward, View.TfStartPos, View.TfEndPos);
                else
                    ui.Setup(reward, View.TfStartPos, View.TfFailedPos);
                //if (i == ListSummonHeroReward.Count - 1)
                //    uiHeroGacha.SetupActionLastSummoned(OnGachaCompleted);
                ui.gameObject.SetActive(true);
                ui.InitDelay(i * 0.4f);
            }
        }
        //private bool IsAllAnimCompleted()
        //{
        //    for (int i = 0; i < View.ListUIHeroGacha.Count; i++)
        //    {
        //        if (ListSummonHeroReward[i].Is)
        //            return false;
        //    }
        //}
        private void CheckAllAnimCompleted()
        {
            NumAnimCompleted += 1;
            if (NumAnimCompleted == ListSummonTreasureConfig.Count)
            {
                OnGachaCompleted();
            }
        }
        private void SpawnUIGachaReward(GachaTreasureReward reward)
        {
            Debug.Log("SpawnUIGacha");
            Model.NumSummoned.Value += 1;
            //int uiIndex = Model.NumSummoned.Value - 1;
            //UIGachaReward uiGachaReward = View.ListUIGachaReward[uiIndex];
            //uiGachaReward.Setup(reward);
            for(int i=0;i< View.ListUIGachaReward.Count;i++)
            {
                if (View.ListUIGachaReward[i].Reward == reward)
                {
                    View.ListUIGachaReward[i].gameObject.SetActive(true);
                    View.ListUIGachaReward[i].PlayAnim(true);
                    break;
                }
            }

            //uiGachaReward.PlayAnim(true);
            //View.ListUIGachaReward.Add(uiGachaReward);
        }
        private void OnGachaCompleted()
        {
            if (Model.StageSummon.Value < 10 && Model.NumSummoned.Value >= Model.StageSummon.Value)
            {
                View.BtnFree.gameObject.SetActive(true);
                View.BtnClose.interactable = true;
            }
            else
            {
                DOVirtual.DelayedCall(3f, () =>
                {
                    View.PanelGachaReward.gameObject.SetActive(false);
                    View.PanelGachaButton.SetActive(true);
                    TreasureManager.Instance.SetCurrentStageSummon(TreasureGachaGlobalConfig.Instance.StartStage);
                    Model.NumSummoned.Value = 0;
                    View.BtnClose.interactable = true;
                });
            }

        }
        private void UpdateTxtSummoned()
        {
            View.TxtNumSummon.SetText($"{Model.NumSummoned.Value}/{Model.StageSummon.Value}");
        }
    }
}