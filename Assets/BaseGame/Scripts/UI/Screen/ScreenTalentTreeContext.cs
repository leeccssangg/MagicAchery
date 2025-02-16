using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Screens;
using UnityEngine.UI;

[Serializable]
public class ScreenTalentTreeContext 
{
    public static class Events
    {
        public static Action<UITalentTreeNode> ShowUITalentTreeNodeInfo { get; set; } 
        public static Action UpdateActiveNode { get; set; }
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
        [field: SerializeField] public CustomScrollRect CustomScrollRect {get; private set;}
        [field: SerializeField] public UITalentTreeNode[] UITalentTreeNodeArray {get; private set;}
        [field: SerializeField] public UITalentTreeInfo UITalentTreeInfo {get; private set;}
        [field: SerializeField] public Button ButtonHideTalentTreeInfo {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            foreach (UITalentTreeNode node in UITalentTreeNodeArray)
            {
                node.Setup();
                node.UpdateActive();
            }
            return UniTask.CompletedTask;
        }
        public void OnClickNode(UITalentTreeNode uiTalentTreeNode)
        {
            UITalentTreeInfo.ShowInfo(uiTalentTreeNode);
        }
        [Button]
        public void ResizeContent()
        {
            Vector2 size = Vector2.zero;
            foreach (UITalentTreeNode node in UITalentTreeNodeArray)
            {
                if (!node.IsActiveNode) continue;
                size.x = Mathf.Max(size.x, Mathf.Abs(node.transform.localPosition.x));
                size.y = Mathf.Max(size.y, Mathf.Abs(node.transform.localPosition.y));
            }
            CustomScrollRect.ChangeResizeContent((size + Vector2.one * 400 )* 2);
        }
        public void UpdateActiveNode()
        {
            foreach (UITalentTreeNode node in UITalentTreeNodeArray)
            {
                node.UpdateActive();
            }

            ResizeContent();
        }
        private void OnScrollChanged()
        {
            Events.ShowUITalentTreeNodeInfo?.Invoke(null);
        }
        
#if UNITY_EDITOR
        [Button]
        private void FindTalentTreeNode()
        {
            UnityEditor.EditorUtility.SetDirty(MainView.GetComponent<ScreenTalentTree>());
            UITalentTreeNodeArray = CustomScrollRect.GetComponentsInChildren<UITalentTreeNode>(true);
        }
#endif
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            
            Events.ShowUITalentTreeNodeInfo += View.UITalentTreeInfo.ShowInfo;
            Events.UpdateActiveNode += View.UpdateActiveNode;

            View.ButtonHideTalentTreeInfo.SetOnClickDestination(OnClickButtonHideTalentTreeInfo);
        }
        
        public UniTask Cleanup(Memory<object> args)
        {
            Events.ShowUITalentTreeNodeInfo -= View.UITalentTreeInfo.ShowInfo;
            Events.UpdateActiveNode -= View.UpdateActiveNode;
            return UniTask.CompletedTask;
        }

        public void DidPushEnter(Memory<object> args)
        {
            View.ResizeContent();
        }
        

        private void OnClickButtonHideTalentTreeInfo(Unit unit)
        {
            View.UITalentTreeInfo.SetVisible(false);
        }
    }
}