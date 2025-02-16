using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Modals;
//using UGUI.Core.Modals;
using UnityEngine.UI;

[Serializable]
public class ModalSettingsContext 
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
        [field: SerializeField] public ReactiveValue<bool> MusicActive { get; private set; }
        [field: SerializeField] public ReactiveValue<bool> SoundActive { get; private set; }
        [field: SerializeField] public ReactiveValue<bool> VibrateActive { get; private set; }
        
        public UniTask Initialize(Memory<object> args)
        {
            MusicActive = SettingData.Instance.MusicActive;
            SoundActive = SettingData.Instance.SoundActive;
            VibrateActive = SettingData.Instance.VibrateActive;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public UIToggleButton ToggleButtonMusic {get; private set;}
        [field: SerializeField] public UIToggleButton ToggleButtonSound {get; private set;}
        [field: SerializeField] public UIToggleButton ToggleButtonVibration {get; private set;}
        [field: SerializeField] public Button ButtonClose {get; private set;}
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
            View.ToggleButtonMusic.SetupValue(Model.MusicActive);
            View.ToggleButtonSound.SetupValue(Model.SoundActive);
            View.ToggleButtonVibration.SetupValue(Model.VibrateActive);
            
            View.ToggleButtonMusic.OnClickButton.AddListener(OnClickButtonMusic);
            View.ToggleButtonSound.OnClickButton.AddListener(OnClickButtonSound);
            View.ToggleButtonVibration.OnClickButton.AddListener(OnClickButtonVibration);

            View.ButtonClose.SetOnClickDestination(ClickButtonClose);
        }
        private void OnClickButtonMusic(bool value)
        {
            Model.MusicActive.Value = value;
            InGameDataManager.Instance.SaveData();
        }
        private void OnClickButtonSound(bool value)
        {
            Model.SoundActive.Value = value;
            InGameDataManager.Instance.SaveData();
        }
        private void OnClickButtonVibration(bool value)
        {
            Model.VibrateActive.Value = value;
            InGameDataManager.Instance.SaveData();
        }
        private void ClickButtonClose(Unit unit)
        {
            ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
        }
    }
}