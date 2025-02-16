using System;
using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEditor;
using UnityEngine.UI;

[SelectionBase]
public partial class UITalentTreeNode : ACachedMonoBehaviour
{
    private TalentTreeManager TalentTreeManagerCache { get; set; }
    private TalentTreeManager TalentTreeManager => TalentTreeManagerCache ??= TalentTreeManager.Instance;
    
    [field: InlineEditor, InlineButton("@CreateNewTalentTreeNodeConfig()", "+")]
    [field: SerializeField] public TalentTreeNodeConfig TalentTreeNodeConfig {get; private set;}
    [field: SerializeField] public Image ImageIcon {get; private set;}
    [field: SerializeField] public Image ImageProgress {get; private set;}
    [field: SerializeField] public List<UILinkNode> UILinkNodeList {get; private set;} = new();
    [field: SerializeField] private Button ButtonShowInfo {get; set;}
    public bool IsActiveNode { get; private set; }
    private TalentTreeNodeData TalentTreeNodeData { get; set; }

    public void Setup()
    {
        ButtonShowInfo.onClick.AddListener(OnClickButtonShowInfo);
        TalentTreeNodeData = TalentTreeManager.GetTalentNodeData(TalentTreeNodeConfig.NodeId);
        TalentTreeNodeData.NodeLevel.ReactiveProperty.Subscribe(UpdateProgress).AddTo(this);
    }
    public void UpdateActive()
    {
        bool isActive = true;
        foreach (int nodeId in TalentTreeNodeConfig.RequireNode)
        {
            if (TalentTreeManager.IsTalentNodeUnlockedAndMaxLevel(nodeId)) continue;
            isActive = false;
            break;
        }
        SetActive(isActive);
    }
    
    public void SetActive(bool active)
    {
        IsActiveNode = active;
        gameObject.SetActive(IsActiveNode);
        foreach (UILinkNode uiLinkNode in UILinkNodeList)
        {
            uiLinkNode.SetActive(IsActiveNode);
        }
    }
    private void OnClickButtonShowInfo()
    {
        ScreenTalentTreeContext.Events.ShowUITalentTreeNodeInfo?.Invoke(this);
    }
    private void UpdateProgress(int level)
    {
        ImageProgress.fillAmount = (float)level / TalentTreeNodeConfig.MaxLevelUpgrade;
    }
}
#if UNITY_EDITOR
public partial class UITalentTreeNode
{

    [Button]
    public void EditorSetup()
    {
        if (TalentTreeNodeConfig == null) return;
        EditorUtility.SetDirty(ImageIcon);
        EditorUtility.SetDirty(this);
        ImageIcon.sprite = TalentTreeNodeConfig.NodeIcon;
        foreach (UILinkNode link in UILinkNodeList)
        {
            if (link == null) continue;
            DestroyImmediate(link.gameObject);
        }
        UILinkNodeList.Clear();
        ScreenTalentTree screenTalentTree = this.GetComponentInParentUntilNoParent<ScreenTalentTree>();
        Transform lineContainer = screenTalentTree.Transform.FindChildOrCreate("LinkContainer");
        
        UILinkNode linkNodePrefab = AssetDatabase.LoadAssetAtPath<UILinkNode>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Prefab UILinkNode")[0]));

        foreach (int nodeId in TalentTreeNodeConfig.RequireNode)
        {
            UILinkNode linkNode = PrefabUtility.InstantiatePrefab(linkNodePrefab, lineContainer) as UILinkNode;
            if (linkNode == null) return;
            linkNode.Transform.SetParent(lineContainer);
            UITalentTreeNode requireNode = FindUITalentTreeNode(nodeId);
            if (requireNode == null) continue;
            linkNode.Setup(requireNode.Transform, Transform);
            UILinkNodeList.Add(linkNode);
        }
        
        gameObject.name = $"Node_{TalentTreeNodeConfig.NodeId}_{TalentTreeNodeConfig.NodeName}";
    }

    private UITalentTreeNode FindUITalentTreeNode(int id)
    {
        ScreenTalentTree screenTalentTree = this.GetComponentInParentUntilNoParent<ScreenTalentTree>();
        UITalentTreeNode[] uITalentTreeNodeArray =screenTalentTree.GetComponentsInChildren<UITalentTreeNode>();
        foreach (UITalentTreeNode uITalentTreeNode in uITalentTreeNodeArray)
        {
            if (uITalentTreeNode.TalentTreeNodeConfig.NodeId == id)
            {
                return uITalentTreeNode;
            }
        }

        return null;
    }

    private void CreateNewTalentTreeNodeConfig()
    {
        TalentTreeNodeConfig talentTreeNodeConfig = ScriptableObject.CreateInstance<TalentTreeNodeConfig>();
        AssetDatabase.CreateAsset(talentTreeNodeConfig, "Assets/BaseGame/ScriptableObjects/TalentTreeNodeConfig/TalentTreeNodeConfig.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(this);
        TalentTreeNodeConfig = AssetDatabase.LoadAssetAtPath<TalentTreeNodeConfig>("Assets/BaseGame/ScriptableObjects/TalentTreeNodeConfig/TalentTreeNodeConfig.asset");
        UILinkNodeList.Clear();
    }
}
#endif