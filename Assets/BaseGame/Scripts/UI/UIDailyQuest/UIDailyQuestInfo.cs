using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using TW.Utility.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Views;
using System;
//using SDK;

public class UIDailyQuestInfo : ACachedMonoBehaviour
{
    [Title("UI Daily Quest Info")]
    [SerializeField] private RectTransform rectTfRoot;
    [SerializeField] private DailyQuest m_Quest;
    [SerializeField] private TextMeshProUGUI m_TxtDes;
    [SerializeField] private TextMeshProUGUI m_TxtProcess;
    [SerializeField] private TextMeshProUGUI m_TxtPoint;
    //[SerializeField] private Image m_ImgClaimed;
    //[SerializeField] private Image m_ImgNotClaimed;
    [SerializeField] private Button m_BtnClaim;

    [Title("", "Gift Detail")]
    [SerializeField] private GameObject m_GoImgNotClaimable;
    [SerializeField] private GameObject m_GoImgClaimed;
    [SerializeField] private GameObject m_GoReadyClaim;
    [SerializeField] private Button m_BtnGiftDetail;

    private UnityAction<DailyQuest, UnityAction> m_ClaimCompletedCallBack;
    public LocalMoveTweenTransition m_Tween;
    [SerializeField] private Slider m_QuestProcess;
    [SerializeField] private FeelAnimation TweenClaimQuest;

    private readonly float _posXValue = 1000;
    private float defaultDelay = -1;
    public bool IsClaimed => m_Quest.IsClaimed();
    public bool IsClaimable => m_Quest.GetProgress() >= 1 && !m_Quest.IsClaimed();

    public bool IsDoing => m_Quest.GetProgress() < 1 && m_Quest.GetProgress() > 0;

    private void Awake()
    {
        m_BtnClaim.onClick.AddListener(OnClickBtnClaim);
        //m_BtnGiftDetail.onClick.AddListener(ShowGiftDetail);
    }
    public void Setup(DailyQuest quest)
    {
        m_Quest = quest;
        m_TxtDes.SetText($"{m_Quest.GetDescription()}",m_Quest.tgm);
        m_TxtProcess.SetText($"{m_Quest.cl}/{m_Quest.tgm}");
        m_TxtPoint.SetText($"{m_Quest.pt}");
        m_QuestProcess.DOValue(m_Quest.GetProgress(), 0.5f);
        //m_ImgClaimed.gameObject.SetActive(m_Quest.IsClaimed());
        m_GoImgClaimed.SetActive(m_Quest.IsClaimed());
        m_GoImgNotClaimable.SetActive((!m_Quest.IsCompleted()) || m_Quest.IsClaimed());
        m_GoReadyClaim.SetActive(m_Quest.IsCompleted() && !m_Quest.IsClaimed());
        m_BtnClaim.gameObject.SetActive(m_Quest.GetProgress() >= 1 && !m_Quest.IsClaimed());

    }
    public void UpdateData()
    {
        m_TxtProcess.SetText($"{m_Quest.cl}/{m_Quest.tgm}");
        m_TxtPoint.SetText($"{m_Quest.pt}");
        m_QuestProcess.DOValue(m_Quest.GetProgress(), 0.5f);
        //m_ImgClaimed.gameObject.SetActive(m_Quest.IsClaimed());
        m_GoImgClaimed.SetActive(m_Quest.IsClaimed());
        m_GoImgNotClaimable.SetActive((!m_Quest.IsCompleted()) || m_Quest.IsClaimed());
        m_GoReadyClaim.SetActive(m_Quest.IsCompleted() && !m_Quest.IsClaimed());
        m_BtnClaim.gameObject.SetActive(m_Quest.GetProgress() >= 1 && !m_Quest.IsClaimed());
    }
    public int GetQuestId()
    {
        return m_Quest.id;
    }
    public void SetUpDelay(int spawnId)
    {
        if (defaultDelay <= 0)
        {
            defaultDelay = m_Tween.Delay;
        }
        m_Tween.Delay = defaultDelay + 0.15f * spawnId;
    }
    private void OnClickBtnClaim()
    {
        m_BtnClaim.interactable = false;
        TweenClaimQuest.Play();
    }
    public void OnClaim()
    {
        SheetDailyQuest.Events.ClaimQuest?.Invoke(m_Quest, OnClaimCallBack);
    }

    //private void ShowGiftDetail()
    //{
    //    ViewOptions options = new ViewOptions(nameof(ActivityGiftDetail));
    //    Memory<object> args = new Memory<object>(new object[] { m_BtnClaim.transform.position, m_Quest.GetQuestConfig().reward });
    //    ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options, args);
    //}

    private void OnClaimCallBack()
    {
        //m_ImgClaimed.gameObject.SetActive(true);
        //m_BtnClaim.gameObject.SetActive(false);
    }
}
