using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Screens;
using Pextension;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using TW.UGUI.Core.Modals;

[Serializable]
public class ScreenItemRewardContext 
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
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public UIResourceCoin UIResourceCoin { get; private set; }
        MiniPool<UIItemReward> uiItemRewardPool = new();
        [field: SerializeField] private Transform m_ItemRewardParent;
        [field: SerializeField] private UIItemReward m_UIItemRewardPrefab;
        [field: SerializeField] public Button ClaimButton { get; private set; }
        [field: SerializeField] public Button DoubleButton { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            uiItemRewardPool.OnInit(m_UIItemRewardPrefab, 3, m_ItemRewardParent);
            UIResourceCoin.Initialize(args);
            return UniTask.CompletedTask;
        }
        public void Setup(List<GameResource> gameResources)
        {
            //m_BtnClaim.gameObject.SetActive(!isIAP);
            uiItemRewardPool.Release();
            for (int i = 0; i < gameResources.Count; i++)
            {
                UIItemReward uiItemReward = uiItemRewardPool.Spawn(m_ItemRewardParent.position, Quaternion.identity);
                uiItemReward.Setup(gameResources[i]);
            }
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; } = new();
        [field: SerializeField] public UIView View { get; set; } = new();

        [field: SerializeField] private List<GameResource> m_ItemRewardPack = new();
        private Action m_CallBack;
        private Tween m_Tween;
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            View.ClaimButton.onClick.AddListener(OnClaimRewardList);
            View.DoubleButton.onClick.AddListener(OnDoubleReward);
            m_Tween = View.DoubleButton.transform.DOScale(Vector3.one * 1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        }

        public UniTask Cleanup(Memory<object> args)
        {
            m_Tween?.Kill();
            return UniTask.CompletedTask;
        }

        private void Setup(Memory<object> args)
        {
            //Debug.Log(args.Span[0] is List<GameResource>);
            m_ItemRewardPack = args.Span[0] as List<GameResource>;
            m_CallBack = args.Span[1] as Action;
            View.Setup(m_ItemRewardPack);
        }
        private void OnClaimRewardList()
        {
            PlayerResourceData.Instance.ClaimListResources(m_ItemRewardPack, 1);
            OnClaimComplete();
        }
        private void OnDoubleReward()
        {
            //AdsController.ShowRewardedVideo("Double Reward", () =>
            //{
            //    PlayerResource.ClaimListReward(m_ItemRewardPack, 2);
               OnClaimComplete();
            //});
        }
        private void OnClaimComplete()
        {
            m_CallBack?.Invoke();
            ScreenContainer.Find(ContainerKey.Screens).Pop(true);
        }

        public UniTask WillPushEnter(Memory<object> args)
        {
            Setup(args);
            return UniTask.CompletedTask;
        }
    }
}