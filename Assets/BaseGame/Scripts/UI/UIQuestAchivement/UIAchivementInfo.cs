using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using TW.Utility.CustomComponent;
using R3;
using UnityEditor.PackageManager.Requests;
using TW.Reactive.CustomComponent;

public class UIAchivementInfo : ACachedMonoBehaviour
{
    [field: SerializeField] public Achivement Achivement { get; private set; }
    [field: SerializeField] public Image ImgResourceIcon { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtResourceAmount { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtDescription { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtProcess { get; private set; }
    [field: SerializeField] public Slider SliderProcess { get; private set; }
    [field: SerializeField] public Button BtnClaim { get; private set; }
    [field: SerializeField] public GameObject GoImgNotReadyClaim { get; private set; }
    [field: SerializeField] public GameObject GoImgReadyClaim { get; private set; }
    [field: SerializeField] public GameObject GoImgMaxLevel { get; private set; }
    public bool IsClaimable => Achivement.GetProgress() >= 1 && !Achivement.IsMaxLevel();
    public bool IsMaxLevel => Achivement.IsMaxLevel();
    [field: SerializeField] public FeelAnimation AnimClaim { get; private set; }

    private void Awake()
    {
        BtnClaim.SetOnClickDestination(OnClickBtnClaim);
    }
    public void Setup(Achivement achivement)
    {
        Achivement = achivement;
        TxtDescription.SetText($"{Achivement.GetDescription()}", Achivement.targetAmount);
        TxtProcess.SetText($"{Achivement.collected}/{Achivement.targetAmount}");
        SliderProcess.DOValue(Achivement.GetProgress(), 0.5f);
        GoImgMaxLevel.SetActive(Achivement.IsMaxLevel());
        GoImgNotReadyClaim.SetActive((!Achivement.IsCompleted()) || Achivement.IsMaxLevel());
        GoImgReadyClaim.SetActive(Achivement.IsCompleted() && !Achivement.IsMaxLevel());
        BtnClaim.gameObject.SetActive(Achivement.GetProgress() >= 1 && !Achivement.IsMaxLevel());
        BtnClaim.interactable = IsClaimable;
    }
    public int GetId()
    {
        return Achivement.id;
    }
    private void OnClickBtnClaim(Unit _)
    {
        BtnClaim.interactable = false;
        AnimClaim?.Play();
    }
    public void OnClaim()
    {
        SheetAchivement.Events.ClaimAchivement?.Invoke(Achivement, OnClaimCallBack);
    }
    private void OnClaimCallBack()
    {
        //m_ImgClaimed.gameObject.SetActive(true);
        //m_BtnClaim.gameObject.SetActive(false);
    }
}
