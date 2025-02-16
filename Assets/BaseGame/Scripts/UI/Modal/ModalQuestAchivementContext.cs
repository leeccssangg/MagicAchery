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
using static ScreenMainMenuTabContext;
using UnityEditor.PackageManager.Requests;

[Serializable]
public class ModalQuestAchivementContext 
{
    public enum QuestAchivementTab
    {
        None = -1,
        DailyQuest,
        Achievement,
    }
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<DailyQuest, Action> ClaimDailyQuest { get; set; }
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
        [field: SerializeField] public TabGroupButton TabGroupButton {get; private set;}
        [field: SerializeField] public SheetQuestAchivementContainer SheetQuestAchivementContainer { get; private set;}
        [field: SerializeField] public Button BtnClose { get; private set;}
        [field: SerializeField] public UIResourceCoin UIResourceCoin { get; private set;}
        
        public UniTask Initialize(Memory<object> args)
        {
            UIResourceCoin.Initialize(args);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();    
        private int[] SheetId { get; set; } = new int[2];
        [field: SerializeField] public QuestAchivementTab CurrentTab { get; private set; } = QuestAchivementTab.None;

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            SheetOptions options0 = new SheetOptions(nameof(SheetDailyQuest), OnSheetLoaded);
            SheetId[0] = await View.SheetQuestAchivementContainer.RegisterAsync(options0, args);
            //(SheetId[0] as SheetWeapon).InitSheet().Forget();

            SheetOptions option1 = new SheetOptions(nameof(SheetAchivement), OnSheetLoaded);
            SheetId[1] = await View.SheetQuestAchivementContainer.RegisterAsync(option1, args);

            View.TabGroupButton.Setup<QuestAchivementTab>(OnOpenTab);

            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
        }
        public UniTask Cleanup(Memory<object> args)
        {
            //Events.DelayIncreaseValueMoney = null;
            //Events.InteractableCanvasGroup = null;
            //Events.NotiWeapon = null;
            return UniTask.CompletedTask;
        }

        public void DidPushEnter(Memory<object> args)
        {
            //ShowSheet(1).Forget();
            OpenTab(QuestAchivementTab.DailyQuest);
            //EquipmentManager.Instance.TryNotiWeapon();
            //SkinManager.Instance.TryNotiSkin();
            //StatManager.Instance.TryNotification();
        }

        private void OnSheetLoaded(int sheetId, Sheet sheet, Memory<object> args)
        {
            (sheet as ISetupAble)?.Setup();
        }
        private void OnOpenTab(QuestAchivementTab tab)
        {
            if (CurrentTab == tab) return;
            //Debug.Log("OnOpenTab: " + tab);
            CloseAllPanel();
            switch (tab)
            {
                case QuestAchivementTab.DailyQuest:
                    ShowSheet(0).Forget();
                    //OpenTab(MainMenuTab.Weapon);
                    break;
                case QuestAchivementTab.Achievement:
                    ShowSheet(1).Forget();
                    //OpenTab(MainMenuTab.Skin);
                    break;
                default:
                    break;
            }
            CurrentTab = tab;
        }
        private async UniTask ShowSheet(int index)
        {
            if (View.SheetQuestAchivementContainer.IsInTransition)
            {
                return;
            }

            if (View.SheetQuestAchivementContainer.ActiveSheetId == SheetId[index])
            {
                // This sheet is already displayed.
                return;
            }
            //OpenTab((MainMenuTab)index);
            await View.SheetQuestAchivementContainer.ShowAsync(SheetId[index], true);
        }
        public void OpenTab(QuestAchivementTab tab)
        {
            if (tab == QuestAchivementTab.None)
            {
                if (CurrentTab == QuestAchivementTab.None)
                {
                    OnClickTabButton(QuestAchivementTab.DailyQuest);
                }
                else
                {
                    tab = CurrentTab;
                    CurrentTab = QuestAchivementTab.None;
                    OnClickTabButton(tab);
                }
            }
            else
            {
                OnClickTabButton(tab);
            }
            //OnClickTabButton(tab);
            //m_ContentTransform.GetComponent<RectTransform>().DOAnchorPosY(1040, 0.45f).SetEase(Ease.OutBack);
            // m_DoTweenAnimation.tween.Restart();
        }
        private void OnClickTabButton(QuestAchivementTab tab)
        {
            View.TabGroupButton.OnClickButton(tab);
        }
        private void CloseAllPanel()
        {

        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
    }
}