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
using TW.Utility.CustomType;

[Serializable]
public class ModalTreasureInfoContext 
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
        [field: SerializeField] public int TreasureId { get; private set; }
        [field: SerializeField] public TreasureConfig TreasureConfig { get; private set; }
        [field: SerializeField] public EachTreasureUpgradeData TreasureUpgradeData { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            TreasureId = (int)args.Span[0];
            TreasureConfig = TreasureManager.Instance.GetTreasureConfig(TreasureId);
            TreasureUpgradeData = TreasureManager.Instance.GetEachTreasureUpgradeData(TreasureConfig.Id);
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Image ImgIcon { get; private set; }
        [field: SerializeField] public Image ImgRarity { get; private set; }

        [field: SerializeField] public TextMeshProUGUI TxtName { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtPiece { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtRarity { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtResourceUpgrade { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAttackDamage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtHealthPoint { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtAtkSpd { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtCriticalRate { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtCriticalDamage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtMagicDmg { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtMainAbility { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtUpgradeAbility6 { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtUpgradeAbility12 { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtArcanePointRequire { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtLuckPointRequire { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtFuryPointRequire { get; private set; }

        [field: SerializeField] public Slider SliderPiece { get; private set; }

        //[field: SerializeField] public List<UIHeroAbilityInventoryInfo> ListUIHeroAbilityInventoryInfo { get; private set; }
        //[field: SerializeField] public List<UIHeroUpgradeInventoryInfo> ListUIHeroUpgradeInventoryInfo { get; private set; }

        [field: SerializeField] public Button BtnUpgrade { get; private set; }
        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public Button BtnEquip { get; private set; }
        [field: SerializeField] public Button BtnUnEquip { get; private set; }
        [field: SerializeField] public GameObject GOLockLevel6 { get; private set; }
        [field: SerializeField] public GameObject GOLockLevel12 { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
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
            SetupStaticUI();
            SetupMainAbility();
            SetupUpgradeAbility();

            Model.TreasureUpgradeData.Level.ReactiveProperty.Subscribe(OnLevelTreasureChange).AddTo(View.MainView);
            if(Model.TreasureConfig.Rarity != Rarity.Mythic)
                Model.TreasureUpgradeData.Piece.ReactiveProperty.Subscribe(OnPieceTreasureNormalChange).AddTo(View.MainView);
            else
                PlayerResourceData.Instance.GetGameResource(GameResource.Type.MythicStone).ReactiveAmount.ReactiveProperty
                    .Subscribe(OnPieceTreasureMythicChange).AddTo(View.MainView);
            PlayerResourceData.Instance.GetGameResource(GameResource.Type.Coin).ReactiveAmount.ReactiveProperty
                .Subscribe(OnResourceUpgradeChange).AddTo(View.MainView);
            //TODO Add subscribe resource
            //Model.TreasureUpgradeData.Level.ReactiveProperty
            //    .CombineLatest(Model.TreasureUpgradeData.Piece.ReactiveProperty, (level, piece) => (level, piece))
            //    .Subscribe(OnLevelAndPieceChange);

            View.BtnUpgrade.SetOnClickDestination(OnClickBtnUpgrade);
            View.BtnEquip.SetOnClickDestination(OnClickBtnEquip);
            View.BtnUnEquip.SetOnClickDestination(OnClickBtnUnEquip);
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
        }
        private void SetupStaticUI()
        {
            View.TxtName.text = $"{Model.TreasureConfig.Name}";
            View.TxtRarity.text = $"{Model.TreasureConfig.Rarity.ToString()}";
            View.TxtArcanePointRequire.text = $"{Model.TreasureConfig.ArcanePoint.ToString()}";
            View.TxtLuckPointRequire.text = $"{Model.TreasureConfig.LuckPoint.ToString()}";
            View.TxtFuryPointRequire.text = $"{Model.TreasureConfig.FuryPoint.ToString()}";
            View.TxtMainAbility.text = $"{Model.TreasureConfig.MainAbilityDescription}";
            View.TxtUpgradeAbility6.text = $"{Model.TreasureConfig.UpgradeAbility6Description}";
            View.TxtUpgradeAbility12.text = $"{Model.TreasureConfig.UpgradeAbility12Description}";
        }
        private void SetupMainAbility()
        {
            //for (int i = 0; i < Model.HeroConfigData.AbilityPower.Length; i++)
            //{
            //    View.ListUIHeroAbilityInventoryInfo[i].Setup(Model.HeroConfigData.AbilityPower[i]);
            //}
        }
        private void SetupUpgradeAbility()
        {
            //for (int i = 0; i < Model.HeroConfigData.AbilityPower.Length; i++)
            //{
            //    View.ListUIHeroUpgradeInventoryInfo[i].Setup(Model.HeroConfigData.AbilityPower[i]);
            //}
        }
        //private void OnLevelAndPieceChange((int level, int piece) x)
        //{
        //    //OnLevelTreasureChange(x.level);
        //    //OnPieceTreasureChange(x.piece);
        //    if(x.level >= 1)
        //    {
        //        View.BtnUpgrade.gameObject.SetActive(true);
        //        View.BtnEquip.gameObject.SetActive(true);
        //        View.BtnUnEquip.gameObject.SetActive(true);
        //        View.GOLockLevel6.SetActive(false);
        //        View.GOLockLevel12.SetActive(false);
        //        //View.BtnLock.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        View.BtnUpgrade.gameObject.SetActive(false);
        //        View.BtnEquip.gameObject.SetActive(false);
        //        View.BtnUnEquip.gameObject.SetActive(false);
        //        View.GOLockLevel6.SetActive(true);
        //        View.GOLockLevel12.SetActive(true);
        //        //View.BtnLock.gameObject.SetActive(true);
        //    }
        //}
        private void OnLevelTreasureChange(int level)
        {
            if (level >= 1)
            {
                View.BtnUpgrade.gameObject.SetActive(true);
                View.BtnEquip.gameObject.SetActive(!TreasureManager.Instance.IsTreasureEuiped(Model.TreasureId));
                View.BtnUnEquip.gameObject.SetActive(TreasureManager.Instance.IsTreasureEuiped(Model.TreasureId));
                View.GOLockLevel6.SetActive(level<6);
                View.GOLockLevel12.SetActive(level<12);
                View.TxtResourceUpgrade.text = $"{TreasureManager.Instance.GetCostUpgradeTreasure(Model.TreasureId)}";
                //View.BtnLock.gameObject.SetActive(false);
            }
            else
            {
                View.BtnUpgrade.gameObject.SetActive(false);
                View.BtnEquip.gameObject.SetActive(false);
                View.BtnUnEquip.gameObject.SetActive(false);
                View.GOLockLevel6.SetActive(true);
                View.GOLockLevel12.SetActive(true);
                //View.BtnLock.gameObject.SetActive(true);
            }
            View.TxtLevel.text = $"Lv.{level}";

            //View.TxtAttackDamage.text = $"{Model.HeroConfigData.AttackDamage.ToString()}";
            //View.TxtHealthPoint.text = $"{Model.HeroConfigData.HealthPoint.ToString()}";
            //TODO Add resource upgrade text
        }
        private void OnPieceTreasureNormalChange(int piece)
        {
            if (Model.TreasureConfig.Rarity == Rarity.Mythic) return;
            if (Model.TreasureUpgradeData.Level >= 1)
            {
                View.BtnUpgrade.interactable = TreasureManager.Instance.IsUpgradeAbleTreasure(Model.TreasureConfig.Id);
                View.SliderPiece.value = TreasureManager.Instance.GetTreasurePieceProcess(Model.TreasureConfig.Id);
                View.TxtPiece.text = $"{piece}/{TreasureManager.Instance.GetPieceNeededUpgradeTreasure(Model.TreasureConfig.Id)}";
            }
            else
            {
                View.BtnUpgrade.interactable = false;
                View.SliderPiece.value = 0;
                View.TxtPiece.text = $"0/0";
            }

        }
        private void OnPieceTreasureMythicChange(BigNumber piece)
        {
            if (Model.TreasureConfig.Rarity != Rarity.Mythic) return;
            if (Model.TreasureUpgradeData.Level >= 1)
            {
                View.BtnUpgrade.interactable = TreasureManager.Instance.IsUpgradeAbleTreasure(Model.TreasureConfig.Id);
                View.SliderPiece.value = TreasureManager.Instance.GetTreasurePieceProcess(Model.TreasureConfig.Id);
                View.TxtPiece.text = $"{piece.ToStringUI()}/{TreasureManager.Instance.GetPieceNeededUpgradeTreasure(Model.TreasureConfig.Id)}";
            }
            else
            {
                View.BtnUpgrade.interactable = false;
                View.SliderPiece.value = 0;
                View.TxtPiece.text = $"0/0";
            }
        }
        private void OnResourceUpgradeChange(BigNumber resource)
        {
            //View.TxtResource.text = $"{resource}";
            if (Model.TreasureUpgradeData.Level >= 1)
            {
                View.BtnUpgrade.interactable = TreasureManager.Instance.IsUpgradeAbleTreasure(Model.TreasureConfig.Id);
            }
            else
            {
                View.BtnUpgrade.interactable = false;
            }
        }
        private void OnClickBtnUpgrade(Unit _)
        {
            TreasureManager.Instance.UpgradeTreasure(Model.TreasureConfig.Id);
        }
        private async UniTask OnClickBtnEquip()
        {
            //HeroManager.Instance.LockHero(Model.HeroConfigData.HeroId);
            //TreasureManager.Instance.Euip(Model.TreasureConfig.Id);
            await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            ViewOptions options = new ViewOptions(nameof(ModalTreasureDeck));
            await ModalContainer.Find(ContainerKey.Modals).PushAsync(options,Model.TreasureId);
        }
        private async UniTask OnClickBtnUnEquip()
        {
            //HeroManager.Instance.UnlockHero(Model.HeroConfigData.HeroId);
            TreasureManager.Instance.UnEquipTreasureBySlotId(TreasureManager.Instance.GetSlotIdByTreasureId(Model.TreasureConfig.Id));
            await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
    }
}