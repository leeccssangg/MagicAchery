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
public class ScreenTreasureContext 
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
        [field: SerializeField] public List<UITreasure> ListUIHeroInventory { get; private set; }
        [field: SerializeField] public UITreasure UITreasurePrefab { get; private set; }
        [field: SerializeField] public Transform UITreasureContainer { get; private set; }
        [field: SerializeField] public List<UISlotTreasure> ListUISlotTreasure { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtRemainArcanePoint { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtRemainLuckPoint { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtRemainFuryPoint { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtMythicPiece { get; private set; }

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
        private MiniPool<UITreasure> _miniPool = new();

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            _miniPool.OnInit(View.UITreasurePrefab,10, View.UITreasureContainer);

            SetupUI();

            SubcribeOnPointChange();

            PlayerResourceData.Instance.GetGameResource(GameResource.Type.MythicStone).ReactiveAmount.ReactiveProperty.Subscribe(OnMythicPieceChange);
        }    
        private void SetupUI()
        {
            List<TreasureConfig> treasureConfigs = TreasurePoolGlobalConfig.Instance.TreasureConfigs;
            for (int i = 0; i < treasureConfigs.Count; i++)
            {
                UITreasure uiTreasure = _miniPool.Spawn(View.UITreasureContainer.position, Quaternion.identity);
                uiTreasure.Setup(treasureConfigs[i].Id,true);
                View.ListUIHeroInventory.Add(uiTreasure);
            }
            for(int i = 0; i < View.ListUISlotTreasure.Count; i++)
            {
                View.ListUISlotTreasure[i].Setup(i);
            }
        }
        private void SubcribeOnPointChange()
        {
            TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.ArcanePoint).ReactiveAmount.ReactiveProperty
                .CombineLatest(TreasureManager.Instance.RemainArcanePoint.ReactiveProperty, (a, b) => (a, b))
                .Subscribe(OnArcanePointChange);
            TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.LuckPoint).ReactiveAmount.ReactiveProperty
                .CombineLatest(TreasureManager.Instance.RemainLuckPoint.ReactiveProperty, (a, b) => (a, b))
                .Subscribe(OnLuckPointChange);
            TalentTreeManager.Instance.GetTalentStat(TalentStat.Type.FuryPoint).ReactiveAmount.ReactiveProperty
                .CombineLatest(TreasureManager.Instance.RemainFuryPoint.ReactiveProperty, (a, b) => (a, b))
                .Subscribe(OnFuryPointChange);
        }
        private void OnArcanePointChange((BigNumber a, BigNumber b) value)
        {
            View.TxtRemainArcanePoint.SetText($"{value.b.ToStringUI()}/{value.a.ToStringUI()}");
        }
        private void OnLuckPointChange((BigNumber a, BigNumber b) value)
        {
            View.TxtRemainLuckPoint.SetText($"{value.b.ToStringUI()}/{value.a.ToStringUI()}");
        }
        private void OnFuryPointChange((BigNumber a, BigNumber b) value)
        {
            View.TxtRemainFuryPoint.SetText($"{value.b.ToStringUI()}/{value.a.ToStringUI()}");
        }
        private void OnMythicPieceChange(BigNumber piece)
        {
            View.TxtMythicPiece.SetText($"{piece.ToStringUI()}");
        }
    }
}