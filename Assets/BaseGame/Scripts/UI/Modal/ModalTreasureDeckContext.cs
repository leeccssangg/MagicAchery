using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
//using UGUI.Core.Modals;
using System.Collections.Generic;
using Pextension;
using UnityEngine.UI;
using System.ComponentModel;
using DG.Tweening;
using TW.UGUI.Core.Modals;
using TW.UGUI.MVPPattern;
using Spine;

[Serializable]
public class ModalTreasureDeckContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<int> PlaceRing { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public int Id { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            Id = (int)args.Span[0];
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
        [field: SerializeField] public List<UISlotTreasureDeck> ListUISlotTreasureDeck { get; private set; }
        [field: SerializeField] public Image ImgIcon { get; private set; }
        
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

            Events.PlaceRing = PlaceRing;

            Setup();

            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
        }
        public UniTask Cleanup(Memory<object> args)
        {
            Events.PlaceRing = null;
            return UniTask.CompletedTask;
        }
        private void PlaceRing(int slotId)
        {
            TreasureManager.Instance.PlaceRingToSlot(slotId,Model.Id);
            ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
        public void Setup()
        {
            for (int i = 0; i < View.ListUISlotTreasureDeck.Count; i++)
            {
                View.ListUISlotTreasureDeck[i].Setup(i, Model.Id);
            }
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
    }
}