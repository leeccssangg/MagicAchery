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

public class UISlotTreasure : ACachedMonoBehaviour
{
    [field: SerializeField] public int SlotId { get; private set; }
    [field: SerializeField] public ReactiveValue<int> TreasureID { get; private set; } = new();
    [field: SerializeField] public Image ImgTreasureIcon { get; private set; }
    [field: SerializeField] public Image ImgRarity { get; private set; }
    [field: SerializeField] public Button BtnUnEquip { get; private set; }
    [field: SerializeField] public Button BtnInfo { get; private set; }

    private void Awake()
    {
        BtnUnEquip.SetOnClickDestination(OnClickBtnUnEquip);
        BtnInfo.SetOnClickDestination(OnClickBtnInfo);
    }
    public void Setup(int slotId)
    {
        SlotId = slotId;
       //TreasureID.Value = InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[SlotId].Value;
        InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[SlotId].ReactiveProperty.Subscribe(OnTreasureIDChange).AddTo(this);
    }
    private void OnTreasureIDChange(int id)
    {
        TreasureID.Value = id;
        if(TreasureID < 0)
        {
            ImgTreasureIcon.gameObject.SetActive(false);
            BtnUnEquip.gameObject.SetActive(false);
            BtnInfo.gameObject.SetActive(false);
        }
        else
        {
            ImgTreasureIcon.gameObject.SetActive(true);
            BtnUnEquip.gameObject.SetActive(true);
            BtnInfo.gameObject.SetActive(true);
            TreasureConfig treasureConfig = TreasureManager.Instance.GetTreasureConfig(TreasureID);
            //ImgTreasureIcon.sprite = treasureConfig.SpriteIcon;
            //ImgRarity.sprite = treasureConfig.RarityIcon;
        }
    }
    private void OnClickBtnUnEquip(Unit _)
    {
        TreasureManager.Instance.UnEquipTreasureBySlotId(SlotId);
    }
    private void OnClickBtnInfo(Unit _)
    {
        ViewOptions options = new ViewOptions(nameof(ModalTreasureInfo));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(options,TreasureID.Value);
        //ScreenManager.Instance.ShowScreen<ScreenTreasureInfo>(new Memory<object>(TreasureID.Value));
    }
}
