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
public class ModalGachaRewardContext 
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
        [field: SerializeField] public List<GachaTreasureReward> ListReward { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            ListReward = (List<GachaTreasureReward>)args.Span[0];
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public MiniPool<UITreasure> PoolUITreasure { get; private set; } = new();
        [field: SerializeField] public UITreasure UIHeroInventory { get; private set; }
        [field: SerializeField] public Transform TfUIContainer { get; private set; }
        [field: SerializeField] public List<UITreasure> ListUIReward { get; private set; } = new();

        public UniTask Initialize(Memory<object> args)
        {
            PoolUITreasure.OnInit(UIHeroInventory, 10, TfUIContainer);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            View.BtnClose.SetOnClickDestination(OnClickBtnClose);

            SetUpUI();
        }
        private void SetUpUI()
        {
            for (int i = 0; i < Model.ListReward.Count; i++)
            {
                if (IsContainHero(Model.ListReward[i].RewardTreasureConfig.Id)) continue;
                UITreasure uiReward = View.PoolUITreasure.Spawn(View.TfUIContainer.position, Quaternion.identity);
                uiReward.Setup(Model.ListReward[i].RewardTreasureConfig.Id, false);
                View.ListUIReward.Add(uiReward);
            }
        }
        private bool IsContainHero(int Id)
        {
            for (int i = 0; i < View.ListUIReward.Count; i++)
            {
                if (View.ListUIReward[i].TreasureConfig.Id == Id)
                {
                    return true;
                }
            }
            return false;
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
    }
}