using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using System.Collections.Generic;
using Pextension;
using R3.Triggers;
using System.Collections;
using TW.Utility.Extension;
using System.Threading;
using TW.UGUI.MVPPattern;

[Serializable]
public class ActivityGiftDetailContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action HideActivity { get; set; }


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
        private RectTransform m_RectTransform;
        public RectTransform RectTransform => m_RectTransform = m_RectTransform != null ? m_RectTransform : MainView.GetComponent<RectTransform>();
        [field: Title("UIC Popup")]
        [field: SerializeField]
        public UIItemReward UIItemRewardPrefab { get; private set; }

        [field: SerializeField] public MiniPool<UIItemReward> UIItemRewardPool { get; private set; }
        [field: SerializeField] public GameObject GoGiftDetail { get; private set; }
        [field: SerializeField] public Transform TfGoGiftDetail { get; private set; }
        [field: SerializeField] public Transform TfContainer { get; private set; }
        [field: SerializeField] public RectTransform RectTfGoGiftDetail { get; private set; }
        [field: SerializeField] public RectTransform RectTfContent { get; private set; }
        [field: SerializeField] public Vector3 AdditionPosition { get; private set; }
        [field: SerializeField] public Vector2 AdditionAnchoredPosition { get; private set; }
        [field: SerializeField] public Vector3 StartContentPosition { get; private set; }
        [field: SerializeField] public Vector3 StartContentLocalPosition { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            UIItemRewardPool.OnInit(UIItemRewardPrefab,3,TfContainer);
            StartContentPosition = RectTfContent.position;
            StartContentLocalPosition = RectTfContent.localPosition;
            return UniTask.CompletedTask;
        }

    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            Events.HideActivity = HideActivity;
            View.MainView.LateUpdateAsObservable()
                .Subscribe(OnLateUpdate)
                .AddTo(View.MainView);
        }  
        public UniTask Cleanup(Memory<object> args)
        {
            Events.HideActivity = null;
            return UniTask.CompletedTask;
        }
        private void OnLateUpdate(Unit unit)
        {
            //if (!View.GoGiftDetail.activeInHierarchy) return;
            if (Input.GetMouseButtonDown(0) &&
                !RectTransformUtility.RectangleContainsScreenPoint(View.RectTfGoGiftDetail, Input.mousePosition))
            {
                ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityGiftDetail));
            };
        }
        public async UniTask OnShowGiftDetail(Vector3 position, List<GameResource> rewards)
        {
            View.UIItemRewardPool.Collect();
            //TfGoGiftDetail.position = Vector3.zero;
            View.GoGiftDetail.SetActive(true);
            View.TfGoGiftDetail.position = position + View.AdditionPosition;
            for (int i = 0; i < rewards.Count; i++)
            {
                UIItemReward uiItemReward = View.UIItemRewardPool.Spawn(View.TfContainer.position, Quaternion.identity);
                uiItemReward.SetShowParticle(false);
                uiItemReward.Setup(rewards[i]);
                uiItemReward.transform.SetParent(View.TfContainer);
                uiItemReward.transform.localScale = Vector3.one;
                uiItemReward.transform.SetSiblingIndex(i);
            }
            await UniTask.DelayFrame(1, cancellationToken: View.MainView.GetCancellationTokenOnDestroy());
            CheckSize();
        }
        public void CheckSize()
        {
            // TODO: Fix later
            Vector2 canvasSize = Screen.safeArea.size;
            Vector2 contentSize = View.RectTfContent.sizeDelta * View.RectTfContent.lossyScale;
            View.RectTfContent.localPosition = View.StartContentLocalPosition;
            if (View.RectTfContent.position.x + contentSize.x / 2 > canvasSize.x)
            {
                float diff = View.RectTfContent.position.x + contentSize.x / 2 - canvasSize.x;
                View.RectTfContent.localPosition = View.StartContentLocalPosition - new Vector3(diff + 20, 0, 0);

            }
            else if (View.RectTfContent.position.x - contentSize.x / 2 < 0)
            {
                float diff = contentSize.x / 2 - View.RectTfContent.position.x;
                View.RectTfContent.localPosition = View.StartContentLocalPosition + new Vector3(diff + 20, 0, 0);
            }
        }
        public async UniTask WillEnter(Memory<object> args)
        {
            Vector3 position = args.Span[0] is Vector3 ? (Vector3)args.Span[0] : Vector3.zero;
            Debug.Log(position);
            List<GameResource> rewards = args.Span[1] is List<GameResource> ? (List<GameResource>)args.Span[1] : new List<GameResource>();
            await OnShowGiftDetail(position, rewards);
        }
        public void HideActivity()
        {
            ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityGiftDetail)); ;
        }
    }
}