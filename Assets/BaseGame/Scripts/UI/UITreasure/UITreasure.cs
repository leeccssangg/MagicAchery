using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using TW.Reactive.CustomComponent;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Modals;
using TW.Utility.CustomComponent;
using Lofelt.NiceVibrations;
using TW.Utility.CustomType;

public class UITreasure : ACachedMonoBehaviour
{
    [field: SerializeField] public TreasureConfig TreasureConfig { get; private set; }
    [field: SerializeField] public EachTreasureUpgradeData TreasureUpgradeData { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtPiece { get; private set; }
    [field: SerializeField] public Image ImgRarity { get; private set; }
    [field: SerializeField] public Image ImgHeroIcon { get; private set; }
    [field: SerializeField] public Slider SliderExp { get; private set; }
    [field: SerializeField] public Button BtnInfo { get; private set; }
    [field: SerializeField] public Button BtnEquip { get; private set; }
    [field: SerializeField] public Button BtnUnEquip { get; private set; }
    [field: SerializeField] public string FormatText { get; private set; }
    [field: SerializeField] public bool IsInventory { get; private set; }
    private IDisposable _disposableLevel;
    private IDisposable _disposablePiece;

    private void Awake()
    {
        BtnInfo.SetOnClickDestination(OnClickBtnInfo);
        BtnEquip.SetOnClickDestination(OnClickBtnEquip);
        BtnUnEquip.SetOnClickDestination(OnClickBtnUnEquip);
    }
    public void Setup(int id, bool isInventory)
    {
        IsInventory = isInventory;
        TreasureConfig = TreasureManager.Instance.GetTreasureConfig(id);
        TreasureUpgradeData = TreasureManager.Instance.GetEachTreasureUpgradeData(TreasureConfig.Id);
        TxtName.SetText(TreasureConfig.Name);
        TxtLevel.SetText(TreasureUpgradeData.Level.ToString());
        TxtPiece.SetText(TreasureUpgradeData.Piece.ToString());
        //ImgRarity.sprite = HeroConfigData.RarityIcon;
        //ImgHeroIcon.sprite = TreasureConfig.SpriteIcon;
        if (TreasureUpgradeData.Level >= 1)
        {
            SliderExp.gameObject.SetActive(true);
            SliderExp.value = TreasureManager.Instance.GetTreasurePieceProcess(TreasureConfig.Id);
        }
        else
        {
            SliderExp.gameObject.SetActive(false);
        }

        _disposableLevel?.Dispose();
        _disposablePiece?.Dispose();
        _disposableLevel = TreasureUpgradeData.Level.ReactiveProperty.Subscribe(OnLevelHeroChange).AddTo(this);
        _disposablePiece = TreasureUpgradeData.Piece.ReactiveProperty.Subscribe(OnPieceHeroChange).AddTo(this);
        InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[0].ReactiveProperty
            .CombineLatest(InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[1].ReactiveProperty,(s0,s1)=>(s0,s1))
            .CombineLatest(InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[2].ReactiveProperty,(s01,s2)=>(s01,s2))
            .CombineLatest(InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[3].ReactiveProperty,(s012,s3)=>(s012,s3))
            .CombineLatest(InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[4].ReactiveProperty,(s0123,s4)=>(s0123,s4))
            .Subscribe(OnSlotIdChange).AddTo(this);
    }
    private void OnLevelHeroChange(int level)
    {
        //TxtLevel.gameObject.SetActive(level >= 1);
        TxtLevel.SetText(level.ToString());
        BtnEquip.gameObject.SetActive(level >= 1);
    }
    private void OnPieceHeroChange(int piece)
    {
        if (TreasureUpgradeData.Level < 1)
        {
            SliderExp.gameObject.SetActive(false);
            return;
        }
        SliderExp.gameObject.SetActive(true);
        SliderExp.value = TreasureManager.Instance.GetTreasurePieceProcess(TreasureConfig.Id);
        TxtPiece.SetText($"{piece}/{TreasureManager.Instance.GetPieceNeededUpgradeTreasure(TreasureConfig.Id)}");
    }
    private void OnClickBtnInfo(Unit _)
    {
        ViewOptions option = new ViewOptions(nameof(ModalTreasureInfo));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(option, TreasureConfig.Id);
    }
    private void OnClickBtnEquip(Unit _)
    {
        ViewOptions option = new ViewOptions(nameof(ModalTreasureDeck));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(option, TreasureConfig.Id);
    }
    private void OnClickBtnUnEquip(Unit _)
    {
        TreasureManager.Instance.UnEquipTreasureByTreasureId(TreasureConfig.Id);
    }
    private void OnSlotIdChange(((((int s0,int s1),int s2),int s3), int s4) slotData)
    {
        if(TreasureManager.Instance.GetEachTreasureUpgradeData(TreasureConfig.Id).Level < 1)
        {
            BtnEquip.gameObject.SetActive(false);
            BtnUnEquip.gameObject.SetActive(false);
            return;
        }
        BtnEquip.gameObject.SetActive(IsInventory && !TreasureManager.Instance.IsTreasureEuiped(TreasureConfig.Id));
        BtnUnEquip.gameObject.SetActive(IsInventory && TreasureManager.Instance.IsTreasureEuiped(TreasureConfig.Id));
    }
}
