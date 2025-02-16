using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using TW.Utility.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Views;
using System;

public class UIDailyQuestPointGift : ACachedMonoBehaviour
{
    [Header("UI Daily Quest Point Gift")]
    [SerializeField] private GameObject gOParticleImage;
    [SerializeField] private QuestStage m_PointRewardPack;
    [SerializeField] private TextMeshProUGUI m_TxtPoint;
    [SerializeField] private Image m_ImgNotClaimed;
    [SerializeField] private GameObject m_ImgReadyClaim;
    [SerializeField] private Image m_ImgClaimed;
    [SerializeField] private Button m_BtnClaim;
    [SerializeField] private bool m_IsClaimable;

    public Vector3 WorldPosition { get => m_BtnClaim.transform.position; set => m_BtnClaim.transform.position = value; }

    private UnityAction<List<GameResource>, Vector3> m_SubAction;
    private void Awake()
    {
        m_BtnClaim.onClick.AddListener(OnClaim);
    }

    public void Setup(QuestStage pointRewardPack, UnityAction<List<GameResource>, Vector3> actionCallBack)
    {
        m_PointRewardPack = pointRewardPack;
        m_SubAction = actionCallBack;
        int curStage = AllQuestManager.Instance.GetCurrentDailyQuestStage();
        m_IsClaimable = (AllQuestManager.Instance.GetDailyQuestConfigs().GetStageReward().IndexOf(m_PointRewardPack) == curStage) &&
                        AllQuestManager.Instance.IsGoodToClaimDailyStageReward();
        bool isClaimed = curStage == AllQuestManager.Instance.GetLastDailyQuestStageRewardId()
            || m_PointRewardPack.requiredPoint < AllQuestManager.Instance.GetDailyQuestConfigs().GetStageReward()[curStage].requiredPoint;
        //m_TxtPoint.text = m_PointRewardPack.requiredPoint.ToString();
        m_TxtPoint.SetText($"{m_PointRewardPack.requiredPoint}");
        m_ImgNotClaimed.gameObject.SetActive(!m_IsClaimable && !isClaimed);
        m_ImgReadyClaim.SetActive(m_IsClaimable);
        m_ImgClaimed.gameObject.SetActive(isClaimed);
        //m_BtnClaim.interactable = m_IsClaimable;
        gOParticleImage.SetActive(m_IsClaimable);
    }
    private void OnClaim()
    {

        if (!m_IsClaimable)
        {
            ViewOptions options = new ViewOptions(nameof(ActivityGiftDetail));
            Memory<object> args = new Memory<object>(new object[] { WorldPosition, m_PointRewardPack.rewards });
            ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options, args);
            return;
        }
        else
            //List<GameResource> list = m_PointRewardPack.rewards;
            m_SubAction?.Invoke(m_PointRewardPack.rewards, m_ImgReadyClaim.transform.position);
    }
}
