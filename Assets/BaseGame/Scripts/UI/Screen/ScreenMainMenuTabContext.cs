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

[Serializable]
public class ScreenMainMenuTabContext 
{
    public enum MainMenuTab
    {
        Lock = -2,
        None = -1,
        TalentTree = 0,
        Ring = 1,
        Stats = 2,
        Shop = 3,
        

    }
    public static class Events
    {
        public static Action<bool> ForceShowTab { get; set; }
        public static Action<bool> InteractableCanvasGroup { get; set; }    
    }
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
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
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public TabGroupButton TabGroupButton { get; private set; }
        [field: SerializeField] public FeelAnimation ShowTabAnimation {get; private set;}
        [field: SerializeField] public FeelAnimation HideTabAnimation {get; private set;}
        [field: SerializeField] public Button ButtonSettings {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
      
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; }
        [field: SerializeField] public UIView View { get; set; } = new();
        private int[] SheetId { get; set; } = new int[4];
        [field: SerializeField] public MainMenuTab CurrentMainMenuTab { get; private set; } = MainMenuTab.None;

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            //View.ButtonSettings.SetOnClickDestination(OnClickButtonSettings);

            //SheetOptions options0 = new SheetOptions(nameof(SheetDailyQuest), OnSheetLoaded);
            //SheetId[0] = await View.SheetMainMenuContainer.RegisterAsync(options0, args);

            //SheetOptions options1 = new SheetOptions(nameof(SheetWeapon), OnSheetLoaded);
            //SheetId[1] = await View.SheetMainMenuContainer.RegisterAsync(options1, args);

            //SheetOptions option2 = new SheetOptions(nameof(SheetMainMenu), OnSheetLoaded);
            //SheetId[2] = await View.SheetMainMenuContainer.RegisterAsync(option2, args);

            //SheetOptions option3= new SheetOptions(nameof(SheetShop), OnSheetLoaded);
            //SheetId[3] = await View.SheetMainMenuContainer.RegisterAsync(option3, args);

            View.TabGroupButton.Setup<MainMenuTab>(OnOpenTab);
            Events.ForceShowTab = OnForceShowTab;
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.ForceShowTab = null;
            Events.InteractableCanvasGroup = null;
            return UniTask.CompletedTask;
        }

        public void DidPushEnter(Memory<object> args)
        {
            //ShowSheet(1).Forget();
            OpenTab(MainMenuTab.Stats);
        }

        private void OnSheetLoaded(int sheetId, Sheet sheet, Memory<object> args)
        {
            (sheet as ISetupAble)?.Setup();
        }
        private void OnOpenTab(MainMenuTab tab)
        {
            if (CurrentMainMenuTab == tab) return;
            CloseAllPanel();
            switch (tab)
            {
                case MainMenuTab.Stats:
                    ScreenOptions screenStat = new ScreenOptions(nameof(ScreenStats), stack: false);
                    ScreenContainer.Find(ContainerKey.TabScreens).PushAsync(screenStat);
                    break;
                case MainMenuTab.Ring:
                    ScreenOptions screenTreasure = new ScreenOptions(nameof(ScreenTreasure), stack: false);
                    ScreenContainer.Find(ContainerKey.TabScreens).PushAsync(screenTreasure);
                    break;
                case MainMenuTab.Shop:
                    break;
                case MainMenuTab.TalentTree:
                    ScreenOptions screenTalentTree = new ScreenOptions(nameof(ScreenTalentTree), stack: false);
                    ScreenContainer.Find(ContainerKey.TabScreens).PushAsync(screenTalentTree);
                    break;
                default:
                    break;
            }
            CurrentMainMenuTab = tab;
        }
        public void OpenTab(MainMenuTab tab)
        {
            if (tab == MainMenuTab.None)
            {
                if (CurrentMainMenuTab == MainMenuTab.None)
                {
                    OnClickTabButton(MainMenuTab.Stats);
                }
                else
                {
                    tab = CurrentMainMenuTab;
                    CurrentMainMenuTab = MainMenuTab.None;
                    OnClickTabButton(tab);
                }
            }
            else
            {
                OnClickTabButton(tab);
            }
        }
        private void OnClickTabButton(MainMenuTab tab)
        {
            View.TabGroupButton.OnClickButton(tab);
        }
        private void CloseAllPanel()
        {

        }
        private void OnForceShowTab(bool isShow)
        {
            if (isShow)
            {
                View.ShowTabAnimation.Play();
            }
            else
            {
                View.HideTabAnimation.Play();
            }
        }
        private void OnClickButtonAvatar(Unit _)
        {
            Debug.Log("OnClickButtonAvatar");
        }
        private void OnClickButtonSettings(Unit _)
        {
            ViewOptions view = new ViewOptions(nameof(ModalSettings));
            ModalContainer.Find(ContainerKey.Modals).PushAsync(view);
        }
    }
}