using System;
using LitMotion;
using TMPro;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.UI;

public partial class UITalentTreeInfo : ACachedMonoBehaviour
{
    private static readonly string NodeLevelFormat = "Lvl. {0}/{1}";
    private static readonly string NodeRequireFormat = "Require: <style={0}> {1}/{2}";
    private InGameDataManager InGameDataManagerCache { get; set; }
    private InGameDataManager InGameDataManager => InGameDataManagerCache ??= InGameDataManager.Instance;
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    private TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    private PlayerResourceData PlayerResourceDataCache { get; set; }
    private PlayerResourceData PlayerResourceData => PlayerResourceDataCache ??= InGameDataManager.Instance.InGameData.PlayerResourceData;
    
    [field: SerializeField] private TextMeshProUGUI TextNodeName {get; set;}
    [field: SerializeField] private TextMeshProUGUI TextNodeDescription {get; set;}
    [field: SerializeField] private TextMeshProUGUI TextNodeLevel {get; set;}
    [field: SerializeField] private TextMeshProUGUI TextNodeRequire {get; set;}
    [field: SerializeField] private Button ButtonUpgrade {get; set;}
    [field: SerializeField] private GameObject MaxImage {get; set;}
    [field: SerializeField] private GameObject NodeRequireGroup {get; set;}
    
    private UITalentTreeNode UITalentTreeNode { get; set; }
    private TalentTreeNodeConfig TalentTreeNodeConfig { get; set; }
    private TalentTreeNodeData TalentTreeNodeData { get; set; }
    private TalentTreeNodeLevelConfig TalentTreeNodeCurrentLevelConfig { get; set; }
    private TalentTreeNodeLevelConfig TalentTreeNodeNextLevelConfig { get; set; }
    private MotionHandle ScaleMotionHandle { get; set; }
    private float CurrentScale { get; set; }
    private void Awake()
    {
        ButtonUpgrade.onClick.AddListener(OnClickUpgrade);
        UpdateScale(0);
    }

    public void ShowInfo(UITalentTreeNode uiTalentTreeNode)
    {
        UITalentTreeNode = uiTalentTreeNode;
        SetVisible(UITalentTreeNode != null);
        if (UITalentTreeNode == null) return;
        Transform.position = UITalentTreeNode.Transform.position;
        TalentTreeNodeConfig = UITalentTreeNode.TalentTreeNodeConfig;
        TalentTreeNodeData = TalentTreeManager.GetTalentNodeData(TalentTreeNodeConfig.NodeId);

        
        
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        TalentTreeNodeCurrentLevelConfig = TalentTreeNodeConfig.GetTalentTreeNodeLevelConfig(TalentTreeNodeData.NodeLevel.Value);
        TalentTreeNodeNextLevelConfig = TalentTreeNodeConfig.GetTalentTreeNodeLevelConfig(TalentTreeNodeData.NodeLevel.Value + 1);
        TextNodeName.text = TalentTreeNodeConfig.NodeName;
        TextNodeDescription.text = TalentTreeNodeCurrentLevelConfig.Description;
        TextNodeLevel.text = string.Format(NodeLevelFormat, 
            TalentTreeNodeData.NodeLevel.Value.ToString(), 
            TalentTreeNodeConfig.MaxLevelUpgrade.ToString());
        bool isMaxLevel = TalentTreeNodeData.NodeLevel.Value >= TalentTreeNodeConfig.MaxLevelUpgrade;
        if (!isMaxLevel)
        {
            GameResource.Type resourceType = TalentTreeNodeNextLevelConfig.GameResourceRequire.ResourceType;
            BigNumber requireResourceAmount = TalentTreeNodeNextLevelConfig.GameResourceRequire.Amount;
            BigNumber playerResourceAmount = PlayerResourceData.GetGameResource(resourceType).Amount;
            TextNodeRequire.text = string.Format(NodeRequireFormat,
                resourceType.ToString(), 
                requireResourceAmount.ToString(), 
                playerResourceAmount.ToString());
        }
        ButtonUpgrade.gameObject.SetActive(!isMaxLevel);
        MaxImage.SetActive(isMaxLevel);
        NodeRequireGroup.SetActive(!isMaxLevel);
    }
    public void OnClickUpgrade()
    {
        if (TalentTreeNodeNextLevelConfig == null) return;
        TalentTreeManager.UpgradeTalentNode(TalentTreeNodeConfig.NodeId);
        UpdateInfo();
        bool isMaxLevel = TalentTreeNodeData.NodeLevel.Value >= TalentTreeNodeConfig.MaxLevelUpgrade;
        if (isMaxLevel)
        {
            ScreenTalentTreeContext.Events.UpdateActiveNode?.Invoke();
        }
    }
    // public void SetActive(bool active)
    // {
    //     gameObject.SetActive(active);
    // }
    public void SetVisible(bool visible)
    {
        ScaleMotionHandle.TryCancel();
        if (visible)
        {
            UpdateScale(0);
        }
        ScaleMotionHandle = LMotion.Create(CurrentScale, visible ? 1 : 0, 0.25f)
            .WithEase(visible ? Ease.OutBack : Ease.InBack)
            .Bind(UpdateScaleCache);
    }
    [ACacheMethod]
    private void UpdateScale(float scale)
    {
        CurrentScale = scale;
        Transform.localScale = Vector3.one * scale;
    }
}