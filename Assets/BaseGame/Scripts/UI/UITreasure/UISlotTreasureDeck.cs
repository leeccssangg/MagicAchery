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

public class UISlotTreasureDeck : ACachedMonoBehaviour
{
    [field: SerializeField] public int SlotId { get; private set; }
    [field: SerializeField] public ReactiveValue<int> TreasureID { get; private set; } = new();
    [field: SerializeField] public Image ImgTreasureIcon { get; private set; }
    [field: SerializeField] public Image ImgRarity { get; private set; }
    [field: SerializeField] public Button BtnEquip { get; private set; }
    [field: SerializeField] public GameObject GOEquipable { get; private set; }

    private void Awake()
    {
        BtnEquip.SetOnClickDestination(OnClickBtnEquip);
    }
    public void Setup(int slotId, int newId)
    {
        SlotId = slotId;
        TreasureID.Value = InGameDataManager.Instance.InGameData.TreasureData.TreasureSlotData.SlotData[SlotId].Value;
        if (TreasureID < 0)
        {
            ImgTreasureIcon.gameObject.SetActive(false);
        }
        else
        {
            ImgTreasureIcon.gameObject.SetActive(true);
            //TreasureConfig treasureConfig = TreasureManager.Instance.GetTreasureConfig(TreasureID);
            //ImgTreasureIcon.sprite = treasureConfig.SpriteIcon;
            //ImgRarity.sprite = treasureConfig.RarityIcon;
        }
        GOEquipable.SetActive(TreasureManager.Instance.IsPlaceableRing(SlotId,newId));
        BtnEquip.interactable = TreasureManager.Instance.IsPlaceableRing(SlotId,newId);
    }
    private void OnClickBtnEquip(Unit _)
    {
        ModalTreasureDeckContext.Events.PlaceRing?.Invoke(SlotId);
    }
}
