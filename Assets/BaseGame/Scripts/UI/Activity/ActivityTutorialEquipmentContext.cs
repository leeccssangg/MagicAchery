using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

[Serializable]
public class ActivityTutorialEquipmentContext 
{
    public static class Events
    {
        public static Action<Button> OnFocusButton { get; set; }
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
        [field: SerializeField] public CanvasGroup MainView {get; private set;}  
        [field: SerializeField] public RectTransform FocusRect {get; private set;}
        [field: SerializeField] public Button ButtonInteract {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        
        private Button FocusButton { get; set; }
        public bool IsInteractable { get; private set; }
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            View.ButtonInteract.SetOnClickDestination(OnClickButtonInteract);
            Events.OnFocusButton += OnFocusButton;
            IsInteractable = false;
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.OnFocusButton -= OnFocusButton;
            return UniTask.CompletedTask;
        }

        private void OnFocusButton(Button button)
        {
            IsInteractable = true;
            FocusButton = button;
            View.ButtonInteract.interactable = true;
            View.FocusRect.gameObject.SetActive(true);
            
            RectTransform clickTarget = button.GetComponent<RectTransform>();
            View.FocusRect.pivot = clickTarget.pivot;
            View.FocusRect.position = clickTarget.position;
            View.FocusRect.sizeDelta = clickTarget.sizeDelta;
        }
        private void OnClickButtonInteract(Unit _)
        {   
            IsInteractable = false;
            View.ButtonInteract.interactable = false;
            View.FocusRect.gameObject.SetActive(false);
            FocusButton.onClick.Invoke();
        }
    }
}